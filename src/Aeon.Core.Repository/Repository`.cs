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
    public class Repository<TEntity, TDbContext> : ReadonlyRepository<TEntity, TDbContext>, IRepository<TEntity>
            where TEntity : class
            where TDbContext : DbContext {

        private DbSet<TEntity> DbSet => (DbSet<TEntity>)_dbQuery;

        public Repository(TDbContext context) : base(context, true) {
            //_context = context;
            base._dbQuery = context.Set<TEntity>();
        }

        /// <summary>
        /// Adds the given entity to the context underlying the set in the Added state.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual EntityEntry<TEntity> Add(TEntity entity) {
            return DbSet.Add(entity);
        }

        // private IQueryable<TEntity> WithIncludes(IRepositoryInclude<TEntity> includes) {
        //     var queryWithIncludes = _dbQuery.AsQueryable();
        //     if (includes != null) {
        //         //   including all expression includes
        //         queryWithIncludes = includes?.IncludePaths?.Aggregate(queryWithIncludes,
        //                 (current, include) => current.Include(include)) ?? queryWithIncludes;
        //     }
        //     return queryWithIncludes;
        // }


        // public async Task<TEntity> GetAsync(params object[] keyValues) {
        //     return await GetAsync(null, keyValues);
        // }

        // public virtual TEntity Get(params object[] keyValues) {
        //     return GetAsync(keyValues).Result;
        // }

        // public async Task<TEntity> GetAsync(IRepositoryInclude<TEntity> includes, params object[] keyValues) {

        //     var primaryKeyProperties = _context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties;

        //     var entityParameter = Expression.Parameter(typeof(TEntity), "p");

        //     BinaryExpression predicate = null;

        //     for (var i = 0; i < primaryKeyProperties.Count; i++) {
        //         var property = primaryKeyProperties[i];


        //         var leftExpr = Expression.Property(entityParameter, property.Name);

        //         var rightExpr = Expression.Convert(Expression.Constant(keyValues[i]), property.ClrType);

        //         var equalsExpression =
        //                             Expression.Equal(
        //                                 leftExpr,
        //                                 rightExpr);

        //         predicate = predicate == null ? equalsExpression : Expression.AndAlso(predicate, equalsExpression);
        //     }
        //     var criteria = Expression.Lambda<Func<TEntity, bool>>(predicate, entityParameter);

        //     var filter = new RepositoryFilter<TEntity>(criteria, includes);
        //     var (Data, _) = await GetWithFilterAsync(filter);

        //     return Data.FirstOrDefault();
        // }

        // public virtual TEntity Get(IRepositoryInclude<TEntity> includes, params object[] keyValues) {
        //     return GetAsync(includes, keyValues).Result;
        // }
        // public async Task<IEnumerable<TEntity>> AllAsync(IRepositoryInclude<TEntity> includes) {
        //     IQueryable<TEntity> set = _dbQuery;
        //     if (includes != null) {
        //         set = WithIncludes(includes);
        //     }
        //     return await set.ToListAsync();
        // }
        // public async Task<IEnumerable<TEntity>> AllAsync() {
        //     return await AllAsync(null);
        // }

        // public virtual IEnumerable<TEntity> All() {
        //     return AllAsync().Result;
        // }
        // public IEnumerable<TEntity> All(IRepositoryInclude<TEntity> includes) {
        //     return AllAsync(includes).Result;
        // }

        // private static readonly IDictionary<(System.ComponentModel.ListSortDirection direction, bool isFirst), string> sortCallSelection = new Dictionary<(System.ComponentModel.ListSortDirection, bool), string>(){
        //     {(System.ComponentModel.ListSortDirection.Ascending ,true ),"OrderBy"},
        //     {(System.ComponentModel.ListSortDirection.Descending ,true ),"OrderByDescending"},
        //     {(System.ComponentModel.ListSortDirection.Ascending ,false ),"ThenBy"},
        //     {(System.ComponentModel.ListSortDirection.Descending ,false ),"ThenByDescending"}
        // };

        // private static void DynamicOrderBy(ref IQueryable<TEntity> query, Expression<Func<TEntity, object>> expression, ListSortDirection direction, bool isFirst) {
        //     string sortCall = sortCallSelection[(direction, isFirst)];

        //     var resultExpression = Expression.Call(typeof(Queryable), sortCall, new Type[] { typeof(TEntity), typeof(object) },
        //                                           query.Expression, Expression.Quote(expression));

        //     query = query.Provider.CreateQuery<TEntity>(resultExpression);
        // }


        // /// <summary>
        // /// Retrieve Entities with a filter
        // /// </summary>
        // public virtual (IEnumerable<TEntity> Data, int Total) GetWithFilter(
        //         (IRepositoryFilter<TEntity> filter, IRepositorySort<TEntity> sorts) specification,
        //         (int Page, int PageSize)? paging = null) {
        //     return GetWithFilter(specification.filter, specification.sorts, paging);
        // }

        // /// <summary>
        // /// Retrieve Entities with a filter
        // /// </summary>
        // public virtual (IEnumerable<TEntity> Data, int Total) GetWithFilter(
        //         IRepositoryFilter<TEntity> filter,
        //         IRepositorySort<TEntity> sorts = null,
        //         (int Page, int PageSize)? paging = null) {
        //     var queryWithIncludes = WithIncludes(filter);
        //     // IQueryable including all expression includes
        //     // var queryWithIncludes = filter?.Includes?.Aggregate(_dbSet.AsQueryable(),
        //     //         (current, include) => current.Include(include)) ?? _dbSet.AsQueryable();

        //     // // query including string includes
        //     // queryWithIncludes = filter?.IncludeStrings?.Aggregate(queryWithIncludes,
        //     //       (current, include) => current.Include(include)) ?? queryWithIncludes;

        //     // return the result filtered by Criteria
        //     IQueryable<TEntity> queryWithCriteria;
        //     if (filter?.Criteria != null) {
        //         queryWithCriteria = queryWithIncludes.Where(filter.Criteria);
        //     } else {
        //         queryWithCriteria = queryWithIncludes;
        //     }

        //     // sort result by sort
        //     if (sorts != null) {
        //         bool isFirstSort = true;
        //         foreach (var (Direction, KeySelector) in sorts.Sorts) {
        //             DynamicOrderBy(ref queryWithCriteria, KeySelector, Direction, isFirstSort);
        //             isFirstSort = false;
        //         }
        //     }

        //     var itemsTotal = queryWithCriteria.Count();

        //     if (paging.HasValue) {
        //         queryWithCriteria = queryWithCriteria.Skip(paging.Value.PageSize * (paging.Value.Page - 1))
        //                                              .Take(paging.Value.PageSize);

        //     }
        //     return (queryWithCriteria, itemsTotal);
        // }

        // /// <summary>
        // /// Retrieve Entities with a filter
        // /// </summary>
        // public async Task<(IEnumerable<TEntity> Data, int Total)> GetWithFilterAsync(
        //         (IRepositoryFilter<TEntity> filter, IRepositorySort<TEntity> sorts) specification,
        //         (int Page, int PageSize)? paging = null) {
        //     return await GetWithFilterAsync(specification.filter, specification.sorts, paging);
        // }

        // public async Task<(IEnumerable<TEntity> Data, int Total)> GetWithFilterAsync(
        //         IRepositoryFilter<TEntity> filter,
        //         IRepositorySort<TEntity> sorts = null,
        //         (int Page, int PageSize)? paging = null) {

        //     if (filter == null) throw new ArgumentNullException(nameof(filter));

        //     // expression includes
        //     IQueryable<TEntity> queryWithIncludes = WithIncludes(filter);

        //     // return the result filtered by Criteria
        //     IQueryable<TEntity> queryWithCriteria;
        //     if (filter?.Criteria != null) {
        //         queryWithCriteria = queryWithIncludes.Where(filter.Criteria);
        //     } else {
        //         queryWithCriteria = queryWithIncludes;
        //     }

        //     // sort result by sort
        //     if (sorts != null) {
        //         bool isFirstSort = true;
        //         foreach (var (Direction, KeySelector) in sorts.Sorts) {
        //             DynamicOrderBy(ref queryWithCriteria, KeySelector, Direction, isFirstSort);
        //             isFirstSort = false;
        //         }
        //     }

        //     var itemsTotal = await queryWithCriteria.CountAsync();

        //     if (paging.HasValue) {
        //         queryWithCriteria = queryWithCriteria.Skip(paging.Value.PageSize * (paging.Value.Page - 1))
        //                                              .Take(paging.Value.PageSize);

        //     }
        //     return (queryWithCriteria, itemsTotal);
        // }

        private TEntity GetExisting(TEntity entity) {
            var primaryKey = _context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey();
            var primaryKeyValues = primaryKey.Properties.Select(p => typeof(TEntity).GetProperty(p.Name).GetValue(entity, null)).ToArray();

            return DbSet.Find(primaryKeyValues);
        }

        public virtual void Update(TEntity entity) {
            var existing = GetExisting(entity);
            if (existing != null) {
                _context.Entry(existing).CurrentValues.SetValues(entity);
            } else {
                //TODO: (Repository) custom exceptions
                var primaryKey = _context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey();
                var primaryKeyKeyValues = primaryKey.Properties.ToDictionary(p => p.Name, p => typeof(TEntity).GetProperty(p.Name).GetValue(entity, null));
                var primaryKeyString = string.Join(",", primaryKeyKeyValues.Select(kvp => $"{kvp.Key}={kvp.Value}"));

                throw new ArgumentException($"Object of type {typeof(TEntity)} with PrimaryKey {primaryKeyString} was not found");
            }
        }

        public virtual void Delete(TEntity entity) {
            var existing = GetExisting(entity);
            if (existing != null) {
                _context.Entry(existing).State = EntityState.Deleted;
            }
        }


    }
}