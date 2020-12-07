using Aeon.Core.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Aeon.Samples.Extend.SoftDelete.Infrastructure {
    public class SoftDeleteRepository<TEntity, TDbContext> : Repository<TEntity, TDbContext>
      where TEntity : class
      where TDbContext : DbContext {

        private readonly PropertyInfo _softDeletePropertyInfo;

        public SoftDeleteRepository(TDbContext context, string softDeletePropertyName = "Deleted") : base(context) {
            _softDeletePropertyInfo = typeof(TEntity).GetProperty(softDeletePropertyName);
            if (_softDeletePropertyInfo == null) {
                throw new ArgumentException($"Property {softDeletePropertyName} not found in {typeof(TEntity).FullName}");
            }
        }

        public override void Delete(TEntity entity) {
            bool originalDeleteFlag = (bool)_softDeletePropertyInfo.GetValue(entity);

            // set 'SoftDeleteProperty' to true
            _softDeletePropertyInfo.SetValue(entity, true);

            // get current EntityEntry
            var entryState = _context.Entry(entity).State;

            // Delete when EntityState == Added => don't add
            if (entryState == EntityState.Added) {
                _context.Entry(entity).State = EntityState.Detached;
                return;
            }

            // Delete when Untracked = ignore delete
            if (entryState == EntityState.Detached) {
                return;
            }


            if (originalDeleteFlag != true) {
                _context.Entry(entity).State = EntityState.Modified;
            }

        }

    }
}
