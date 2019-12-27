using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Model {
    public partial class MediaType {
        public MediaType() {
            Track = new HashSet<Track>();
        }

        public long MediaTypeId { get; set; }
        [Column(TypeName = "NVARCHAR(120)")]
        public string Name { get; set; }

        [InverseProperty("MediaType")]
        public ICollection<Track> Track { get; set; }
    }
}
