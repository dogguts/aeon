using Aeon.Core.Repository.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Priority;

namespace Chinook.Repository.Integration.Tests {
    public class RepositoryTest : IClassFixture<RepositorySetup> {
        private readonly ServiceProvider _serviceProvider;

        public RepositoryTest(RepositorySetup serviceSetup) {
            _serviceProvider = serviceSetup.ServiceProvider;
        }

        /// <summary>
        /// TheoryData for AddArtistAlbumTracksGenre
        /// </summary>
        /// <typeparam name="Track"></typeparam>
        public class ArtistAlbumTracksGenreTestData : TheoryData<string, string, string, IList<Track>> {
            private static IList<Track> BlizzardBeastsTracks() {
                return new List<Track>() {
                    new Track (){Name="Intro", Milliseconds =60000 },
                    new Track (){Name="Blizzard Beasts", Milliseconds =169000 },
                    new Track (){Name="Nebular Ravens Winter", Milliseconds =253000 },
                    new Track (){Name="Suns That Sank Below", Milliseconds =167000 },
                    new Track (){Name="Battlefields", Milliseconds =220000 },
                    new Track (){Name="Mountains of Might", Milliseconds =398000 },
                    new Track (){Name="Noctambulant", Milliseconds =142000 },
                    new Track (){Name="Winter of the Ages", Milliseconds =153000 },
                    new Track (){Name="Frostdemonstorm", Milliseconds =174000 },
                };
            }

            private static IList<Track> TaraTracks() {
                return new List<Track> {
                    new Track (){Name="Tara", Milliseconds =116000 },
                    new Track (){Name="Pillars of Mercy", Milliseconds =261000 },
                    new Track (){Name="A Shield with an Iron Face", Milliseconds =202000 },
                    new Track (){Name="Manannán", Milliseconds =399000 },
                    new Track (){Name="The Cognate House of Courtly Witches Lies West of County Meath", Milliseconds =259000 },
                    new Track (){Name="She Cries the Quiet Lake", Milliseconds =250000 },
                    new Track (){Name="Yrp Lluyddawc", Milliseconds =111000 },
                    new Track (){Name="From Ancient Times (Starless Skies Burn to Ash)", Milliseconds =233000 },
                    new Track (){Name="Four Crossed Wands (Spell 181)", Milliseconds =286000 },
                    new Track (){Name="Vorago (Spell 182)", Milliseconds =345000 },
                    new Track (){Name="Bron (Of the Waves)", Milliseconds =92000 },
                    new Track (){Name="Stone of Destiny (...for Magh Slecht and Ard Righ)", Milliseconds =466000 },
                    new Track (){Name="Tara (Recapitulation)", Milliseconds =105000 },
                };
            }

            private static IList<Track> K3DeWereldRondTracks() {
                return new List<Track> {
                    new Track (){Name="Liefdeskapitein", Milliseconds =214000 },
                    new Track (){Name="Een ongelooflijk idee", Milliseconds =215000 },
                    new Track (){Name="Dat ik van je hou", Milliseconds =213000 },
                    new Track (){Name="Wij blijven vrienden", Milliseconds =192000 },
                    new Track (){Name="Zou er iemand zijn op Mars", Milliseconds =225000 },
                    new Track (){Name="Fiesta de amor", Milliseconds =189000 },
                    new Track (){Name="Hakuna matata", Milliseconds =206000 },
                    new Track (){Name="Rokjes", Milliseconds =229000 },
                    new Track (){Name="(ik wil) Bamba", Milliseconds =199000 },
                    new Track (){Name="Alle Chinezen", Milliseconds =185000 },
                    new Track (){Name="Fiets", Milliseconds =214000 },
                    new Track (){Name="Babouchka", Milliseconds =212000 },
                    new Track (){Name="Bonustrack: Superhero", Milliseconds =211000 },
                };
            }

            public ArtistAlbumTracksGenreTestData() {
                Add("Immortal", "Blizzard Beasts", "Black Metal", BlizzardBeastsTracks());
                Add("Absu", "Tara", "Black Metal", TaraTracks());
                Add("K3", "De Wereld Rond", "Pop", K3DeWereldRondTracks());
            }
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
            var artistRepository = _serviceProvider.GetRequiredService<IRepository<Model.Artist>>();
            var artist = artistRepository.Get(id);
            Assert.Equal(expectedArtistName, artist.Name);
        }

