using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using AutoMapper;
using Chinook.Dto;
using Chinook.Model;

namespace Chinook.Service {
    public interface IGlobalSearchService {
        Task<IEnumerable<Dto.SearchResult>> Search(string searchString);
        Task<IEnumerable<SearchResult>> SearchArtist(string searchString);
    }

    public class TextFilter<T> {
        public TextFilter() { }
    }

    static class TextFilterSpecification {
        public static (IRepositoryFilter<Artist> Filter, RepositorySort<Artist> Sort) SearchFor(this TextFilter<Artist> cls, string searchString) {
            var filter = new RepositoryFilter<Artist>(p => p.Name.ToLower().Contains(searchString.ToLower()));
            var sort = new RepositorySort<Artist>((ListSortDirection.Ascending, a => a.Name));
            return (filter, sort);
        }
        public static (IRepositoryFilter<Album> Filter, RepositorySort<Album> Sort) SearchFor(this TextFilter<Album> cls, string searchString) {
            var inc = new RepositoryInclude<Album>();
            inc.Include(a => a.Artist);
            var filter = new RepositoryFilter<Album>(p => p.Title.ToLower().Contains(searchString.ToLower()), inc);
            var sort = new RepositorySort<Album>((ListSortDirection.Ascending, a => a.Title));
            return (filter, sort);
        }
        public static (IRepositoryFilter<Track> Filter, RepositorySort<Track> Sort) SearchFor(this TextFilter<Track> cls, string searchString) {
            var inc = new RepositoryInclude<Track>();
            inc.Include(t => t.Album).ThenInclude(a => a.Artist);
            var filter = new RepositoryFilter<Track>(p => p.Name.ToLower().Contains(searchString.ToLower()), inc);
            var sort = new RepositorySort<Track>((ListSortDirection.Ascending, a => a.Name));
            return (filter, sort);
        }
    }

    public class GlobalSearchService : IGlobalSearchService {
        private readonly IRepository<Artist> _artistRepository;
        private readonly IRepository<Album> _albumRepository;
        private readonly IRepository<Track> _trackRepository;
        private readonly IMapper _mapper;
        public GlobalSearchService(IMapper mapper,
                                   IRepository<Artist> artistRepository,
                                   IRepository<Album> albumRepository,
                                   IRepository<Track> trackRepository) {
            _mapper = mapper;
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _trackRepository = trackRepository;
        }

        public async Task<IEnumerable<SearchResult>> SearchArtist(string searchString) {
            //NOTE: await each operation here, EF Context has scoped lifetime, beware with async/await and concurrency
            var paging = (1, 5);
            var artistResult = await _artistRepository.GetWithFilterAsync(new TextFilter<Artist>().SearchFor(searchString), paging);

            return _mapper.Map<IEnumerable<Dto.SearchResult>>((artistResult).Data);
        }

        public async Task<IEnumerable<SearchResult>> Search(string searchString) {
            //NOTE: await each operation here, EF Context has scoped lifetime, beware with async/await and concurrency
            var paging = (1, 5);
            var artistResult = await _artistRepository.GetWithFilterAsync(new TextFilter<Artist>().SearchFor(searchString), paging);
            var albumResult = await _albumRepository.GetWithFilterAsync(new TextFilter<Album>().SearchFor(searchString), paging);
            var trackResult = await _trackRepository.GetWithFilterAsync(new TextFilter<Track>().SearchFor(searchString), paging);

            var searchResults = new List<SearchResult>();
            searchResults.AddRange(_mapper.Map<IEnumerable<Dto.SearchResult>>((artistResult).Data));
            searchResults.AddRange(_mapper.Map<IEnumerable<Dto.SearchResult>>((albumResult).Data));
            searchResults.AddRange(_mapper.Map<IEnumerable<Dto.SearchResult>>((trackResult).Data));

            return searchResults;
        }
    }
}