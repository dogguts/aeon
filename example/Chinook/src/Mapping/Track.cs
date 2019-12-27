using System;
using AutoMapper;
using System.Linq;
using VML = Chinook.ViewModel.ListItem;

namespace Chinook.Mapping {
    public class Track : Profile {
        public Track() {
            //Track -> ListItem 
            CreateMap<Model.Track, VML.TrackListItem>()
                   .ForMember(dst => dst.TrackId, o => o.MapFrom(src => src.TrackId))
                   .ForMember(dst => dst.AlbumName, o => o.MapFrom(src => src.Album.Title))
                   .ForMember(dst => dst.AlbumId, o => o.MapFrom(src => src.Album.AlbumId))
                   .ForMember(dst => dst.ArtistName, o => o.MapFrom(src => src.Album.Artist.Name))
                   .ForMember(dst => dst.GenreName, o => o.MapFrom(src => src.Genre.Name))
                   .ForMember(dst => dst.Duration, o => o.MapFrom(src => TimeSpan.FromMilliseconds(src.Milliseconds)))
                   .ForMember(dst => dst.Name, o => o.MapFrom(src => src.Name));

        }
    }
}
