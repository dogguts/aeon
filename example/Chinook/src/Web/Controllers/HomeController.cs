using System;
using System.Threading.Tasks;
using Chinook.Service.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Chinook.Web.Controllers {
    public class HomeController : Controller {

        //private readonly Service.ILastFmService _lastFmService;
 
        public HomeController( ) {
            
        }

        public async Task<IActionResult> Index() {
            return View();
        }



        public IActionResult BestSoldAlbums(int timeframe) {
            return ViewComponent(nameof(Web.Views.Shared.Components.BestSoldAlbums), new { timeframe });
        }
        public IActionResult BestSoldTracks(int timeframe) {
            return ViewComponent(nameof(Web.Views.Shared.Components.BestSoldTracks), new { timeframe });
        }
        public IActionResult BestSoldArtists(int timeframe) {
            return ViewComponent(nameof(Web.Views.Shared.Components.BestSoldArtists), new { timeframe });
        }
    }
}