using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using AutoMapper;
using Chinook.Repository.Infrastructure;
using Chinook.ViewModel.LastFm;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace Chinook.Service {

    public interface ILastFmService {
        Task<Chinook.ViewModel.LastFm.Artist> GetArtist(long artistId);
        void ResetArtistCache(long artistId);
        Task<Chinook.ViewModel.LastFm.Album> GetAlbum(long artistId);
        void ResetAlbumCache(long albumId);
    }

    public class LastFmService : ILastFmService {
        public const int CACHEDSECONDS = 60 * 2;
        public class LastFmSettings {
            public string Key { get; set; }
            public string Secret { get; set; }
        }

        private readonly LastfmClient _lastfmClient;
        private readonly IRepository<Chinook.Model.Artist> _artistRepository;
        private readonly IRepository<Chinook.Model.Album> _albumRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        public LastFmService(IMemoryCache memoryCache,
                             LastfmClient lastfmClient,
                             IRepository<Chinook.Model.Artist> artistRepository,
                             IRepository<Chinook.Model.Album> albumRepository,
                             IMapper mapper) {
            _memoryCache = memoryCache;
            _lastfmClient = lastfmClient;
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _mapper = mapper;
        }

        public void ResetArtistCache(long artistId) {
            string cacheKey = $"_Artist-{artistId}";
            _memoryCache.Remove(cacheKey);
        }

        public async Task<Chinook.ViewModel.LastFm.Artist> GetArtist(long artistId) {
            string cacheKey = $"_Artist-{artistId}";
            var lastFmArtist = new Chinook.ViewModel.LastFm.Artist();
            if (artistId != 0 && !_memoryCache.TryGetValue(cacheKey, out lastFmArtist)) {
                //not cached, create and put in cache
                var dbArtist = _artistRepository.Get(artistId);

                var lastFmArtistResponse = await _lastfmClient.Artist.GetInfoAsync(dbArtist.Name);

                lastFmArtist = _mapper.Map<Chinook.ViewModel.LastFm.Artist>(lastFmArtistResponse.Content);


                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(CACHEDSECONDS));
                _memoryCache.Set(cacheKey, lastFmArtist, cacheEntryOptions);
            }
            return lastFmArtist;
        }

        public async Task<Chinook.ViewModel.LastFm.Album> GetAlbum(long albumId) {

            string cacheKey = $"_Album-{albumId}";
            if (!_memoryCache.TryGetValue(cacheKey, out Album lastFmAlbum)) {
                //not cached, create and put in cache
                var inc = new RepositoryInclude<Model.Album>();
                inc.Include(a => a.Artist);

                var dbAlbum = _albumRepository.Get(inc, albumId);

                if (dbAlbum == null) return null;

                var lastFmAlbumResponse = await _lastfmClient.Album.GetInfoAsync(dbAlbum.Artist.Name, dbAlbum.Title);

                lastFmAlbum = _mapper.Map<Chinook.ViewModel.LastFm.Album>(lastFmAlbumResponse.Content);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(CACHEDSECONDS));
                _memoryCache.Set(cacheKey, lastFmAlbum, cacheEntryOptions);
            }
            return lastFmAlbum;
        }

        public void ResetAlbumCache(long albumId) {
            string cacheKey = $"_Album-{albumId}";
            _memoryCache.Remove(cacheKey);
        }
    }
}