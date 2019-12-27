using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp.Repository {
    public class TodoAppSimpleDbUnitOfWork : Infrastructure.ITodoAppDbUnitOfWork {
        private readonly TodoAppDbContext _context;

        public TodoAppSimpleDbUnitOfWork(TodoAppDbContext context) {
            _context = context;
        }
        private void ApplySoftDelete(IEnumerable<EntityEntry> entries) {
            var softDeletes = entries.Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Deleted && e.Entity is Model.Infrastructure.ISoftDelete);
            foreach (var entry in softDeletes) {

#if DEBUG
                object[] keyParts = entry.Metadata.FindPrimaryKey()
                           .Properties
                           .Select(p => entry.Property(p.Name).CurrentValue.ToString())
                           .ToArray();

                System.Diagnostics.Debug.WriteLine($"ApplySoftDelete: {entry.Entity.GetType().FullName} {{{string.Join(",", keyParts)}}}");
# endif
                entry.CurrentValues[nameof(Model.Infrastructure.ISoftDelete.Deleted)] = true;
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
        }

        public void Commit() {
            _context.ChangeTracker.DetectChanges();

            var entries = _context.ChangeTracker.Entries();


            ApplySoftDelete(entries);

            System.Diagnostics.Debug.WriteLine("Commit :: TodoAppDbContext =" + ((object)_context).GetHashCode());
            _context.SaveChanges();
        }
    }
}