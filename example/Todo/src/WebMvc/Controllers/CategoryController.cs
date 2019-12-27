using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Service.Infrastructure;

namespace TodoApp.WebMvc.Controllers {

    [Route("[Controller]")]
    public class CategoryController : Controller {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService) {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index() {
            var data = await _categoryService.GetCategories(true);
            return View(data);
        }


        [HttpGet("[Action]/{id}")]
        public async Task<IActionResult> Edit(long id) {
            var data = await _categoryService.Get(id);
            return View(data);

            // return ViewComponent(typeof(TodoApp.WebMvc.Views.Shared.Components.NoteItem.NoteItem), new { noteItem = data });
        }

        [HttpPost("[Action]/{id}")]
        public async Task<IActionResult> Delete(long id) {
            return View(null);
        }

    }
}