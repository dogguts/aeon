using System.Collections.Generic;

namespace Chinook.ViewModel {
    public class AlbumViewModel {
        public long AlbumId { get; set; }
        public string MbId { get; set; }
        public string Name { get; set; }
        public long ArtistId { get; set; }
        public string ArtistName { get; set; }
        public string Summary { get; set; }
        public IEnumerable<ListItem.TrackListItem> Tracks { get; set; }
    }
}