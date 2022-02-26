using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public interface IReadonlyRepository<TEntity, TDbContext> : IReadonlyRepository<TEntity>
            where TEntity : class
            where TDbContext : DbContext { }

    public class ReadonlyRepository<TEntity, TDbContext> : IReadonlyRepository<TEntity, TDbContext>
            where TEntity : class
            where TDbContext : DbContext {

        protected readonly TDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        protected static readonly IDictionary<(ListSortDirection direction, bool isFirst), string> sortCallSelection = new Dictionary<(ListSortDirection, bool), string>(){
            {(ListSortDirection.Ascending ,true ),"OrderBy"},
            {(ListSortDirection.Descending ,true ),"OrderByDescending"},
            {(ListSortDirection.Ascending ,false ),"ThenBy"},
            {(ListSortDirection.Descending ,false ),"ThenByDescending"}
        };

        /// <inheritdoc/>
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
        // ****** Get ******
        /// <inheritdoc/>
        public TEntity Get(params object[] keyValues) => GetAsync(keyValues).Result;

        /// <inheritdoc/>
        public TEntity Get(IRepositoryInclude<TEntity> includes, params object[] keyValues) {
            return GetAsync(includes, keyValues).Result;
        }

        /// <inheritdoc/>
        public async Task<TEntity> GetAsync(params object[] keyValues) => await GetAsync(null, keyValues);

        /// <inheritdoc/>
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

        // ****** All ******
        /// <inheritdoc/>
        public IEnumerable<TEntity> All() => AllAsync().Result;

        /// <inheritdoc/>
        public IEnumerable<TEntity> All(IRepositoryInclude<TEntity> includes) {
            return AllAsync(includes).Result;
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> AllAsync(IRepositoryInclude<TEntity> includes) {
            return (await GetWithFilterAsync(new RepositoryFilter<TEntity>(null, includes))).Data;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TEntity>> AllAsync() =>  await AllAsync(null);

        private static void DynamicOrderBy(ref IQueryable<TEntity> query, Expression<Func<TEntity, object>> expression, ListSortDirection direction, bool isFirst) {
            string sortCall = sortCallSelection[(direction, isFirst)];

            var resultExpression = Expression.Call(typeof(Queryable), sortCall, new Type[] { typeof(TEntity), typeof(object) },
                                                  query.Expression, Expression.Quote(expression));

            query = query.Provider.CreateQuery<TEntity>(resultExpression);
        }

        // ****** GetWithFilter ******

        /// <inheritdoc/>
        [Obsolete("Use GetWithFilter(IRepositoryFilter<TEntity>, IRepositorySort<TEntity>, (int Page, int PageSize)?)")]
        public (IEnumerable<TEntity> Data, int Total) GetWithFilter(
                (IRepositoryFilter<TEntity> filter, IRepositorySort<TEntity> sorts) specification,
                (int Page, int PageSize)? paging = null) => GetWithFilter(specification.filter, specification.sorts, paging);


        /// <inheritdoc/>
        public (IEnumerable<TEntity> Data, int Total) GetWithFilter(
                IRepositoryFilter<TEntity> filter,
                IRepositorySort<TEntity> sorts = null,
                (int Page, int PageSize)? paging = null) => GetWithFilterAsync(filter, sorts, paging).Result;

        /// <inheritdoc/>
        [Obsolete("Use GetWithFilterAsync(IRepositoryFilter<TEntity>, IRepositorySort<TEntity>, (int Page, int PageSize)?)")]
        public async Task<(IEnumerable<TEntity> Data, int Total)> GetWithFilterAsync(
                (IRepositoryFilter<TEntity> filter, IRepositorySort<TEntity> sorts) specification,
                (int Page, int PageSize)? paging = null) => await GetWithFilterAsync(specification.filter, specification.sorts, paging);

        /// <inheritdoc/>
        public virtual async Task<(IEnumerable<TEntity> Data, int Total)> GetWithFilterAsync(
                IRepositoryFilter<TEntity> filter,
                IRepositorySort<TEntity> sorts = null,
                (int Page, int PageSize)? paging = null) {

            // expression includes
            IQueryable<TEntity> queryWithIncludes = WithIncludes(filter);

            // add tags
            foreach (string tag in filter?.Tags?? Array.Empty<string>()) {
                queryWithIncludes = queryWithIncludes.TagWith(tag);
            }

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
                if (paging.HasValue && paging.Value.Page < 1) {
                    throw new ArgumentOutOfRangeException(nameof(paging), $"{nameof(paging.Value.Page)} < 1 (paging is one-based)");
                }
                queryWithCriteria = queryWithCriteria.Skip(paging.Value.PageSize * (paging.Value.Page - 1))
                                                     .Take(paging.Value.PageSize);

            }

            return (queryWithCriteria, itemsTotal);
        }
    }
}