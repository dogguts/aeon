using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Repository.Model {
    public partial class Genre {
        public Genre() {
            Track = new HashSet<Track>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long GenreId { get; set; }
        [Column(TypeName = "NVARCHAR(120)")]
        public string Name { get; set; }

        [InverseProperty("Genre")]
        public ICollection<Track> Track { get; set; }
    }
}
