using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Repository.Model {
    public partial class Artist {
        public Artist() {
            Album = new HashSet<Album>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ArtistId { get; set; }
        [Column(TypeName = "NVARCHAR(120)")]
        public string Name { get; set; }

        [InverseProperty("Artist")]
        public ICollection<Album> Album { get; set; }
    }
}
