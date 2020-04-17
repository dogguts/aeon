using System;
using System.Collections.Generic;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// Defines the basic functionality for a synchronous readonly repository
    /// </summary>
    /// <typeparam name="TEntity">The entity type this synchronous readonly repository handles</typeparam>
    public interface IReadonlyRepository<TEntity> : IReadonlyRepositoryAsync<TEntity> where TEntity : class {

        ///<summary>
        /// Gets an entity with the given primary key values. 
        /// </summary>
        ///<param name="keyValues">The values of the primary key for the entity to get.</param>
        ///<returns>The entity found, or null.</returns>
        TEntity Get(params object[] keyValues);

        /// <summary>
        /// Gets an entity with the given primary key values.  Also loads entity or entities referenced by navigation properties as defined in includes. 
        /// </summary>
        /// <param name="includes">Navigation properties to load</param>
        /// <param name="keyValues">The values of the primary key for the entity to get.</param>
        /// <returns>The entity found, or null.</returns>
        TEntity Get(IRepositoryInclude<TEntity> includes, params object[] keyValues);



        /// <summary>
        /// Get all entities of type TEntity. 
        /// </summary>
        /// <returns>An Enumerable of all entities of type TEntity.</returns>
        IEnumerable<TEntity> All();
        /// <summary>
        /// Get all entities of type TEntity. Also loads entity or entities referenced by navigation properties as defined in includes. 
        /// </summary>
        /// <param name="includes">Navigation properties to load</param>
        /// <returns>An Enumerable of all entities of TEntity.</returns>
        IEnumerable<TEntity> All(IRepositoryInclude<TEntity> includes);


        /// <summary>
        /// Get Entities by filter
        /// </summary>
        /// <param name="filter">Filter specification</param>
        /// <param name="sorts">Optional sort specification</param>
        /// <param name="paging">Optional The current Page (one-based) and pageSize for paging</param>
        /// <returns>
        /// Data: results matching the filter and sorted by sorts, paged by paging.
        /// Total: total matches the filter produced (regardless of paging)
        /// </returns>
        (IEnumerable<TEntity> Data, int Total) GetWithFilter(
            IRepositoryFilter<TEntity> filter,
            IRepositorySort<TEntity> sorts = null,
            (int Page, int PageSize)? paging = null);

        /// <summary>
        /// Obsolete! Get Entities by filter
        /// </summary>
        /// <param name="specification">Filter and Sort specification</param>
        /// <param name="paging">Optional The current Page and pageSize for paging</param>
        /// <returns>
        /// Data: results matching the filter and sorted by sorts, paged by paging.
        /// Total: total matches the filter produced (regardless of paging)
        /// </returns>
        [Obsolete("Use GetWithFilter(IRepositoryFilter<TEntity>, IRepositorySort<TEntity>, (int Page, int PageSize)?)")]
        (IEnumerable<TEntity> Data, int Total) GetWithFilter(
            (IRepositoryFilter<TEntity> filter, IRepositorySort<TEntity> sorts) specification,
            (int Page, int PageSize)? paging = null);
    }
}