using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chinook.Repository {

    public class ChinookDbUnitOfWorkWithConsoleLogging : Infrastructure.IChinookDbUnitOfWork {
        private readonly ChinookDbContext _context;

        public ChinookDbUnitOfWorkWithConsoleLogging(ChinookDbContext context) {
            _context = context;
        }

        public void Commit() {
            Commit(true);
        }

        private List<UnitOfWorkAuditItem> OnBeforeSaveChanges() {

            _context.ChangeTracker.DetectChanges();
            var auditEntries = new List<UnitOfWorkAuditItem>();

            foreach (var entry in _context.ChangeTracker.Entries()) {

                var auditEntry = new UnitOfWorkAuditItem(entry) {
                    Model = entry.Entity.GetType().ToString()
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties) {
                    if (property.IsTemporary) {
                        // value will be generated by the database, get the value after saving
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey()) {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State) {
                        case EntityState.Added:
                            auditEntry.Action = "Add";
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.Action = "Delete";
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified) {
                                auditEntry.Action = "Modify";
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }
            // print audit items that have all the modifications
            foreach (var auditEntry in auditEntries.Where(a => !a.HasTemporaryProperties)) {
                if (auditEntry.NewValues.Count != 0 || auditEntry.OldValues.Count != 0) {
                    Console.WriteLine(auditEntry.ToString());
                }
            }

            // keep a list of entries where the value of some properties are unknown at this step
            return auditEntries.Where(a => a.HasTemporaryProperties).ToList();
        }

        public void Commit(bool auditEnabled) {
            if (auditEnabled) {
                var auditEntries = OnBeforeSaveChanges();
                if (OnAfterSaveChanges(auditEntries)) {
                    //audit entries changed
                }
            }
            _context.SaveChanges();
        }

        private bool OnAfterSaveChanges(List<UnitOfWorkAuditItem> auditEntries) {
            if (auditEntries == null || auditEntries.Count == 0)
                return false;

            foreach (var auditEntry in auditEntries) {
                // Get the final value of the temporary properties
                foreach (var prop in auditEntry.TemporaryProperties) {
                    if (prop.Metadata.IsPrimaryKey()) {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    } else {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                // print the audit item
                if (auditEntry.NewValues.Count != 0 || auditEntry.OldValues.Count != 0) {
                    Console.WriteLine(auditEntry.ToString());
                }
            }

            return true;
        }
    }
}
