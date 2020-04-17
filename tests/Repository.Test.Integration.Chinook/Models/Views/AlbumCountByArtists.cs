
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Chinook.Repository.Model.View {
    [DebuggerDisplay("{Name} #{Total}")]
    public class AlbumCountByArtists {

        public long ArtistId { get; set; }

        [ForeignKey("ArtistId")]
        public Artist Artist { get; set; }

        public string Name { get; set; }
        public int Total { get; set; }
    }
}