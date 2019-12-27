using System;
using AutoMapper;
using System.Linq;
using VML = Chinook.ViewModel.ListItem;
using VM = Chinook.ViewModel;

namespace Chinook.Mapping {
    public class Album : Profile {
        public Album() {
            //Album -> ListItem
            CreateMap<Model.Album, VML.AlbumListItem>()
                .ForMember(dst => dst.AlbumId, o => o.MapFrom(src => src.AlbumId))
                .ForMember(dst => dst.ArtistName, o => o.MapFrom(src => src.Artist.Name))
                .ForMember(dst => dst.Name, o => o.MapFrom(src => src.Title));


            CreateMap<Model.Album, VM.AlbumViewModel>()
                .ForMember(dst => dst.AlbumId, o => o.MapFrom(src => src.AlbumId))
                .ForMember(dst => dst.ArtistId, o => o.MapFrom(src => src.Artist.ArtistId))
                .ForMember(dst => dst.ArtistName, o => o.MapFrom(src => src.Artist.Name))
                .ForMember(dst => dst.Name, o => o.MapFrom(src => src.Title))
                .ForMember(dst => dst.Tracks, o => o.MapFrom(src => src.Track))
                .ForMember(dst => dst.MbId, o => o.Ignore())
                .ForMember(dst => dst.Summary, o => o.Ignore());

            CreateMap<Chinook.ViewModel.LastFm.Album, Chinook.ViewModel.AlbumViewModel>()
                .ForMember(dst => dst.AlbumId, o => o.Ignore())
                .ForMember(dst => dst.Name, o => o.Ignore())
                .ForMember(dst => dst.ArtistName, o => o.Ignore())
                .ForMember(dst => dst.ArtistId, o => o.Ignore())
                .ForMember(dst => dst.Tracks, o => o.Ignore());
        }
    }
}
