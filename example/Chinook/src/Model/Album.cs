using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Model {
    public partial class Album {
        public Album() {
            Track = new HashSet<Track>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AlbumId { get; set; }
        [Required]
        [Column(TypeName = "NVARCHAR(160)")]
        public string Title { get; set; }
        public long ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        [InverseProperty("Album")]
        public Artist Artist { get; set; }
        [InverseProperty("Album")]
        public ICollection<Track> Track { get; set; }
    }
}
