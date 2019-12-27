using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chinook.ViewModel {
    public class ArtistViewModel {
        public long ArtistId { get; set; }
        public string MbId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Bio { get; set; }
        public IEnumerable<ListItem.AlbumListItem> Albums { get; set; }

        public IEnumerable<ListItem.TrackListItem> TopTracks { get; set; }
    }
}