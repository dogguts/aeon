using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.ObjectModel;
using System.Collections.Immutable;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public class ReadonlyRepository<TQuery, TDbContext> : IReadonlyRepository<TQuery>, IReadonlyRepositoryAsync<TQuery>
            where TQuery : class
            where TDbContext : DbContext {

        protected readonly TDbContext _context;
        protected IQueryable<TQuery> _dbQuery;

        public ReadonlyRepository(TDbContext context, bool contextOnly = false) {
            _context = context;
            if (!contextOnly) {
                _dbQuery = context.Query<TQuery>();
            }
        }


        private IQueryable<TQuery> WithIncludes(IRepositoryInclude<TQuery> includes) {
            var queryWithIncludes = _dbQuery.AsQueryable();
            if (includes != null) {
                //   including all expression includes
                queryWithIncludes = includes?.IncludePaths?.Aggregate(queryWithIncludes,
                        (current, include) => current.Include(include)) ?? queryWithIncludes;
            }
            return queryWithIncludes;
        }

        public async Task<TQuery> GetAsync(params object[] keyValues) {
            return await GetAsync(null, keyValues);
        }

        public virtual TQuery Get(params object[] keyValues) {
            return GetAsync(keyValues).Result;
        }


        public async Task<TQuery> GetAsync(IRepositoryInclude<TQuery> includes, params object[] keyValues) {
            var entityType = _context.Model.FindEntityType(typeof(TQuery));
            var primaryKeyProperties = entityType.FindPrimaryKey()?.Properties;
            if (entityType.IsQueryType) {
                throw new InvalidOperationException("Retrieving a query type by primary key is not supported since those EntityTypes don't have a primary key");
                //NOTE: or?: return await Task.FromException<TQuery>(new InvalidOperationException(... 
            }
            if (primaryKeyProperties == null) {
                //NOTE: or?: return await Task.FromException<TQuery>(new InvalidOperationException(... 
                throw new InvalidOperationException($"The entity type '{typeof(TQuery).Name}' requires a primary key to be defined.'");
            }
 
            BinaryExpression predicate = null;
            var entityParameter = Expression.Parameter(typeof(TQuery), "p");

            for (var i = 0; i < primaryKeyProperties.Count; i++) {
                var property = primaryKeyProperties[i];

                var leftExpr = Expression.Property(entityParameter, property.Name);

                object keyValuePart = keyValues[i];
                Expression<Func<object>> closure = () => keyValuePart;
                var rightExpr = Expression.Convert(closure.Body, leftExpr.Type);

                var equalsExpression = Expression.Equal(leftExpr, rightExpr);


                predicate = predicate == null ? equalsExpression : Expression.AndAlso(predicate, equalsExpression);
            }
            var criteria = Expression.Lambda<Func<TQuery, bool>>(predicate, entityParameter);

            var filter = new RepositoryFilter<TQuery>(criteria, includes);
            var (Data, _) = await GetWithFilterAsync(filter);

            return Data.FirstOrDefault();
        }

        public virtual TQuery Get(IRepositoryInclude<TQuery> includes, params object[] keyValues) {
            return GetAsync(includes, keyValues).Result;
        }
        public async Task<IEnumerable<TQuery>> AllAsync(IRepositoryInclude<TQuery> includes) {
            IQueryable<TQuery> set = _dbQuery;
            if (includes != null) {
                set = WithIncludes(includes);
            }
            return await set.ToListAsync();
        }
        public async Task<IEnumerable<TQuery>> AllAsync() {
            return await AllAsync(null);
        }

        public virtual IEnumerable<TQuery> All() {
            return AllAsync().Result;
        }
        public IEnumerable<TQuery> All(IRepositoryInclude<TQuery> includes) {
            return AllAsync(includes).Result;
        }

        private static readonly IDictionary<(System.ComponentModel.ListSortDirection direction, bool isFirst), string> sortCallSelection = new Dictionary<(System.ComponentModel.ListSortDirection, bool), string>(){
            {(System.ComponentModel.ListSortDirection.Ascending ,true ),"OrderBy"},
            {(System.ComponentModel.ListSortDirection.Descending ,true ),"OrderByDescending"},
            {(System.ComponentModel.ListSortDirection.Ascending ,false ),"ThenBy"},
            {(System.ComponentModel.ListSortDirection.Descending ,false ),"ThenByDescending"}
        };

        private static void DynamicOrderBy(ref IQueryable<TQuery> query, Expression<Func<TQuery, object>> expression, ListSortDirection direction, bool isFirst) {
            string sortCall = sortCallSelection[(direction, isFirst)];

            var resultExpression = Expression.Call(typeof(Queryable), sortCall, new Type[] { typeof(TQuery), typeof(object) },
                                                  query.Expression, Expression.Quote(expression));

            query = query.Provider.CreateQuery<TQuery>(resultExpression);
        }


        /// <summary>
        /// Retrieve Entities with a filter
        /// </summary>
        public virtual (IEnumerable<TQuery> Data, int Total) GetWithFilter(
                (IRepositoryFilter<TQuery> filter, IRepositorySort<TQuery> sorts) specification,
                (int Page, int PageSize)? paging = null) {
            return GetWithFilter(specification.filter, specification.sorts, paging);
        }

        /// <summary>
        /// Retrieve Entities with a filter
        /// </summary>
        public virtual (IEnumerable<TQuery> Data, int Total) GetWithFilter(
                IRepositoryFilter<TQuery> filter,
                IRepositorySort<TQuery> sorts = null,
                (int Page, int PageSize)? paging = null) {
            var queryWithIncludes = WithIncludes(filter);
            // IQueryable including all expression includes
            // var queryWithIncludes = filter?.Includes?.Aggregate(_dbSet.AsQueryable(),
            //         (current, include) => current.Include(include)) ?? _dbSet.AsQueryable();

            // // query including string includes
            // queryWithIncludes = filter?.IncludeStrings?.Aggregate(queryWithIncludes,
            //       (current, include) => current.Include(include)) ?? queryWithIncludes;

            // return the result filtered by Criteria
            IQueryable<TQuery> queryWithCriteria;
            if (filter?.Criteria != null) {
                queryWithCriteria = queryWithIncludes.Where(filter.Criteria);
            } else {
                queryWithCriteria = queryWithIncludes;
            }

            // sort result by sort
            if (sorts != null) {
                bool isFirstSort = true;
                foreach (var (Direction, KeySelector) in sorts.Sorts) {
                    DynamicOrderBy(ref queryWithCriteria, KeySelector, Direction, isFirstSort);
                    isFirstSort = false;
                }
            }

            var itemsTotal = queryWithCriteria.Count();

            if (paging.HasValue) {
                queryWithCriteria = queryWithCriteria.Skip(paging.Value.PageSize * (paging.Value.Page - 1))
                                                     .Take(paging.Value.PageSize);

            }
            return (queryWithCriteria, itemsTotal);
        }

        /// <summary>
        /// Retrieve Entities with a filter
        /// </summary>
        public async Task<(IEnumerable<TQuery> Data, int Total)> GetWithFilterAsync(
                (IRepositoryFilter<TQuery> filter, IRepositorySort<TQuery> sorts) specification,
                (int Page, int PageSize)? paging = null) {
            return await GetWithFilterAsync(specification.filter, specification.sorts, paging);
        }

        public async Task<(IEnumerable<TQuery> Data, int Total)> GetWithFilterAsync(
                IRepositoryFilter<TQuery> filter,
                IRepositorySort<TQuery> sorts = null,
                (int Page, int PageSize)? paging = null) {

            if (filter == null) throw new ArgumentNullException(nameof(filter));

            // expression includes
            IQueryable<TQuery> queryWithIncludes = WithIncludes(filter);

            // return the result filtered by Criteria
            IQueryable<TQuery> queryWithCriteria;
            if (filter?.Criteria != null) {
                queryWithCriteria = queryWithIncludes.Where(filter.Criteria);
            } else {
                queryWithCriteria = queryWithIncludes;
            }

            // sort result by sort
            if (sorts != null) {
                bool isFirstSort = true;
                foreach (var (Direction, KeySelector) in sorts.Sorts) {
                    DynamicOrderBy(ref queryWithCriteria, KeySelector, Direction, isFirstSort);
                    isFirstSort = false;
                }
            }

            var itemsTotal = await queryWithCriteria.CountAsync();

            if (paging.HasValue) {
                queryWithCriteria = queryWithCriteria.Skip(paging.Value.PageSize * (paging.Value.Page - 1))
                                                     .Take(paging.Value.PageSize);

            }
            return (queryWithCriteria, itemsTotal);
        }
    }
}