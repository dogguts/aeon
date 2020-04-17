using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// Defines the basic functionality for a asynchronous readonly repository
    /// </summary>
    /// <typeparam name="TEntity">The entity type this asynchronous readonly repository handles</typeparam>
    public interface IReadonlyRepositoryAsync<TEntity> where TEntity : class {

        /// <summary>
        /// Asynchronously gets an entity with the given primary key values.
        /// Also loads entity or entities referenced by navigation properties as defined in includes. 
        /// </summary>
        /// <param name="includes">Navigation properties to load</param>
        /// <param name="keyValues">The values of the primary key for the entity to get.</param>
        /// <returns>The entity found, or null.</returns>
        Task<TEntity> GetAsync(IRepositoryInclude<TEntity> includes, params object[] keyValues);

        /// <summary>
        /// Asynchronously gets an entity with the given primary key values.
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to get.</param>
        /// <returns>The entity found, or null.</returns>
        Task<TEntity> GetAsync(params object[] keyValues);

        /// <summary>
        /// Asynchronously get all entities of type TEntity. 
        /// Also loads entity or entities referenced by navigation properties as defined in includes. 
        /// </summary>
        /// <param name="includes">Navigation properties to load</param>
        /// <returns>A task that represents the asynchronous fetch operation. An Enumerable of all entities of type TEntity.</returns>
        Task<IEnumerable<TEntity>> AllAsync(IRepositoryInclude<TEntity> includes);

        /// <summary>
        /// Asynchronously get all entities of type TEntity. 
        /// </summary>
        /// <returns>All entities of type TEntity.</returns>
        Task<IEnumerable<TEntity>> AllAsync();

        /// <summary>
        /// Asynchronously get Entities by filter
        /// </summary>
        /// <param name="filter">Filter specification</param>
        /// <param name="sorts">Optional sort specification</param>
        /// <param name="paging">Optional The current Page and pageSize for paging</param>
        /// <returns>An Enumerable of entities of type TEntity matching filter and sorted by sorts, paged by paging.</returns>
        Task<(IEnumerable<TEntity> Data, int Total)> GetWithFilterAsync(
             IRepositoryFilter<TEntity> filter,
             IRepositorySort<TEntity> sorts = null,
             (int Page, int PageSize)? paging = null);

        /// <summary>
        /// Obsolete! Asynchronously get Entities by filter
        /// </summary>
        /// <param name="specification">Filter and Sort specification</param>
        /// <param name="paging">Optional The current Page and pageSize for paging</param>
        /// <returns>
        /// Data: results matching the filter and sorted by sorts, paged by paging.
        /// Total: total matches the filter produced (regardless of paging)
        /// </returns>
        [Obsolete("Use GetWithFilterAsync(IRepositoryFilter<TEntity>, IRepositorySort<TEntity>, (int Page, int PageSize)?)")]
        Task<(IEnumerable<TEntity> Data, int Total)> GetWithFilterAsync(
             (IRepositoryFilter<TEntity> filter, IRepositorySort<TEntity> sorts) specification,
             (int Page, int PageSize)? paging = null);
    }
}