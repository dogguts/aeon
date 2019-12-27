using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using IF.Lastfm.Core.Objects;

namespace Chinook.Mapping {
    public class LastFm : Profile {

        public class ArtistImageResolver : IValueResolver<IF.Lastfm.Core.Objects.LastArtist, ViewModel.LastFm.Artist, byte[]> {
            private readonly IHttpClientFactory _httpClientFactory;
            public ArtistImageResolver(IHttpClientFactory httpClientFactory) {
                _httpClientFactory = httpClientFactory;
            }

            public byte[] Resolve(IF.Lastfm.Core.Objects.LastArtist source, ViewModel.LastFm.Artist destination, byte[] member, ResolutionContext context) {
                Uri url = source.MainImage.Largest;
                if (url == null) return null;
                var image = Task.Run(() => _httpClientFactory.CreateClient().GetByteArrayAsync(url)).Result;
                return image;
            }
        }
        public class AlbumImageResolver : IValueResolver<IF.Lastfm.Core.Objects.LastAlbum, ViewModel.LastFm.Album, byte[]> {
            private readonly IHttpClientFactory _httpClientFactory;
            public AlbumImageResolver(IHttpClientFactory httpClientFactory) {
                _httpClientFactory = httpClientFactory;
            }

            public byte[] Resolve(IF.Lastfm.Core.Objects.LastAlbum source, ViewModel.LastFm.Album destination, byte[] member, ResolutionContext context) {
                Uri url = source.Images.Largest;
                if (url != null) {
                    var image = Task.Run(() => _httpClientFactory.CreateClient().GetByteArrayAsync(url)).Result;
                    return image;
                } else {
                    return null;
                }
            }
        }

        public LastFm() {
            // Lastfm.LastArtist -> ViewModel.LastFm.Artist 
            CreateMap<IF.Lastfm.Core.Objects.LastArtist, ViewModel.LastFm.Artist>()
              .ForMember(dst => dst.Id, o => o.Ignore())
              .ForMember(dst => dst.Image, o => o.MapFrom<ArtistImageResolver>())
              .ForMember(dst => dst.MbId, o => o.MapFrom(src => src.Mbid))
              .ForMember(dst => dst.Bio, o => o.MapFrom(src => src.Bio.Summary));


            // Lastfm.LastAlbum -> ViewModel.LastFm.Album 
            CreateMap<IF.Lastfm.Core.Objects.LastAlbum, ViewModel.LastFm.Album>()
              .ForMember(dst => dst.Summary, o => o.MapFrom(src => "//TODO: wait for WIKI property on album"))
              .ForMember(dst => dst.Id, o => o.Ignore())
              .ForMember(dst => dst.Image, o => o.MapFrom<AlbumImageResolver>())
              .ForMember(dst => dst.MbId, o => o.MapFrom(src => src.Mbid));



        }
    }
}