using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aeon.Samples.UnitOfWork.CustomLogging.Models {
    public class AuditContext : DbContext {
        public DbSet<Audit> Audits { get; set; }

        public AuditContext(DbContextOptions<AuditContext> options) : base(options) {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Audit>(entity => {
                entity.Property(audit => audit.Action).HasConversion(a => a.ToString(), s => Enum.Parse<AuditAction>(s));
            });

            modelBuilder.Entity<Audit>().HasData(new Audit { AuditId = 1, Action = AuditAction.AuditLogCreated, DateTime = DateTime.UtcNow });

        }
    }

    public enum AuditAction {
        Unknown = 0,
        AuditLogCreated,
        Insert,
        Update,
        Delete
    }

    public class Audit {
        [Key]
        public int AuditId { get; set; }
        public DateTime DateTime { get; set; }

        public AuditAction Action { get; set; }
        public string Model { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
    }

}
