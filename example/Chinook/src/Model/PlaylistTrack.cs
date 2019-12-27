using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Model {
    public partial class PlaylistTrack {
        public long PlaylistId { get; set; }
        public long TrackId { get; set; }

        [ForeignKey("PlaylistId")]
        [InverseProperty("PlaylistTrack")]
        public Playlist Playlist { get; set; }
        [ForeignKey("TrackId")]
        [InverseProperty("PlaylistTrack")]
        public Track Track { get; set; }
    }
}
