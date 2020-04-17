using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Repository.Model {
    public partial class Playlist {
        public Playlist() {
            PlaylistTrack = new HashSet<PlaylistTrack>();
        }

        public long PlaylistId { get; set; }
        [Column(TypeName = "NVARCHAR(120)")]
        public string Name { get; set; }

        [InverseProperty("Playlist")]
        public ICollection<PlaylistTrack> PlaylistTrack { get; set; }
    }
}
