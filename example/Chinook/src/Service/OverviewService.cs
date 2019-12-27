using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using AutoMapper;
using Chinook.Model;
using VM = Chinook.ViewModel.ListItem;

namespace Chinook.Service {
    public interface IOverviewService {
        Task<IEnumerable<VM.AlbumListItem>> BestSoldAlbums(TimeSpan since);
        Task<IEnumerable<VM.TrackListItem>> BestSoldTracks(TimeSpan since);
        Task<IEnumerable<VM.ArtistListItem>> BestSoldArtists(TimeSpan since);
        Task<IEnumerable<VM.AlbumListItem>> ArtistAlbums(long artistId);
        Task<IEnumerable<VM.TrackListItem>> ArtistTopTracks(long artistId);
    }

    public class OverviewService : IOverviewService {
        private IRepository<InvoiceLine> _invoiceLineRepository;
        private IDateTimeService _dateTimeService;
        private IMapper _mapper;

        public OverviewService(IRepository<InvoiceLine> invoiceLineRepository,
                               IDateTimeService dateTimeService,
                               IMapper mapper) {
            _invoiceLineRepository = invoiceLineRepository;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
        }

        /// <summary>
        /// 'total' most popular M, since 'since' for optional 'artistId' and/or optional 'albumId' 
        /// </summary>
        private async Task<IEnumerable<M>> BestSold<M>(Func<InvoiceLine, M> select, TimeSpan since, long? artistId = null, long? albumId = null, int total = 10) where M : class {
            var fromdt = _dateTimeService.CurrentDateTime - since;

            RepositoryFilter<InvoiceLine> filter;
            if (since == TimeSpan.Zero) {
                filter = new RepositoryFilter<InvoiceLine>(null);
            } else {
                filter = new RepositoryFilter<InvoiceLine>(l => l.Invoice.InvoiceDate >= fromdt);
            }

            filter = new RepositoryFilter<InvoiceLine>(l => (since != TimeSpan.Zero ? l.Invoice.InvoiceDate >= fromdt : true) &&
                                                            ((artistId ?? l.Track.Album.Artist.ArtistId) == l.Track.Album.Artist.ArtistId) &&
                                                            ((albumId ?? l.Track.Album.AlbumId) == l.Track.Album.AlbumId));


            filter.Include(il => il.Track).ThenInclude(t => t.Album).ThenInclude(a => a.Artist);

            var (Data, Total) = await _invoiceLineRepository.GetWithFilterAsync(filter);
            var allGrouped = Data.GroupBy(select)
                            .Select(group => (Object: (M)((object)group.Key), Count: group.Count())
                        ).OrderByDescending(a => a.Count);

            return allGrouped.Take(total).Select(t => t.Object);
        }

        /// <summary>
        /// Get albums for artist sorted by most sold
        /// </summary>
        public async Task<IEnumerable<VM.AlbumListItem>> ArtistAlbums(long artistId) {
            var albums = await BestSold(x => x.Track.Album, TimeSpan.Zero, artistId: artistId, total: int.MaxValue);
            return _mapper.Map<IEnumerable<VM.AlbumListItem>>(albums);
        }
        /// <summary>
        /// Get top tracks for artist sorted by most sold
        /// </summary>
        public async Task<IEnumerable<VM.TrackListItem>> ArtistTopTracks(long artistId) {
            var tracks = await BestSold(x => x.Track, TimeSpan.Zero, artistId: artistId);
            return _mapper.Map<IEnumerable<VM.TrackListItem>>(tracks);
        }

        public async Task<IEnumerable<VM.AlbumListItem>> BestSoldAlbums(TimeSpan since) {
            var albums = await BestSold(x => x.Track.Album, since);
            return _mapper.Map<IEnumerable<VM.AlbumListItem>>(albums);
        }

        public async Task<IEnumerable<VM.ArtistListItem>> BestSoldArtists(TimeSpan since) {
            var artists = await BestSold(x => x.Track.Album.Artist, since);
            return _mapper.Map<IEnumerable<VM.ArtistListItem>>(artists);
        }

        public async Task<IEnumerable<VM.TrackListItem>> BestSoldTracks(TimeSpan since) {
            var tracks = await BestSold(x => x.Track, since);
            return _mapper.Map<IEnumerable<VM.TrackListItem>>(tracks);
        }
    }
}
