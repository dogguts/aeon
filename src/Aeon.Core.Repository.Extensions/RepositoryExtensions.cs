using Aeon.Core.Repository.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aeon.Core.Repository {
    /// <summary>
    /// Utility extensions on Repositories
    /// </summary>
    public static class RepositoryExtensions {
        private static readonly (int Page, int PageSize) FirstPageFirstItem = (1, 1);

        private static TEntity GetEntityWithSelector<TEntity>(this IReadonlyRepository<TEntity> repository,
                                                 IRepositoryFilter<TEntity> filter,
                                                 Func<IEnumerable<TEntity>, TEntity> selector) where TEntity : class {
            return selector(repository.GetWithFilter(filter, paging: FirstPageFirstItem).Data);
        }
        private static async Task<TEntity> GetEntityWithSelectorAsync<TEntity>(this IReadonlyRepositoryAsync<TEntity> repository,
                                           IRepositoryFilter<TEntity> filter,
                                           Func<IEnumerable<TEntity>, TEntity> selector) where TEntity : class {
            return selector((await repository.GetWithFilterAsync(filter, paging: FirstPageFirstItem)).Data);
        }

        /* Single */
        /// <summary>
        /// Returns the only entity matched by the Filter, throws an exception when there is not exactly one entity found
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity.</typeparam>
        /// <param name="repository">A IReadonlyRepository to return the single entity of</param>
        /// <param name="filter">A filter to test an entity for a condition.</param>
        /// <returns>The single matched entity.</returns>
        public static TEntity GetSingle<TEntity>(this IReadonlyRepository<TEntity> repository, IRepositoryFilter<TEntity> filter) where TEntity : class {
            return repository.GetEntityWithSelector(filter, (a) => a.Single());
        }
        /// <summary>
        /// Returns the only entity matched by the Filter, throws an exception when there is not exactly one entity found
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity.</typeparam>
        /// <param name="repository">A IReadonlyRepository to return the single entity of</param>
        /// <param name="filter">A filter to test an entity for a condition.</param>
        /// <returns>The single matched entity.</returns>
        public static async Task<TEntity> GetSingleAsync<TEntity>(this IReadonlyRepositoryAsync<TEntity> repository, IRepositoryFilter<TEntity> filter) where TEntity : class {
            return await repository.GetEntityWithSelectorAsync(filter, (a) => a.Single());
        }

        /* SingleOrDefault*/
        /// <summary>
        /// Returns the only entity matched by the Filter or a default value if no entity was found.
        /// Throws an exception if more than one element was found
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity.</typeparam>
        /// <param name="repository">A IReadonlyRepository to return the single entity of</param>
        /// <param name="filter">A filter to test an entity for a condition.</param>
        /// <returns>The single matched entity,  or default(TEntity) if none found</returns>
        public static TEntity GetSingleOrDefault<TEntity>(this IReadonlyRepository<TEntity> repository, IRepositoryFilter<TEntity> filter) where TEntity : class {
            return repository.GetEntityWithSelector(filter, (a) => a.SingleOrDefault());
        }

        /// <summary>
        /// Returns the only entity matched by the Filter or a default value if no entity was found.
        /// Throws an exception if more than one entity was found
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity</typeparam>
        /// <param name="repository">A IReadonlyRepository to return the single element of</param>
        /// <param name="filter">A filter to test an element for a condition.</param>
        /// <returns>The single matched entity,  or default(TEntity) if none found</returns>
        public static async Task<TEntity> GetSingleOrDefaultAsync<TEntity>(this IReadonlyRepositoryAsync<TEntity> repository, IRepositoryFilter<TEntity> filter) where TEntity : class {
            return await repository.GetEntityWithSelectorAsync(filter, (a) => a.SingleOrDefault());
        }

        /* First */
        /// <summary>
        /// Returns the first entity matched by the Filter 
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity</typeparam>
        /// <param name="repository">A IReadonlyRepository to return the single entity of</param>
        /// <param name="filter">A filter to test an entity for a condition.</param>
        /// <returns>The first matched entity in the repository.</returns>
        public static TEntity GetFirst<TEntity>(this IReadonlyRepository<TEntity> repository, IRepositoryFilter<TEntity> filter) where TEntity : class {
            return repository.GetEntityWithSelector(filter, (a) => a.First());
        }

        /// <summary>
        /// Returns the first entity matched by the Filter 
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity</typeparam>
        /// <param name="repository">A IReadonlyRepository to return the single entity of</param>
        /// <param name="filter">A filter to test an entity for a condition.</param>
        /// <returns>The first matched entity in the repository.</returns>
        public static async Task<TEntity> GetFirstAsync<TEntity>(this IReadonlyRepositoryAsync<TEntity> repository, IRepositoryFilter<TEntity> filter) where TEntity : class {
            return await repository.GetEntityWithSelectorAsync(filter, (a) => a.First());
        }

        /* FirstOrDefault */
        /// <summary>
        /// Returns the first entity matched by the Filter 
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity</typeparam>
        /// <param name="repository">A IReadonlyRepository to return the single entity of</param>
        /// <param name="filter">A filter to test an entity for a condition.</param>
        /// <returns>The first matched entity in the repository.</returns>
        public static TEntity GetFirstOrDefault<TEntity>(this IReadonlyRepository<TEntity> repository, IRepositoryFilter<TEntity> filter) where TEntity : class {
            return repository.GetEntityWithSelector(filter, (a) => a.FirstOrDefault());
        }

        /// <summary>
        /// Returns the first entity matched by the Filter 
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity</typeparam>
        /// <param name="repository">A IReadonlyRepository to return the single entity of</param>
        /// <param name="filter">A filter to test an entity for a condition.</param>
        /// <returns>The first matched entity in the repository.</returns>
        public static async Task<TEntity> GetFirstOrDefaultAsync<TEntity>(this IReadonlyRepositoryAsync<TEntity> repository, IRepositoryFilter<TEntity> filter) where TEntity : class {
            return await repository.GetEntityWithSelectorAsync(filter, (a) => a.FirstOrDefault());
        }

        /* AsEnumerable */
        /// <summary>
        /// Returns all entities matched by the filter, optionally sorted and/or paged
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity</typeparam>
        /// <param name="repository">A IReadonlyRepository used to return the entities</param>
        /// <param name="filter">A filter to test entities for a condition.</param>
        /// <param name="sort">Optional sort specification</param>
        /// <param name="paging">Optional The current Page (one-based) and pageSize for paging</param>
        /// <returns>All matched entities, optionally sorted and/or optionally paged.</returns>
        public static IEnumerable<TEntity> AsEnumerable<TEntity>(this IReadonlyRepository<TEntity> repository, IRepositoryFilter<TEntity> filter, IRepositorySort<TEntity> sort = null, (int Page, int PageSize)? paging = null) where TEntity : class {
            return repository.GetWithFilter(filter, sort, paging).Data;
        }

        /// <summary>
        /// Returns all entities matched by the filter, optionally sorted and/or paged
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity</typeparam>
        /// <param name="repository">A IReadonlyRepository used to return the entities</param>
        /// <param name="filter">A filter to test entities for a condition.</param>
        /// <param name="sort">Optional sort specification</param>
        /// <param name="paging">Optional The current Page (one-based) and pageSize for paging</param>
        /// <returns>All matched entities, optionally sorted and/or optionally paged.</returns>
        public static async Task<IEnumerable<TEntity>> AsEnumerableAsync<TEntity>(this IReadonlyRepositoryAsync<TEntity> repository, IRepositoryFilter<TEntity> filter, IRepositorySort<TEntity> sort = null, (int Page, int PageSize)? paging = null) where TEntity : class {
            return (await repository.GetWithFilterAsync(filter, sort, paging)).Data;
        }

        /* GetCount */
        /// <summary>
        /// Returns number of entities matched by the filter
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity</typeparam>
        /// <param name="repository">A IReadonlyRepository used to count the entities</param>
        /// <param name="filter">A filter to test entities for a condition.</param>
        /// <returns>Total matched entities.</returns>
        public static int GetCount<TEntity>(this IReadonlyRepository<TEntity> repository, IRepositoryFilter<TEntity> filter) where TEntity : class {
            return repository.GetWithFilter(filter).Total;
        }

        /// <summary>
        /// Returns number of entities matched by the filter
        /// </summary>
        /// <typeparam name="TEntity">The type of the Repository Entity</typeparam>
        /// <param name="repository">A IReadonlyRepository used to count the entities</param>
        /// <param name="filter">A filter to test entities for a condition.</param>
        /// <returns>Total matched entities.</returns>
        public static async Task<int> GetCountAsync<TEntity>(this IReadonlyRepositoryAsync<TEntity> repository, IRepositoryFilter<TEntity> filter) where TEntity : class {
            return (await repository.GetWithFilterAsync(filter)).Total;
        }

    }
}

