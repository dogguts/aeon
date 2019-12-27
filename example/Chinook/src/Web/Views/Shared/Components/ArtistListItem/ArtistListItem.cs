using System;
using System.Threading.Tasks;
using Chinook.Service;
using Microsoft.AspNetCore.Mvc;

namespace Chinook.Web.Views.Shared.Components {
    public class ArtistListItem : ViewComponent {
        public ArtistListItem() {
        }

        public Task<IViewComponentResult> InvokeAsync(ViewModel.ListItem.ArtistListItem data) {
            return Task.FromResult<IViewComponentResult>(View(data));
        }
    }
}