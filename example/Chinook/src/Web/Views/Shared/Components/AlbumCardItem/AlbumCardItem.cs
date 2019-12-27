using System;
using System.Threading.Tasks;
using Chinook.Service;
using Microsoft.AspNetCore.Mvc;

namespace Chinook.Web.Views.Shared.Components {
    public class AlbumCardItem : ViewComponent {
        public AlbumCardItem() {
        }

        public Task<IViewComponentResult> InvokeAsync(ViewModel.ListItem.AlbumListItem data, string cssClass = "") {
            return Task.FromResult<IViewComponentResult>(View((data, cssClass)));
        }
    }
}