        [Theory]
        [Priority(1)]
        [ClassData(typeof(ArtistAlbumTracksGenreTestData))]
        public void AddArtistAlbumTracksGenre(string artistName, string albumTitle, string genreName, IList<Track> tracks) {
            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            // simulate scope (so that the injected DbContext is garantied to be the same within this scope)           
            // this wouldn't be an issue with dependency injection in combination with aspnet core
            using (var scope = scopeFactory.CreateScope()) {

                var uow = scope.ServiceProvider.GetRequiredService<Infrastructure.IChinookDbUnitOfWork>();
                var genreRepository = scope.ServiceProvider.GetRequiredService<IRepository<Model.Genre>>();
                var artistRepository = scope.ServiceProvider.GetRequiredService<IRepository<Model.Artist>>();
                var trackRepository = scope.ServiceProvider.GetRequiredService<IRepository<Model.Track>>();

                // find existing artist by name or create new one
                Model.Artist artist = artistRepository.GetWithFilter(Filter.ArtistFilter.ByName(artistName)).Data.FirstOrDefault() ?? new Model.Artist() { Name = artistName };

                // create new album (let's assume it never exists in our current database)
                var album = new Model.Album() { Title = albumTitle, Artist = artist };

                // find existing genre by name or create new one
                Model.Genre genre = genreRepository.GetWithFilter(Filter.GenreFilter.ByName(genreName)).Data.FirstOrDefault() ?? new Model.Genre() { Name = genreName };

                // Add tracks to album
                var newTracks = tracks.Select(t => new Model.Track() {
                    Name = t.Name,
                    Album = album,
                    MediaTypeId = t.MediaTypeId,
                    Genre = genre,
                    Milliseconds = t.Milliseconds,
                    UnitPrice = t.UnitPrice,
                    Bytes = t.Size
                });
                // add tracks to our track repository (EF changetracker will take care of all related entities (like album, genre and artist) )
                newTracks.ToList().ForEach(newTrack => {
                    trackRepository.Add(newTrack);
                });
                // commit changes to database
                uow.Commit();
            }

            //check data in the database
            using (var scope = scopeFactory.CreateScope()) {
                var artistRepository = scope.ServiceProvider.GetRequiredService<IRepository<Model.Artist>>();
                // get discography by artistname
                var (Data, Total) = artistRepository.GetWithFilter(Filter.ArtistFilter.Discography(artistName));
                // get first hit (in our database there will only be one hit)
                var band = Data.First();
                // Asserts (database vs testdata)
                Data.ToList().ForEach(a => {
                    Assert.Equal(artistName, a.Name);
                    a.Album.ToList().ForEach(al => {
                        Assert.Equal(albumTitle, al.Title);
                        int trackIndex = 0;
                        al.Track.ToList().ForEach(t => {
                            Assert.Equal(tracks[trackIndex++].Name, t.Name);
                        });
                    });
                });
            }
        }


        [Theory]
        [Priority(2)]
        [InlineData("Immortal", "Mortal")]
        [InlineData("Absu", "Abzu")]
        public void UpdateArtist(string fromArtistName, string toArtistName) {
            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope()) {
                var uow = scope.ServiceProvider.GetRequiredService<Infrastructure.IChinookDbUnitOfWork>();
                var artistRepository = scope.ServiceProvider.GetRequiredService<IRepository<Model.Artist>>();

                // get artist by name
                var dbArtist = artistRepository.GetWithFilter(Filter.ArtistFilter.Discography(fromArtistName)).Data.First();

                dbArtist.Name = toArtistName;

                uow.Commit();
            }

            using (var scope = scopeFactory.CreateScope()) {
                var artistRepository = scope.ServiceProvider.GetRequiredService<IRepository<Model.Artist>>();

                var dbArtist = artistRepository.GetWithFilter(Filter.ArtistFilter.ByName(toArtistName)).Data.FirstOrDefault();
                Assert.NotNull(dbArtist);
            }
        }

        [Theory]
        [Priority(3)]
        [InlineData("AC/DC")]
        public void DeleteArtist(string artistToDelete) {
            var scopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope()) {
                var uow = scope.ServiceProvider.GetRequiredService<Infrastructure.IChinookDbUnitOfWork>();
                var artistRepository = scope.ServiceProvider.GetRequiredService<IRepository<Model.Artist>>();
                var albumRepository = scope.ServiceProvider.GetRequiredService<IRepository<Model.Album>>();
                var trackRepository = scope.ServiceProvider.GetRequiredService<IRepository<Model.Track>>();

                var artist = artistRepository.GetWithFilter(Filter.ArtistFilter.Discography(artistToDelete)).Data.First();

                foreach (var album in artist.Album) {
                    foreach (var track in album.Track) {
                        trackRepository.Delete(track);
                    }
                    albumRepository.Delete(album);
                }
                artistRepository.Delete(artist);

                uow.Commit();
            }

            using (var scope = scopeFactory.CreateScope()) {
                var artistRepository = scope.ServiceProvider.GetRequiredService<IRepository<Model.Artist>>();

                var dbArtist = artistRepository.GetWithFilter(Filter.ArtistFilter.ByName(artistToDelete)).Data.FirstOrDefault();
                Assert.Null(dbArtist);
            }
        }
    }
}