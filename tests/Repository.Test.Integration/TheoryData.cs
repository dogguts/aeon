namespace Chinook.Repository.Integration.Tests {
    public class Track {
        public string Name { get; set; }
        public long Milliseconds { get; set; }
        public long? Size { get; set; }

        public string UnitPrice { get; set; } = "0.99";
        public int MediaTypeId { get; set; } = 1; //MP3 
    }
}