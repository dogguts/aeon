using System.Collections.Generic;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// Defines the basic functionality for a synchronous readonly repository
    /// </summary>
    /// <typeparam name="T">The entity type this synchronous readonly repository handles</typeparam>
    public interface IReadonlyRepository<T> where T : class {

        /// <summary>
        /// Gets an entity with the given primary key values. 
        /// </summary>
        /// <param name="keyValues">The values of the primary key for the entity to get.</param>
        /// <returns>The entity found, or null.</returns>
        T Get(params object[] keyValues);

        /// <summary>
        /// Gets an entity with the given primary key values.  Also loads entity or entities referenced by navigation properties as defined in includes. 
        /// </summary>
        /// <param name="includes">Navigation properties to load</param>
        /// <param name="keyValues">The values of the primary key for the entity to get.</param>
        /// <returns>The entity found, or null.</returns>
        T Get(IRepositoryInclude<T> includes, params object[] keyValues);



        /// <summary>
        /// Get all entities of type T. 
        /// </summary>
        /// <returns>An Enumerable of all entities of type T.</returns>
        IEnumerable<T> All();
        /// <summary>
        /// Get all entities of type T. Also loads entity or entities referenced by navigation properties as defined in includes. 
        /// </summary>
        /// <param name="includes">Navigation properties to load</param>
        /// <returns>An Enumerable of all entities of type T.</returns>
        IEnumerable<T> All(IRepositoryInclude<T> includes);


        /// <summary>
        /// Get Entities by filter
        /// </summary>
        /// <param name="filter">Filter specification</param>
        /// <param name="sorts">Optional sort specification</param>
        /// <param name="paging">Optional The current Page and pageSize for paging</param>
        /// <returns>An Enumerable of entities of type T matching filter and sorted by sorts, paged by paging.</returns>
        (IEnumerable<T> Data, int Total) GetWithFilter(
            IRepositoryFilter<T> filter,
            IRepositorySort<T> sorts = null,
            (int Page, int PageSize)? paging = null);

        /// <summary>
        /// Get Entities by filter
        /// </summary>
        /// <param name="specification">Filter and Sort specification</param>
        /// <param name="paging">Optional The current Page and pageSize for paging</param>
        /// <returns>An Enumerable of entities of type T matching filter and sorted by sorts, paged by paging.</returns>
        (IEnumerable<T> Data, int Total) GetWithFilter(
            (IRepositoryFilter<T> filter, IRepositorySort<T> sorts) specification,
            (int Page, int PageSize)? paging = null);
    }
}