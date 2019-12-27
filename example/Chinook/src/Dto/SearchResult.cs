namespace Chinook.Dto {
    public class SearchResult {
        public long Id { get; set; }
        public SearchResultType Type { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
    }
}