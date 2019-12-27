using System;
using AutoMapper;
using System.Linq;
using VML = Chinook.ViewModel.ListItem;
using Microsoft.AspNetCore.Mvc;
using Chinook.Dto;
using Chinook.Model;

namespace Chinook.Mapping {
    public class SearchResult : Profile {

        public abstract class UrlResolver<T> : IValueResolver<T, Dto.SearchResult, string> {
            protected readonly IUrlHelper _urlHelper;
            public UrlResolver(IUrlHelper urlHelper) {
                _urlHelper = urlHelper;
            }
            protected virtual string Action => "";
            protected virtual string Controller => typeof(T).Name;


            public string Resolve(long id) {
                return _urlHelper.Action(Action, Controller, new { id });
            }

            public abstract string Resolve(T source, Dto.SearchResult destination, string destMember, ResolutionContext context);
        }

        public class ArtistUrlResolver : UrlResolver<Model.Artist> {
            public ArtistUrlResolver(IUrlHelper urlHelper) : base(urlHelper) { }

            public override string Resolve(Model.Artist source, Dto.SearchResult destination, string destMember, ResolutionContext context) {
                return Resolve(source.ArtistId);
            }
        }

        public class AlbumUrlResolver : UrlResolver<Model.Album> {
            public AlbumUrlResolver(IUrlHelper urlHelper) : base(urlHelper) { }

            public override string Resolve(Model.Album source, Dto.SearchResult destination, string destMember, ResolutionContext context) {
                return Resolve(source.AlbumId);
            }
        }

        public class TrackUrlResolver : UrlResolver<Model.Track> {
            public TrackUrlResolver(IUrlHelper urlHelper) : base(urlHelper) { }

            public override string Resolve(Model.Track source, Dto.SearchResult destination, string destMember, ResolutionContext context) {
                return Resolve(source.TrackId);
            }
        }

        public class ArtistImageUrlResolver : UrlResolver<Model.Artist> {
            public ArtistImageUrlResolver(IUrlHelper urlHelper) : base(urlHelper) { }
            protected override string Action => "Image";
            public override string Resolve(Model.Artist source, Dto.SearchResult destination, string destMember, ResolutionContext context) {
                return Resolve(source.ArtistId);
            }
        }

        public class AlbumImageUrlResolver : UrlResolver<Model.Album> {
            public AlbumImageUrlResolver(IUrlHelper urlHelper) : base(urlHelper) { }
            protected override string Action => "Image";
            public override string Resolve(Model.Album source, Dto.SearchResult destination, string destMember, ResolutionContext context) {
                return Resolve(source.AlbumId);
            }
        }

        public class TrackImageUrlResolver : UrlResolver<Model.Track> {
            public TrackImageUrlResolver(IUrlHelper urlHelper) : base(urlHelper) { }
            protected override string Action => "Image";
            protected override string Controller => nameof(Model.Album);
            public override string Resolve(Model.Track source, Dto.SearchResult destination, string destMember, ResolutionContext context) {
                return Resolve(source.Album.AlbumId);
            }
        }

        public SearchResult() {
            //Artist -> SearchResult
            CreateMap<Model.Artist, Dto.SearchResult>()
                .ForMember(dst => dst.Id, o => o.MapFrom(src => src.ArtistId))
                .ForMember(dst => dst.Title, o => o.MapFrom(src => src.Name))
                .ForMember(dst => dst.SubTitle, o => o.MapFrom(src => nameof(Dto.SearchResultType.Artist)))
                .ForMember(dst => dst.Type, o => o.MapFrom(src => Dto.SearchResultType.Artist))
                .ForMember(dst => dst.ImageUrl, o => o.MapFrom<ArtistImageUrlResolver>())
                .ForMember(dst => dst.Url, o => o.MapFrom<ArtistUrlResolver>());

            CreateMap<Model.Album, Dto.SearchResult>()
                .ForMember(dst => dst.Id, o => o.MapFrom(src => src.AlbumId))
                .ForMember(dst => dst.Title, o => o.MapFrom(src => src.Title))
                .ForMember(dst => dst.SubTitle, o => o.MapFrom(src => $"Album by {src.Artist.Name}"))
                .ForMember(dst => dst.Type, o => o.MapFrom(src => Dto.SearchResultType.Album))
                .ForMember(dst => dst.ImageUrl, o => o.MapFrom<AlbumImageUrlResolver>())
                .ForMember(dst => dst.Url, o => o.MapFrom<AlbumUrlResolver>());

            CreateMap<Model.Track, Dto.SearchResult>()
                .ForMember(dst => dst.Id, o => o.MapFrom(src => src.TrackId))
                .ForMember(dst => dst.Title, o => o.MapFrom(src => src.Name))
                .ForMember(dst => dst.SubTitle, o => o.MapFrom(src => $"Track by {src.Album.Artist.Name}"))
                .ForMember(dst => dst.Type, o => o.MapFrom(src => Dto.SearchResultType.Track))
                .ForMember(dst => dst.ImageUrl, o => o.MapFrom<TrackImageUrlResolver>())
                .ForMember(dst => dst.Url, o => o.MapFrom<TrackUrlResolver>());

        }
    }
}

