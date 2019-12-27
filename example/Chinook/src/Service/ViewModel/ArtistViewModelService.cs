using System;
using System.Threading.Tasks;
using Aeon.Core.Repository.Infrastructure;
using AutoMapper;
using Chinook.Repository.Infrastructure;
using Chinook.ViewModel;

namespace Chinook.Service.ViewModel {
    public interface IArtistViewModelService {
        Task<ArtistViewModel> Get(long id);
        Task<ArtistViewModel> Save(ArtistViewModel vm);
        Task<ArtistViewModel> Create();
    }
    public class ArtistViewModelService : IArtistViewModelService {
        private readonly ILastFmService _lastFmService;
        private readonly IRepository<Model.Artist> _artistRepository;
        private readonly IOverviewService _overviewService;
        private readonly IMapper _mapper;
        private readonly IChinookDbUnitOfWork _chinookDbUnitOfWork;
        public ArtistViewModelService(IMapper mapper,
                                      IChinookDbUnitOfWork chinookDbUnitOfWork,
                                      ILastFmService lastFmService,
                                      IOverviewService overviewService,
                                      IRepository<Model.Artist> artistRepository) {
            _mapper = mapper;
            _chinookDbUnitOfWork = chinookDbUnitOfWork;
            _lastFmService = lastFmService;
            _overviewService = overviewService;
            _artistRepository = artistRepository;
        }

        public Task<ArtistViewModel> Create() {
            return Task.FromResult(new ArtistViewModel());
        }

        public async Task<ArtistViewModel> Get(long id) {
            var dbArtist = await _artistRepository.GetAsync(id);
            var lastFmArtist = await _lastFmService.GetArtist(id);
            var albums = await _overviewService.ArtistAlbums(id);
            var topTracks = await _overviewService.ArtistTopTracks(id);

            var vmArtist = _mapper.Map<Chinook.ViewModel.ArtistViewModel>(dbArtist);
            _mapper.Map(lastFmArtist, vmArtist);
            _mapper.Map(albums, vmArtist);
            _mapper.Map(topTracks, vmArtist);

            return vmArtist;
        }

        public async Task<ArtistViewModel> Save(ArtistViewModel vm) {
            Model.Artist artistDb;
            if (vm.ArtistId != 0) {
                //get entity
                artistDb = _artistRepository.Get(vm.ArtistId);
                //update entity
                _mapper.Map(vm, artistDb);
            } else {
                //insert entity
                artistDb = _mapper.Map<Model.Artist>(vm);
                _artistRepository.Add(artistDb);
            }

            //commit changes
            _chinookDbUnitOfWork.Commit();
            //reset last.fm cache for artist
            _lastFmService.ResetArtistCache(vm.ArtistId);
            return await Get(artistDb.ArtistId);
        }
    }
}