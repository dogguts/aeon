using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace Chinook.Repository.Integration.Tests {

    public class IReadonlyRepositoryFromRepositoryTest : IClassFixture<ReadonlyRepositorySetup> {
        protected IReadonlyRepository<Model.Genre> _genreRepository;
        protected IReadonlyRepository<Model.MediaType> _mediaTypeRepository;
        protected IReadonlyRepository<Model.Artist> _artistRepository;
        protected IReadonlyRepository<Model.Album> _albumRepository;
        protected IReadonlyRepository<Model.Genre> _explicitReadonlyGenreRepository;

        public IReadonlyRepositoryFromRepositoryTest(ReadonlyRepositorySetup serviceSetup) {
            _genreRepository = serviceSetup.ServiceProvider.GetRequiredService<IRepository<Model.Genre>>();
            _mediaTypeRepository = serviceSetup.ServiceProvider.GetRequiredService<IRepository<Model.MediaType>>();
            _artistRepository = serviceSetup.ServiceProvider.GetRequiredService<IRepository<Model.Artist>>();
            _albumRepository = serviceSetup.ServiceProvider.GetRequiredService<IRepository<Model.Album>>();
            _explicitReadonlyGenreRepository = serviceSetup.ServiceProvider.GetRequiredService<IReadonlyRepository<Model.Genre>>();
        }

        /// <summary>
        /// Get all Models
        /// </summary>
        [Theory]
        [InlineData("MPEG audio file", "Protected AAC audio file", "Protected MPEG-4 video file", "Purchased AAC audio file", "AAC audio file")]
        public void All(params string[] expected) {
            var allMediaTypes = _mediaTypeRepository.All();

            var allMediaTypeNames = allMediaTypes.Select(mt => mt.Name);
            Assert.Equal(expected, allMediaTypeNames);
        }

        /// <summary>
        /// Get all models, eagerly loading some properties
        /// </summary>
        [Fact]
        public void AllWithIncludes() {
            //new filter for Artist model repository
            var artistInclude = new RepositoryInclude<Model.Artist>();
            //include from the artist all albums, from the albums all tracks, for all tracks , the genre
            artistInclude.Include(a => a.Album).ThenInclude(al => al.Track).ThenInclude(t => t.Genre);
            //Include from the artist all albums, from the albums all tracks, for all tracks , the mediaType
            artistInclude.Include(a => a.Album).ThenInclude(al => al.Track).ThenInclude(t => t.MediaType);

            var all = _artistRepository.All(artistInclude);

            var acdc = all.First();
            Assert.Equal("Rock", acdc.Album.First().Track.First().Genre.Name);
        }

        /// <summary>
        /// Get model by (primary) key
        /// </summary>
        /// <remarks>
        /// If the key of the model is a composite key, provide the key components in the right order
        /// </remarks>
        [Theory]
        [InlineData(1, "AC/DC")]
        public void Get(long id, string expectedArtistName) {
            var artist = _artistRepository.Get(id);
            Assert.Equal(expectedArtistName, artist.Name);
        }

        /// <summary>
        /// Get model by (primary) key, eagerly loading some properties
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expectedGenreName"></param>
        [Theory]
        [InlineData(1, "Rock")]
        public void GetWithIncludes(long id, string expectedGenreName) {
            var artistInclude = new RepositoryInclude<Model.Artist>();
            //Include from the artist all albums, from the albums all tracks, for all tracks , the genre
            artistInclude.Include(a => a.Album).ThenInclude(al => al.Track).ThenInclude(t => t.Genre);
            //Include from the artist all albums, from the albums all tracks, for all tracks , the mediaType
            artistInclude.Include(a => a.Album).ThenInclude(al => al.Track).ThenInclude(t => t.MediaType);

            var acdc = _artistRepository.Get(artistInclude, id);
            Assert.Equal(acdc.Album.First().Track.First().Genre.Name, expectedGenreName);
        }

        /// <summary>
        /// Get models using a RepositoryFilter
        /// </summary>
        [Theory]
        [InlineData("AC/DC", "Those", "For Those About To Rock We Salute You")]
        public void GetWithFilter(string artistName, string albumTitlePart, string expectedAlbumTitle) {
            //get albums by "AC/DC" where the album title contains "those" (case-insentive)
            var filter = new RepositoryFilter<Model.Album>(a => a.Artist.Name == artistName && a.Title.Contains(albumTitlePart));
            var (data, total) = _albumRepository.GetWithFilter(filter);

            //there should be exactly one result
            Assert.Equal(1, total);

            var album = data.First();

            Assert.Equal(expectedAlbumTitle, album.Title);
        }

        /// <summary>
        /// Get models using a RepositoryFilter and order with RepositorySort
        /// </summary>
        [Theory]
        [InlineData("I", 23, "Virtual XI")]
        [InlineData("A", 27, "Worlds")]
        public void GetWithFilterAndSort(string artistStartsWith, long expectedTotal, string expectedFirstAlbum) {
            //get albums from artists starting with the letter "I" 
            var filter = new RepositoryFilter<Model.Album>(a => a.Artist.Name.StartsWith(artistStartsWith));
            //order backwards by Album Title 
            var sort = new RepositorySort<Model.Album>((ListSortDirection.Descending, al => al.Title));

            var (data, total) = _albumRepository.GetWithFilter(filter, sort);

            //there should be 23 results in total
            Assert.Equal(expectedTotal, total);

            //get first album 
            var album = data.First();

            Assert.Equal(expectedFirstAlbum, album.Title);
        }

        /// <summary>
        /// Get models using a RepositoryFilter without criteria, criteria will be ignored
        /// </summary>
        [Fact]
        public void GetWithNullFilter() {
            //creating a RepositoryFilter with criterea==null just creates a filter without criteria (same as All)
            var filter = new RepositoryFilter<Model.Album>(null);
            var (_, total) = _albumRepository.GetWithFilter(filter);
            Assert.Equal(347, total);
        }

        /// <summary>
        /// Get models using paging
        /// </summary>
        [Theory]
        [InlineData(1, 10, 347, new string[] { "[1997] Black Light Syndrome", "Zooropa", "Worlds", "Weill: The Seven Deadly Sins", "Warner 25 Anos", "War", "Walking Into Clarksdale", "Wagner: Favourite Overtures", "Vs.", "Vozes do MPB" })]
        [InlineData(2, 10, 347, new string[] { "Voodoo Lounge", "Volume Dois", "Vivaldi: The Four Seasons", "Virtual XI", "Vinícius De Moraes - Sem Limite", "Vinicius De Moraes", "Vault: Def Leppard's Greatest Hits", "Van Halen III", "Van Halen", "Use Your Illusion II" })]
        [InlineData(400, 10, 347, new string[] { })]
        public void GetWithNullFilterAndSortAndPaging(int page, int pageSize, int expectedTotal, string[] expectedAlbumTitles) {
            var filter = new RepositoryFilter<Model.Album>(null);
            var sort = new RepositorySort<Model.Album>((ListSortDirection.Descending, a => a.Title));

            var (data, total) = _albumRepository.GetWithFilter(filter, sort, (page, pageSize));

            Assert.Equal(expectedTotal, total);

            var albums = data.Select(a => a.Title).ToList();

            Assert.Equal(expectedAlbumTitles, albums);
        }

        /// <summary>
        /// Get models using paging
        /// </summary>
        [Theory]
        [InlineData(1, 10, 347, new string[] { "[1997] Black Light Syndrome", "Zooropa", "Worlds", "Weill: The Seven Deadly Sins", "Warner 25 Anos", "War", "Walking Into Clarksdale", "Wagner: Favourite Overtures", "Vs.", "Vozes do MPB" })]
        [InlineData(2, 10, 347, new string[] { "Voodoo Lounge", "Volume Dois", "Vivaldi: The Four Seasons", "Virtual XI", "Vinícius De Moraes - Sem Limite", "Vinicius De Moraes", "Vault: Def Leppard's Greatest Hits", "Van Halen III", "Van Halen", "Use Your Illusion II" })]
        [InlineData(400, 10, 347, new string[] { })]
        public void GetWithFilterNullAndSortAndPaging(int page, int pageSize, int expectedTotal, string[] expectedAlbumTitles) {
            var sort = new RepositorySort<Model.Album>((ListSortDirection.Descending, a => a.Title));

            var (data, total) = _albumRepository.GetWithFilter(null, sort, (page, pageSize));

            Assert.Equal(expectedTotal, total);

            var albums = data.Select(a => a.Title).ToList();

            Assert.Equal(expectedAlbumTitles, albums);
        }
    }
}
