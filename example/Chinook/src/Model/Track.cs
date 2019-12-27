using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Model {
    public partial class Track {
        public Track() {
            InvoiceLine = new HashSet<InvoiceLine>();
            PlaylistTrack = new HashSet<PlaylistTrack>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TrackId { get; set; }
        [Required]
        [Column(TypeName = "NVARCHAR(200)")]
        public string Name { get; set; }
        public long? AlbumId { get; set; }
        public long MediaTypeId { get; set; }
        public long? GenreId { get; set; }
        [Column(TypeName = "NVARCHAR(220)")]
        public string Composer { get; set; }
        public long Milliseconds { get; set; }
        public long? Bytes { get; set; }
        [Required]
        [Column(TypeName = "NUMERIC(10,2)")]
        public string UnitPrice { get; set; }

        [ForeignKey("AlbumId")]
        [InverseProperty("Track")]
        public Album Album { get; set; }
        [ForeignKey("GenreId")]
        [InverseProperty("Track")]
        public Genre Genre { get; set; }
        [ForeignKey("MediaTypeId")]
        [InverseProperty("Track")]
        public MediaType MediaType { get; set; }
        [InverseProperty("Track")]
        public ICollection<InvoiceLine> InvoiceLine { get; set; }
        [InverseProperty("Track")]
        public ICollection<PlaylistTrack> PlaylistTrack { get; set; }
    }
}
