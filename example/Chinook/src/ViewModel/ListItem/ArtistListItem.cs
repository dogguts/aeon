using System;
using System.Diagnostics;

namespace Chinook.ViewModel.ListItem {
    [DebuggerDisplay("{Name}")]
    public class ArtistListItem {
        public long ArtistId { get; set; }
        public string Name { get; set; }
    }
}

