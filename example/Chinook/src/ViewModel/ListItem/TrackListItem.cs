using System;
using System.Diagnostics;

namespace Chinook.ViewModel.ListItem {
    [DebuggerDisplay("{Name} ({Duration}) - {AlbumName} - {ArtistName}")]
    public class TrackListItem {

        public long TrackId { get; set; }
        public string Name { get; set; }
        public string AlbumName { get; set; }
        public long AlbumId { get; set; }
        public string ArtistName { get; set; }
        public string GenreName { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
