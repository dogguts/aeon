using System.Threading.Tasks;
using Chinook.Service;
using Chinook.Service.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers {


    public class ArtistController : Controller {
        private const int CACHEAGE = 60 * 60 * 24;
        private readonly ILastFmService _lastFmService;
        private readonly IArtistViewModelService _artistViewModelService;
        public ArtistController(ILastFmService lastFmService,
                                IArtistViewModelService artistViewModelService) {
            _lastFmService = lastFmService;
            _artistViewModelService = artistViewModelService;
        }

        public async Task<IActionResult> Image(long id) {
            var metaArtist = await _lastFmService.GetArtist(id);

            Response.Headers["Cache-Control"] = $"public,max-age={CACHEAGE}";

            if (metaArtist?.Image == null || metaArtist.Image.Length == 0) {
                return File("~/Images/blank.jpg", "image/jpeg");
            }

            return File(metaArtist.Image, "image/jpeg");
        }


        public async Task<IActionResult> Index(long id) {
            var artistVm = await _artistViewModelService.Get(id);

            return View(artistVm);
        }
    }
}