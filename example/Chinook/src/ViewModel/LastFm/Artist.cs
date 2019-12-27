using System;
using System.Diagnostics;

namespace Chinook.ViewModel.LastFm {
 
    public class Artist {
        public long Id { get; set; }
        public string MbId { get; set; }
        public byte[] Image { get; set; }
        public string Bio { get; set; }
    }
}