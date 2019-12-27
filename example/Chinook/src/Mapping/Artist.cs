using System;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using VML = Chinook.ViewModel.ListItem;
using VM = Chinook.ViewModel;

namespace Chinook.Mapping {
    public class Artist : Profile {
        public Artist() {
            //Artist -> ListItem 
            CreateMap<Model.Artist, VML.ArtistListItem>()
                .ForMember(dst => dst.ArtistId, o => o.MapFrom(src => src.ArtistId))
                .ForMember(dst => dst.Name, o => o.MapFrom(src => src.Name));

            //Artist -> ArtistViewModel
            CreateMap<Model.Artist, VM.ArtistViewModel>()
                .ForMember(dst => dst.ArtistId, o => o.MapFrom(src => src.ArtistId))
                .ForMember(dst => dst.MbId, o => o.Ignore())
                .ForMember(dst => dst.Name, o => o.MapFrom(src => src.Name))
                .ForMember(dst => dst.Bio, o => o.Ignore())
                .ForMember(dst => dst.Albums, o => o.Ignore())
                .ForMember(dst => dst.TopTracks, o => o.Ignore());

            // ArtistViewModel -> Artist
            CreateMap<VM.ArtistViewModel, Model.Artist>()
                .ForMember(dst => dst.ArtistId, o => o.MapFrom(src => src.ArtistId))
                .ForMember(dst => dst.Name, o => o.MapFrom(src => src.Name))
                .ForMember(dst => dst.Album, o => o.Ignore());

            //LastFmArtist -> ArtistViewModel
            CreateMap<ViewModel.LastFm.Artist, VM.ArtistViewModel>()
                .ForMember(dst => dst.MbId, o => o.MapFrom(src => src.MbId))
                .ForMember(dst => dst.Bio, o => o.MapFrom(src => src.Bio))
                .ForMember(dst => dst.Name, o => o.Ignore())
                .ForMember(dst => dst.ArtistId, o => o.Ignore())
                .ForMember(dst => dst.Albums, o => o.Ignore())
                .ForMember(dst => dst.TopTracks, o => o.Ignore());

            //TrackListItems -> ArtistViewModel
            CreateMap<IEnumerable<VML.TrackListItem>, VM.ArtistViewModel>()
                .ForMember(dst => dst.TopTracks, o => o.MapFrom(src => src))
                .ForMember(dst => dst.MbId, o => o.Ignore())
                .ForMember(dst => dst.Bio, o => o.Ignore())
                .ForMember(dst => dst.Name, o => o.Ignore())
                .ForMember(dst => dst.ArtistId, o => o.Ignore())
                .ForMember(dst => dst.Albums, o => o.Ignore());

            //Albums -> ArtistViewModel
            CreateMap<IEnumerable<VML.AlbumListItem>, VM.ArtistViewModel>()
                           .ForMember(dst => dst.Albums, o => o.MapFrom(src => src))
                .ForMember(dst => dst.TopTracks, o => o.Ignore())
                                .ForMember(dst => dst.MbId, o => o.Ignore())
                .ForMember(dst => dst.Bio, o => o.Ignore())
                .ForMember(dst => dst.Name, o => o.Ignore())
                .ForMember(dst => dst.ArtistId, o => o.Ignore());



        }
    }
}



