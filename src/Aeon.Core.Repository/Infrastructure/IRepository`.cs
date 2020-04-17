using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// A generic interface for a Repository.
    /// </summary>
    /// <typeparam name="TEntity">The Entity type for the Repository</typeparam>
    public interface IRepository<TEntity> : IReadonlyRepository<TEntity>, IReadonlyRepositoryAsync<TEntity> where TEntity : class {

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity"></param>    
        EntityEntry<TEntity> Add(TEntity entity);

        /// <summary>
        /// Marks an existing entity as updated.
        /// </summary>
        /// <param name="entity">The updated entity.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Marks an existing entity for deletion.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Delete(TEntity entity);
    }
}