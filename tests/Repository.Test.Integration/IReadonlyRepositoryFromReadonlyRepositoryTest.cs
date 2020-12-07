using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Linq;
using Xunit;
using View = Chinook.Repository.Model.View;


namespace Chinook.Repository.Integration.Tests {
    public class IReadonlyRepositoryFromReadonlyRepositoryTest : IClassFixture<ReadonlyRepositorySetup> {
        protected IReadonlyRepository<View.AlbumCountByArtists> _viewRepository;

        public IReadonlyRepositoryFromReadonlyRepositoryTest(ReadonlyRepositorySetup serviceSetup) {
            _viewRepository = serviceSetup.ServiceProvider.GetRequiredService<IReadonlyRepository<View.AlbumCountByArtists>>();
        }

        [Theory]
        [InlineData("Iron Maiden", "Led Zeppelin", "Deep Purple")]
        public void All(params string[] expected) {
            var albumCountsByArtist = _viewRepository.All().Take(3);

            var artistNames = albumCountsByArtist.Select(mt => mt.Name);
            Assert.Equal(expected, artistNames);
        }

        /// <summary>
        /// Get all models, eagerly loading some properties
        /// </summary>
        [Fact]
        public void AllWithIncludes() {
            //new filter for Artist model repository
            var include = new RepositoryInclude<Model.View.AlbumCountByArtists>();
            //include the artist 
            include.Include(a => a.Artist);

            var all = _viewRepository.All(include);

            var ironMaiden = all.First();
            Assert.Equal("Iron Maiden", ironMaiden.Artist.Name);
        }


        [Fact]
        public void GetWithFilter() {
            var filter = new RepositoryFilter<View.AlbumCountByArtists>(acba => acba.Name == "Iron Maiden");
            var (data, _) = _viewRepository.GetWithFilter(filter, paging: (1, 10));
            Assert.Single(data);
            Assert.Equal("Iron Maiden", data.First().Name);
            Assert.Equal(21, data.First().Total);
        }

        [Fact]
        public void GetByPrimaryKeyThrowsException() {
            var ex = Assert.Throws<AggregateException>(() => _viewRepository.Get("Iron Maiden"));
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }

        /// <summary>
        /// Get models using a RepositoryFilter without criteria, criteria will be ignored
        /// </summary>
        [Fact]
        public void GetWithNullFilter() {
            //creating a RepositoryFilter with criterea==null just creates a filter without criteria (same as All)
            var filter = new RepositoryFilter<View.AlbumCountByArtists>(null);
            var (_, total) = _viewRepository.GetWithFilter(filter);
            Assert.Equal(204, total);
        }

        /// <summary>
        /// Get models using a RepositoryFilter and order with RepositorySort
        /// </summary>
        [Theory]
        [InlineData("A", 21, "Audioslave", 3)]
        [InlineData("S", 13, "Santana", 3)]
        public void GetWithFilterAndSort(string artistStartsWith, long expectedTotalRecords, string expectedFirstArtist, long expectedTotalAlbums) {
            //get albums from artists starting with the letter "I" 
            var filter = new RepositoryFilter<View.AlbumCountByArtists>(a => a.Name.StartsWith(artistStartsWith));
            //order backwards by Album Title 
            var sort = new RepositorySort<View.AlbumCountByArtists>((ListSortDirection.Descending, al => al.Total));

            var (data, total) = _viewRepository.GetWithFilter(filter, sort);

            //there should be expectedTotal results in total
            Assert.Equal(expectedTotalRecords, total);

            //get first artist
            var artist = data.First();

            Assert.Equal(expectedFirstArtist, artist.Name);
            Assert.Equal(expectedTotalAlbums, artist.Total);
        }


        /// <summary>
        /// Get models using paging
        /// </summary>
        [Theory]
        [InlineData(1, 10, 10)]
        [InlineData(2, 10, 10)]
        [InlineData(400, 10, 0)]
        public void GetWithNullFilterAndSortAndPaging(int page, int pageSize, int expectedItemsInPage) {
            var filter = new RepositoryFilter<View.AlbumCountByArtists>(null);
            var sort = new RepositorySort<View.AlbumCountByArtists>((ListSortDirection.Descending, ax => ax.Total));

            var (data, total) = _viewRepository.GetWithFilter(filter, sort, (page, pageSize));
            var actualItemsInPage = data.Count();

            Assert.Equal(expectedItemsInPage, actualItemsInPage);
        }

        [Theory]
        [InlineData(0, 10)]
        public void PagingIsOneBased(int page, int pageSize) {
            var filter = new RepositoryFilter<View.AlbumCountByArtists>(null);
            var sort = new RepositorySort<View.AlbumCountByArtists>((ListSortDirection.Descending, ax => ax.Total));

            var ex = Assert.Throws<AggregateException>(() => _viewRepository.GetWithFilter(filter, sort, (page, pageSize)));

            Assert.IsType<ArgumentOutOfRangeException>(ex.InnerException);

        }

    }
}