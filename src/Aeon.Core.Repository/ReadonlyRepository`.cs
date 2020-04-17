using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public class ReadonlyRepository<TEntity, TDbContext> : IReadonlyRepository<TEntity>
            where TEntity : class
            where TDbContext : DbContext {

        protected readonly TDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public ReadonlyRepository(TDbContext context) {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }


        private IQueryable<TEntity> WithIncludes(IRepositoryInclude<TEntity> includes) {
            var queryWithIncludes = _dbSet.AsQueryable();
            if (includes != null) {
                //   including all expression includes
                queryWithIncludes = includes?.IncludePaths?.Aggregate(queryWithIncludes,
                        (current, include) => current.Include(include)) ?? queryWithIncludes;
            }
            return queryWithIncludes;
        }

        public async Task<TEntity> GetAsync(params object[] keyValues) {
            return await GetAsync(null, keyValues);
        }

        public virtual TEntity Get(params object[] keyValues) {
            return GetAsync(keyValues).Result;
        }


        public async Task<TEntity> GetAsync(IRepositoryInclude<TEntity> includes, params object[] keyValues) {
            var entityType = _context.Model.FindEntityType(typeof(TEntity));
            var primaryKeyProperties = entityType.FindPrimaryKey()?.Properties;
            if (entityType.FindPrimaryKey() == null) {
                throw new InvalidOperationException("Retrieving Keyless entity type by key value");
                //NOTE: or?: return await Task.FromException<TEntity>(new InvalidOperationException(... 
            }
            if (primaryKeyProperties == null) {
                //NOTE: or?: return await Task.FromException<TEntity>(new InvalidOperationException(... 
                throw new InvalidOperationException($"The entity type '{typeof(TEntity).Name}' requires a primary key to be defined.'");
            }

            BinaryExpression predicate = null;
            var entityParameter = Expression.Parameter(typeof(TEntity), "p");

            for (var i = 0; i < primaryKeyProperties.Count; i++) {
                var property = primaryKeyProperties[i];

                var leftExpr = Expression.Property(entityParameter, property.Name);

                object keyValuePart = keyValues[i];
                Expression<Func<object>> closure = () => keyValuePart;
                var rightExpr = Expression.Convert(closure.Body, leftExpr.Type);

                var equalsExpression = Expression.Equal(leftExpr, rightExpr);


                predicate = predicate == null ? equalsExpression : Expression.AndAlso(predicate, equalsExpression);
            }
            var criteria = Expression.Lambda<Func<TEntity, bool>>(predicate, entityParameter);

            var filter = new RepositoryFilter<TEntity>(criteria, includes);
            var (Data, _) = await GetWithFilterAsync(filter);

            return Data.FirstOrDefault();
        }

        public virtual TEntity Get(IRepositoryInclude<TEntity> includes, params object[] keyValues) {
            return GetAsync(includes, keyValues).Result;
        }
        public async Task<IEnumerable<TEntity>> AllAsync(IRepositoryInclude<TEntity> includes) {
            IQueryable<TEntity> set = _dbSet;
            if (includes != null) {
                set = WithIncludes(includes);
            }
            return await set.ToListAsync();
        }
        public async Task<IEnumerable<TEntity>> AllAsync() {
            return await AllAsync(null);
        }

        public virtual IEnumerable<TEntity> All() {
            return AllAsync().Result;
        }
        public IEnumerable<TEntity> All(IRepositoryInclude<TEntity> includes) {
            return AllAsync(includes).Result;
        }

        private static readonly IDictionary<(System.ComponentModel.ListSortDirection direction, bool isFirst), string> sortCallSelection = new Dictionary<(System.ComponentModel.ListSortDirection, bool), string>(){
            {(System.ComponentModel.ListSortDirection.Ascending ,true ),"OrderBy"},
            {(System.ComponentModel.ListSortDirection.Descending ,true ),"OrderByDescending"},
            {(System.ComponentModel.ListSortDirection.Ascending ,false ),"ThenBy"},
            {(System.ComponentModel.ListSortDirection.Descending ,false ),"ThenByDescending"}
        };

        private static void DynamicOrderBy(ref IQueryable<TEntity> query, Expression<Func<TEntity, object>> expression, ListSortDirection direction, bool isFirst) {
            string sortCall = sortCallSelection[(direction, isFirst)];

            var resultExpression = Expression.Call(typeof(Queryable), sortCall, new Type[] { typeof(TEntity), typeof(object) },
                                                  query.Expression, Expression.Quote(expression));

            query = query.Provider.CreateQuery<TEntity>(resultExpression);
        }


        /// <summary>
        /// Retrieve Entities with a filter
        /// </summary>
        public virtual (IEnumerable<TEntity> Data, int Total) GetWithFilter(
                (IRepositoryFilter<TEntity> filter, IRepositorySort<TEntity> sorts) specification,
                (int Page, int PageSize)? paging = null) {
            return GetWithFilter(specification.filter, specification.sorts, paging);
        }

        /// <summary>
        /// Retrieve Entities with a filter
        /// </summary>
        public virtual (IEnumerable<TEntity> Data, int Total) GetWithFilter(
                IRepositoryFilter<TEntity> filter,
                IRepositorySort<TEntity> sorts = null,
                (int Page, int PageSize)? paging = null) {
            return GetWithFilterAsync(filter, sorts, paging).Result;
            //var queryWithIncludes = WithIncludes(filter);

            //// return the result filtered by Criteria
            //IQueryable<TEntity> queryWithCriteria;
            //if (filter?.Criteria != null) {
            //    queryWithCriteria = queryWithIncludes.Where(filter.Criteria);
            //} else {
            //    queryWithCriteria = queryWithIncludes;
            //}

            //// sort result by sort
            //if (sorts != null) {
            //    bool isFirstSort = true;
            //    foreach (var (Direction, KeySelector) in sorts.Sorts) {
            //        DynamicOrderBy(ref queryWithCriteria, KeySelector, Direction, isFirstSort);
            //        isFirstSort = false;
            //    }
            //}

            //var itemsTotal = queryWithCriteria.Count();

            //if (paging.HasValue) {
            //    if (paging.HasValue ? paging.Value.Page < 1 : false) {
            //        throw new ArgumentOutOfRangeException(nameof(paging), $"{nameof(paging.Value.Page)} < 1");
            //    }
            //    queryWithCriteria = queryWithCriteria.Skip(paging.Value.PageSize * (paging.Value.Page - 1))
            //                                         .Take(paging.Value.PageSize);

            //}
            //return (queryWithCriteria, itemsTotal);
        }

        /// <summary>
        /// Retrieve Entities with a filter
        /// </summary>
        public async Task<(IEnumerable<TEntity> Data, int Total)> GetWithFilterAsync(
                (IRepositoryFilter<TEntity> filter, IRepositorySort<TEntity> sorts) specification,
                (int Page, int PageSize)? paging = null) {
            return await GetWithFilterAsync(specification.filter, specification.sorts, paging);
        }

        public async Task<(IEnumerable<TEntity> Data, int Total)> GetWithFilterAsync(
                IRepositoryFilter<TEntity> filter,
                IRepositorySort<TEntity> sorts = null,
                (int Page, int PageSize)? paging = null) {

            // expression includes
            IQueryable<TEntity> queryWithIncludes = WithIncludes(filter);

            // return the result filtered by Criteria
            IQueryable<TEntity> queryWithCriteria;
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
                if (paging.HasValue ? paging.Value.Page < 1 : false) {
                    throw new ArgumentOutOfRangeException(nameof(paging), $"{nameof(paging.Value.Page)} < 1 (paging is one-based)");
                }
                queryWithCriteria = queryWithCriteria.Skip(paging.Value.PageSize * (paging.Value.Page - 1))
                                                     .Take(paging.Value.PageSize);

            }

            return (queryWithCriteria, itemsTotal);
        }
    }
}