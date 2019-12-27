

using System;
using System.Diagnostics;

namespace Chinook.ViewModel.LastFm {

    public class Album {
        public long Id { get; set; }
        public string MbId { get; set; }
        public string Summary { get; set; }
        public byte[] Image { get; set; }
    }
}