using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// A generic interface for a Repository.
    /// </summary>
    /// <typeparam name="T">The Entity type for the Repository</typeparam>
    public interface IRepository<T> : IReadonlyRepository<T>, IReadonlyRepositoryAsync<T> where T : class {

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity"></param>    
        EntityEntry<T> Add(T entity);

        /// <summary>
        /// Marks an existing entity as updated.
        /// </summary>
        /// <param name="entity">The updated entity.</param>
        void Update(T entity);

        /// <summary>
        /// Marks an existing entity for deletion.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Delete(T entity);
    }
}