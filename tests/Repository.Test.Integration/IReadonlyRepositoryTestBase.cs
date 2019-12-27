using System;
using System.Linq;
using Aeon.Core.Repository.Infrastructure;
using Model = Chinook.Repository.Model;
using View = Chinook.Repository.Model.View;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Aeon.Core.Repository;
using System.ComponentModel;

namespace Chinook.Repository.Integration.Tests {
    public abstract class IReadonlyRepositoryTestBase {
        protected IReadonlyRepository<Model.Genre> _genreRepository;
        protected IReadonlyRepository<Model.MediaType> _mediaTypeRepository;
        protected IReadonlyRepository<Model.Artist> _artistRepository;
        protected IReadonlyRepository<Model.Album> _albumRepository;
        protected IReadonlyRepository<Model.Genre> _explicitReadonlyGenreRepository;
        protected IReadonlyRepository<View.AlbumCountByArtists> _viewRepository;

        public IReadonlyRepositoryTestBase(ReadonlyRepositorySetup serviceSetup) {

        }

        /*  
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
        */


        public abstract void AllWithIncludes();
        public abstract void All(params string[] expected);


       

        /// <summary>
        /// Get models using paging
        /// </summary>
        [Theory]
        [InlineData(0, 10, 347, new string[] { "[1997] Black Light Syndrome", "Zooropa", "Worlds", "Weill: The Seven Deadly Sins", "Warner 25 Anos", "War", "Walking Into Clarksdale", "Wagner: Favourite Overtures", "Vs.", "Vozes do MPB" })]
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