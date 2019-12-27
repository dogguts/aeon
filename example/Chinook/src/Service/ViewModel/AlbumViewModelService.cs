using System;
using System.Threading.Tasks;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using AutoMapper;
using Chinook.Repository.Infrastructure;
using Chinook.ViewModel;

namespace Chinook.Service.ViewModel {
    public interface IAlbumViewModelService {
        Task<AlbumViewModel> Get(long id);
        Task<AlbumViewModel> Save(AlbumViewModel vm);
        Task<AlbumViewModel> Create(long artistId);
    }
    public class AlbumViewModelService : IAlbumViewModelService {
        private readonly ILastFmService _lastFmService;
        private readonly IRepository<Model.Album> _albumRepository;
        private readonly IRepository<Model.Artist> _artistRepository;

        private readonly IMapper _mapper;
        private readonly IChinookDbUnitOfWork _chinookDbUnitOfWork;
        public AlbumViewModelService(IMapper mapper,
                                      ILastFmService lastFmService,
                                      IRepository<Model.Album> albumRepository,
                                      IRepository<Model.Artist> artistRepository) {
            _mapper = mapper;
            _lastFmService = lastFmService;
            _albumRepository = albumRepository;
            _artistRepository = artistRepository;
        }

        public async Task<AlbumViewModel> Create(long artistId) {
            var artist = await _artistRepository.GetAsync(artistId);

            return new AlbumViewModel() {
                Name = "[New album]",
                ArtistId = artist.ArtistId,
                ArtistName = artist.Name
            };
        }

        public async Task<AlbumViewModel> Get(long id) {
            var dbAlbumIncludes = new RepositoryInclude<Model.Album>();
            dbAlbumIncludes.Include(a => a.Track).ThenInclude(t => t.Genre);
            dbAlbumIncludes.Include(a => a.Artist);

            var dbAlbum = await _albumRepository.GetAsync(dbAlbumIncludes, id);
            var lastFmAlbum = await _lastFmService.GetAlbum(id);
            //var t = await _overviewService.ArtistTopTracks(id);

            var vmAlbum = _mapper.Map<Chinook.ViewModel.AlbumViewModel>(dbAlbum);
            _mapper.Map(lastFmAlbum, vmAlbum);

            return vmAlbum;
        }

        public async Task<AlbumViewModel> Save(AlbumViewModel vm) {
            Model.Album albumDb;
            if (vm.AlbumId != 0) {
                //get entity
                albumDb = _albumRepository.Get(vm.AlbumId);
                //update entity
                _mapper.Map(vm, albumDb);
            } else {
                //insert entity
                albumDb = _mapper.Map<Model.Album>(vm);
                _albumRepository.Add(albumDb);
            }

            //commit changes
            _chinookDbUnitOfWork.Commit();
            //reset last.fm cache for artist
            _lastFmService.ResetAlbumCache(vm.AlbumId);
            return await Get(albumDb.ArtistId);
        }


    }
}