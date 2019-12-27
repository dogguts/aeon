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

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public class Repository<T, TDbContext> : IRepository<T>
            where T : class
            where TDbContext : DbContext {

        protected readonly TDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(TDbContext context) {
            _context = context;
            _dbSet = context.Set<T>();

        }

        /// <summary>
        /// Adds the given entity to the context underlying the set in the Added state.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual EntityEntry<T> Add(T entity) {
            return _dbSet.Add(entity);
        }

        private IQueryable<T> WithIncludes(IRepositoryInclude<T> includes) {
            var queryWithIncludes = _dbSet.AsQueryable();
            if (includes != null) {
                //   including all expression includes
                queryWithIncludes = includes?.IncludePaths?.Aggregate(queryWithIncludes,
                        (current, include) => current.Include(include)) ?? queryWithIncludes;
            }
            // // query including string includes
            // queryWithIncludes = includes?.IncludeStrings?.Aggregate(queryWithIncludes,
            //       (current, include) => current.Include(include)) ?? queryWithIncludes;

            return queryWithIncludes;
        }


        public async Task<T> GetAsync(params object[] keyValues) {
            return await GetAsync(null, keyValues);
        }

        public virtual T Get(params object[] keyValues) {
            return GetAsync(keyValues).Result;
        }

        /* */
        internal static readonly MethodInfo PropertyMethod
                  = typeof(EF).GetTypeInfo().GetDeclaredMethod(nameof(Property));
        public static TProperty Property<TProperty>(
                 object entity,
                     string propertyName)
                   => throw new InvalidOperationException(CoreStrings.PropertyMethodInvoked);

        static readonly MethodInfo GetValueMethod
          = typeof(ValueBuffer).GetRuntimeProperties().Single(p => p.GetIndexParameters().Length > 0).GetMethod;
        /* */
        public async Task<T> GetAsync(IRepositoryInclude<T> includes, params object[] keyValues) {

            var primaryKeyProperties = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties;

            //  if (keyProperties.Count != keyValues.Length) TODO: throw exception


            var entityParameter = Expression.Parameter(typeof(T), "p");

            BinaryExpression predicate = null;

            for (var i = 0; i < primaryKeyProperties.Count; i++) {
                var property = primaryKeyProperties[i];


                var leftExpr = Expression.Property(entityParameter, property.Name);

                object keyValuePart = keyValues[i];
                Expression<Func<object>> closure = () => keyValuePart;
                var rightExpr = Expression.Convert(closure.Body, leftExpr.Type);

                var equalsExpression = Expression.Equal(leftExpr, rightExpr);

                predicate = predicate == null ? equalsExpression : Expression.AndAlso(predicate, equalsExpression);
            }
            var criteria = Expression.Lambda<Func<T, bool>>(predicate, entityParameter);

            var filter = new RepositoryFilter<T>(criteria, includes);
            var (Data, Total) = await GetWithFilterAsync(filter);

            return Data.FirstOrDefault();
        }

        public virtual T Get(IRepositoryInclude<T> includes, params object[] keyValues) {
            return GetAsync(includes, keyValues).Result;
        }
        public async Task<IEnumerable<T>> AllAsync(IRepositoryInclude<T> includes) {
            IQueryable<T> set = _dbSet;
            if (includes != null) {
                set = WithIncludes(includes);
            }
            return await set.ToListAsync();
        }
        public async Task<IEnumerable<T>> AllAsync() {
            return await AllAsync(null);
        }

        public virtual IEnumerable<T> All() {
            return AllAsync().Result;
        }
        public IEnumerable<T> All(IRepositoryInclude<T> includes) {
            return AllAsync(includes).Result;
        }

        private static readonly IDictionary<(System.ComponentModel.ListSortDirection direction, bool isFirst), string> sortCallSelection = new Dictionary<(System.ComponentModel.ListSortDirection, bool), string>(){
            {(System.ComponentModel.ListSortDirection.Ascending ,true ),"OrderBy"},
            {(System.ComponentModel.ListSortDirection.Descending ,true ),"OrderByDescending"},
            {(System.ComponentModel.ListSortDirection.Ascending ,false ),"ThenBy"},
            {(System.ComponentModel.ListSortDirection.Descending ,false ),"ThenByDescending"}
        };

        private static void DynamicOrderBy(ref IQueryable<T> query, Expression<Func<T, object>> expression, ListSortDirection direction, bool isFirst) {
            string sortCall = sortCallSelection[(direction, isFirst)];

            var resultExpression = Expression.Call(typeof(Queryable), sortCall, new Type[] { typeof(T), typeof(object) },
                                                  query.Expression, Expression.Quote(expression));

            query = query.Provider.CreateQuery<T>(resultExpression);
        }


        /// <summary>
        /// Retrieve Entities with a filter
        /// </summary>
        public virtual (IEnumerable<T> Data, int Total) GetWithFilter(
                (IRepositoryFilter<T> filter, IRepositorySort<T> sorts) specification,
                (int Page, int PageSize)? paging = null) {
            return GetWithFilter(specification.filter, specification.sorts, paging);
        }

        /// <summary>
        /// Retrieve Entities with a filter
        /// </summary>
        public virtual (IEnumerable<T> Data, int Total) GetWithFilter(
                IRepositoryFilter<T> filter,
                IRepositorySort<T> sorts = null,
                (int Page, int PageSize)? paging = null) {
            var queryWithIncludes = WithIncludes(filter);
            // IQueryable including all expression includes
            // var queryWithIncludes = filter?.Includes?.Aggregate(_dbSet.AsQueryable(),
            //         (current, include) => current.Include(include)) ?? _dbSet.AsQueryable();

            // // query including string includes
            // queryWithIncludes = filter?.IncludeStrings?.Aggregate(queryWithIncludes,
            //       (current, include) => current.Include(include)) ?? queryWithIncludes;

            // return the result filtered by Criteria
            IQueryable<T> queryWithCriteria;
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
        public async Task<(IEnumerable<T> Data, int Total)> GetWithFilterAsync(
                (IRepositoryFilter<T> filter, IRepositorySort<T> sorts) specification,
                (int Page, int PageSize)? paging = null) {
            return await GetWithFilterAsync(specification.filter, specification.sorts, paging);
        }

        public async Task<(IEnumerable<T> Data, int Total)> GetWithFilterAsync(
                IRepositoryFilter<T> filter,
                IRepositorySort<T> sorts = null,
                (int Page, int PageSize)? paging = null) {

            if (filter == null) throw new ArgumentNullException(nameof(filter));

            // expression includes
            IQueryable<T> queryWithIncludes = WithIncludes(filter);

            // return the result filtered by Criteria
            IQueryable<T> queryWithCriteria;
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

        private T GetExisting(T entity) {
            var primaryKey = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey();
            var primaryKeyValues = primaryKey.Properties.Select(p => typeof(T).GetProperty(p.Name).GetValue(entity, null)).ToArray();

            return _dbSet.Find(primaryKeyValues);
        }

        public virtual void Update(T entity) {
            var existing = GetExisting(entity);
            if (existing != null) {
                _context.Entry(existing).CurrentValues.SetValues(entity);
            } else {
                //TODO: (Repository) custom exceptions
                var primaryKey = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey();
                var primaryKeyKeyValues = primaryKey.Properties.ToDictionary(p => p.Name, p => typeof(T).GetProperty(p.Name).GetValue(entity, null));
                var primaryKeyString = string.Join(",", primaryKeyKeyValues.Select(kvp => $"{kvp.Key}={kvp.Value}"));

                throw new ArgumentException($"Object of type {typeof(T)} with PrimaryKey {primaryKeyString} was not found");
            }
        }

        public virtual void Delete(T entity) {
            var existing = GetExisting(entity);
            if (existing != null) {
                _context.Entry(existing).State = EntityState.Deleted;
            }
        }


    }
}