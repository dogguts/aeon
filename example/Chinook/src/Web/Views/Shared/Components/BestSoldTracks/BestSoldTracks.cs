using System;
using System.Threading.Tasks;
using Chinook.Service;
using Microsoft.AspNetCore.Mvc;

namespace Chinook.Web.Views.Shared.Components {
    public class BestSoldTracks : ViewComponent {
        private readonly IOverviewService _overviewService;
        public BestSoldTracks(IOverviewService overviewService) {
            _overviewService = overviewService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int timeframe) {
            var data = await _overviewService.BestSoldTracks(TimeSpan.FromDays(timeframe));
            return View(data);
        }
    }
}