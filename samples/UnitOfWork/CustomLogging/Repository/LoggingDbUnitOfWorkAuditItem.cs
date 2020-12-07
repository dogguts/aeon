using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;

namespace Aeon.Samples.UnitOfWork.CustomLogging.Repository {
    public class LoggingDbUnitOfWorkAuditItem<TAuditEntity> {
     
        public string ModelName { get; set; }

        public TAuditEntity AuditEntity{ get; set; }

        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

        public bool HasTemporaryProperties => TemporaryProperties.Any();
 
    }
}
