using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Chinook.Service.ViewModel;
using Chinook.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers {
    [Area("Admin")]
    [Route("[Area]/[controller]")]
    public class AlbumController : Controller {
        private readonly IAlbumViewModelService _albumViewModelService;
        public AlbumController(IAlbumViewModelService albumViewModelService) {
            _albumViewModelService = albumViewModelService;
        }

        [HttpGet("[Action]/{id}")]
        public async Task<IActionResult> Edit(long id) {
            var data = await _albumViewModelService.Get(id);
            if (data != null) {
                return View(nameof(Edit), data);
            } else {
                return NotFound(id);
            };
        }
        [HttpGet("[Action]")]
        public async Task<IActionResult> Create(long artistId) {
            var data = await _albumViewModelService.Create(artistId);
            return View(nameof(Edit), data);
        }


        [HttpPost("[Action]/{id}")]
        public async Task<IActionResult> Edit(AlbumViewModel vm) {
            //    _artistViewModelService.Save(model);

            AlbumViewModel data = vm;
            if (ModelState.IsValid) {
                try {
                    data = await _albumViewModelService.Save(vm);
                } catch (ValidationException vex) {
                    // validatilonexception bubbled up, add error to modelstate 
                    ModelState.AddModelError(vex.ValidationResult.MemberNames.FirstOrDefault(), vex.ValidationResult.ErrorMessage);
                }
            }
            //viewmodel/model committed successfully, return to detail view 
            return View(nameof(Edit), data);
        }


    }
}