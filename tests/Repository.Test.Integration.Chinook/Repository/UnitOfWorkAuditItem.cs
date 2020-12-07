using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Chinook.Repository {
    public class UnitOfWorkAuditItem {
        public UnitOfWorkAuditItem(EntityEntry entry) {
            Entry = entry;
        }

        public string Action { get; set; }
        public EntityEntry Entry { get; }
        public string Model { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

        public bool HasTemporaryProperties => TemporaryProperties.Any();

        public override string ToString() {
            var sb = new System.Text.StringBuilder();

            var header = $"{Action} ==== {Model} {{{JsonSerializer.Serialize(KeyValues)}}} ";
            var padding = "".PadLeft(Math.Max(0, 120 - header.Length), '=');
            header = padding + header + padding;
            sb.AppendLine(header);
            string oldValues = OldValues.Count == 0 ? "null" : JsonSerializer.Serialize(OldValues);
            string newValues = NewValues.Count == 0 ? "null" : JsonSerializer.Serialize(NewValues);
            sb.AppendLine($"{oldValues}");
            sb.AppendLine("=>");
            sb.AppendLine($"{newValues}");

            return sb.ToString();
        }
    }


}