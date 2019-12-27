using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using VM = TodoApp.ViewModel;
using System.Drawing;

namespace TodoApp.Mapping {
    public class NoteProfile : Profile {

        public class ColorConverter : IValueConverter<Color, string>, IValueConverter<string, Color> {
            public string Convert(Color c, ResolutionContext context) => $"{c.R:X2}{c.G:X2}{c.B:X2}";

            public Color Convert(string sourceMember, ResolutionContext context) {
                var c = sourceMember?.TrimStart('#')?.Trim();
                return Color.FromArgb(System.Convert.ToInt32(c, 16));
            }
        }

        public NoteProfile() {
            CreateMap<Model.NoteItem, VM.NoteItem_ListItem>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.NoteItemId))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Completed, opt => opt.MapFrom(src => src.Completed));

            CreateMap<VM.NoteItem_ListItem, Model.NoteItem>()
                    .ForMember(dst => dst.NoteItemId, opt => {
                        opt.Condition(src => src.Id.HasValue);
                        opt.MapFrom(src => src.Id);
                    })
                    .ForMember(dst => dst.Title, opt => {
                        opt.Condition(src => src.Title != null);
                        opt.MapFrom(src => src.Title);
                    })
                    .ForMember(dst => dst.Completed, opt => {
                        opt.Condition(src => src.Completed.HasValue);
                        opt.MapFrom(src => src.Completed);
                    })
                    .ForMember(dst => dst.Note, opt => opt.Ignore()) //TODO: needs mapping? yes, when creating
                    .ForMember(dst => dst.NoteId, opt => opt.Ignore());   //TODO: needs mapping? no, don't set id's

            CreateMap<Model.Category, VM.Category>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dst => dst.Color, opt => opt.ConvertUsing<ColorConverter, Color>(s => s.Color))
                .ForMember(dst => dst.Icon, opt => opt.MapFrom(src => src.FaIcon))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title));

            CreateMap<VM.Category, Model.Category>()
                .ForMember(dst => dst.CategoryId, opt => {
                    opt.Condition(src => src.Id.HasValue);
                    opt.MapFrom(src => src.Id);
                })
                .ForMember(dst => dst.Color, opt => {
                    opt.Condition(src => src.Color != null);
                    opt.MapFrom(src => src.Color);
                })
                .ForMember(dst => dst.FaIcon, opt => {
                    opt.Condition(src => src.Icon != null);
                    opt.MapFrom(src => src.Icon);
                })
                .ForMember(dst => dst.FaIcon, opt => {
                    opt.Condition(src => src.Title != null);
                    opt.MapFrom(src => src.Title);
                })
                .ForMember(dst => dst.Deleted, opt => {
                    opt.Condition(src => src.Deleted.HasValue);
                    opt.MapFrom(src => src.Deleted);
                });

            CreateMap<Model.Note, VM.Note_ListItem>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.NoteId))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dst => dst.Items, opt => opt.MapFrom(src => src.NoteItems));



            CreateMap<VM.Note_ListItem, Model.Note>()
                .ForMember(dst => dst.NoteId, opt => {
                    opt.Condition(src => src.Id.HasValue);
                    opt.MapFrom(src => src.Id);
                })
                .ForMember(dst => dst.Title, opt => {
                    opt.Condition(src => src.Title != null);
                    opt.MapFrom(src => src.Title);
                })
                .ForMember(dst => dst.CategoryId, opt => {
                    opt.Condition(src => src.Category?.Id != null);
                    opt.MapFrom(src => src.Category.Id.Value);
                })
                .ForMember(dst => dst.NoteItems, opt => opt.Ignore())
                .ForMember(dst => dst.Category, opt => opt.Ignore());


            //System.Collections.Generic.List`1[[TodoApp.Model.Note, Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]] 
            // -> System.Collections.Generic.IEnumerable`1[[TodoApp.ViewModel.NoteItem_ListItem, ViewModel, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]

            //CreateMap<IEnumerable<Model.Note>, List<ViewModel.NoteItem_ListItem>>();

        }
    }
}