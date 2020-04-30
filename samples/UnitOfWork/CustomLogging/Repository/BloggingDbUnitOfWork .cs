using Aeon.Core.Repository.Infrastructure;
using Aeon.Samples.UnitOfWork.CustomLogging.Models;
using System;
using System.Text.Json;

namespace Aeon.Samples.UnitOfWork.CustomLogging.Repository {
    public interface IBloggingDbUnitOfWork : IUnitOfWork {
    }

    public class BloggingDbUnitOfWork : LoggingDbUnitOfWork<Audit, BloggingContext, AuditContext>, IBloggingDbUnitOfWork {

        private static Audit Map(LoggingDbUnitOfWorkAuditItem<Audit> auditEntry) {
            var action = AuditAction.Unknown;

            if (auditEntry.OldValues.Count == 0 && auditEntry.NewValues.Count > 0) {
                action = AuditAction.Insert;
            }
            if (auditEntry.OldValues.Count > 0 && auditEntry.NewValues.Count == 0) {
                action = AuditAction.Delete;
            }
            if (auditEntry.OldValues.Count > 0 && auditEntry.NewValues.Count > 0) {
                action = AuditAction.Update;
            }

            if (auditEntry.AuditEntity == null) {
                Console.WriteLine(action);
            }


            Audit audit = auditEntry.AuditEntity ?? new Audit() {
                Model = auditEntry.ModelName,
                DateTime = DateTime.UtcNow,
                Action = action
            };


            audit.KeyValues = JsonSerializer.Serialize(auditEntry.KeyValues);
            audit.OldValues = auditEntry.OldValues.Count == 0 ? null : JsonSerializer.Serialize(auditEntry.OldValues);
            audit.NewValues = auditEntry.NewValues.Count == 0 ? null : JsonSerializer.Serialize(auditEntry.NewValues);

            auditEntry.AuditEntity = audit;

            return audit;
        }
        public BloggingDbUnitOfWork(IRepository<Audit> auditRepository, BloggingContext context, AuditContext auditContext) : base(auditRepository, context, auditContext, Map) {

        }

    }
}
