using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Chinook.Service;
using Chinook.Service.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers {

    public class AlbumController : Controller {
        private const int CACHEAGE = 60 * 60 * 24;
        private readonly ILastFmService _lastFmService;
        private readonly IAlbumViewModelService _albumViewModelService;
        public AlbumController(ILastFmService lastFmService,
                               IAlbumViewModelService albumViewModelService) {
            _lastFmService = lastFmService;
            _albumViewModelService = albumViewModelService;
        }

        public async Task<IActionResult> Image(long id) {
            var metaAlbum = await _lastFmService.GetAlbum(id);

            Response.Headers["Cache-Control"] = $"public,max-age={CACHEAGE}";
            
            if (metaAlbum?.Image == null || metaAlbum.Image.Length == 0) {
                return File("~/Images/blank.jpg", "image/jpeg");
            }

            return File(metaAlbum.Image, "image/jpeg");
        }

        public async Task<IActionResult> Index(long id) {
            var albumVm = await _albumViewModelService.Get(id);

            return View(albumVm);
        }

    }
}