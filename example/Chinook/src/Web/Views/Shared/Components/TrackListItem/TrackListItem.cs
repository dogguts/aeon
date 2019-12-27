using System;
using System.Threading.Tasks;
using Chinook.Service;
using Microsoft.AspNetCore.Mvc;

namespace Chinook.Web.Views.Shared.Components {
    public class TrackListItem : ViewComponent {
        public TrackListItem() {
        }

        public Task<IViewComponentResult> InvokeAsync(ViewModel.ListItem.TrackListItem data) {
            return Task.FromResult<IViewComponentResult>(View(data));
        }
    }
}
