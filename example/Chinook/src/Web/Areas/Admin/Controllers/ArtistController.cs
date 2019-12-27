using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Chinook.Service.ViewModel;
using Chinook.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Web.Areas.Admin.Controllers {
    [Area("Admin")]
    [Route("[Area]/[controller]")]
    public class ArtistController : Controller {
        private readonly IArtistViewModelService _artistViewModelService;
        public ArtistController(IArtistViewModelService artistViewModelService) {
            _artistViewModelService = artistViewModelService;
        }

        [HttpGet("[Action]/{id}")]
        public async Task<IActionResult> Edit(long id) {
            var data = await _artistViewModelService.Get(id);
            if (data != null) {
                return View(nameof(Edit), data);
            } else {
                return NotFound(id);
            };
        }
        [HttpGet("[Action]")]
        public IActionResult Create() {
            var data = _artistViewModelService.Create();
            return View(nameof(Edit), data);
        }


        [HttpPost("[Action]/{id}")]
        public async Task<IActionResult> Edit(ArtistViewModel vm) {
            //    _artistViewModelService.Save(model);

            ArtistViewModel data = vm;
            if (ModelState.IsValid) {
                try {
                    data = await _artistViewModelService.Save(vm);
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