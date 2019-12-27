using System;
using System.Threading.Tasks;
using Chinook.Service;
using Microsoft.AspNetCore.Mvc;

namespace Chinook.Web.Views.Shared.Components {
    public class AlbumListItem : ViewComponent {
        public AlbumListItem() {
        }

        public Task<IViewComponentResult> InvokeAsync(ViewModel.ListItem.AlbumListItem data) {
            return Task.FromResult<IViewComponentResult>(View(data));
        }
    }
}