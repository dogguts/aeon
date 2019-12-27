using System;
using System.Diagnostics ;

namespace Chinook.ViewModel.ListItem {
    [DebuggerDisplay("{Name} - {ArtistName}")]
    public class AlbumListItem {
        public long AlbumId { get; set; }
        public string Name { get; set; }
        public string ArtistName { get; set; }
    }
}
