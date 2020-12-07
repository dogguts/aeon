using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public class Repository<TEntity, TDbContext> : ReadonlyRepository<TEntity, TDbContext>, IRepository<TEntity>
            where TEntity : class
            where TDbContext : DbContext {

        public Repository(TDbContext context) : base(context) {
          
        }

        /// <summary>
        /// Adds the given entity to the context underlying the set in the Added state.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public virtual EntityEntry<TEntity> Add(TEntity entity) {
            return _dbSet.Add(entity);
        }
 
        private TEntity GetExisting(TEntity entity) {
            var primaryKey = _context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey();
            var primaryKeyValues = primaryKey.Properties.Select(p => typeof(TEntity).GetProperty(p.Name).GetValue(entity, null)).ToArray();

            return _dbSet.Find(primaryKeyValues);
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