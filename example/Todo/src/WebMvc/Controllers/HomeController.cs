using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMvc.Models;
using TodoApp.Service.Infrastructure;
using VM = TodoApp.ViewModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TodoApp.WebMvc.Controllers {

    /// <summary>
    /// Specifies that a parameter or property should be bound first using the route-data from the current request and than the form-data in the request body.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class FromRouteAndFormAttribute : Attribute, IBindingSourceMetadata {
        private static readonly BindingSource _compositeSource = CompositeBindingSource.Create(
                new[] { BindingSource.Path, BindingSource.Form },
                nameof(FromRouteAndFormAttribute));

        /// <inheritdoc />
        public BindingSource BindingSource => _compositeSource;
    }

    public class HomeController : Controller {
        private readonly INoteService _noteService;
        private readonly ICategoryService _categoryService;

        public HomeController(INoteService NoteService, ICategoryService categoryService) {
            _categoryService = categoryService;
            _noteService = NoteService;
        }

        public async Task<IActionResult> Index() {
            var data = await _noteService.GetNotes();
            return View(data);
        }



        [ActionName("Categories")]
        [HttpGet("[Action]")]
        public async Task<IActionResult> GetCategories() {
            var data = await _categoryService.GetCategories();
            // http://bootboxjs.com/documentation.html#prompt-option-input-options
            var returnData = data.Select(c => new { text = c.Title, value = c.Id }).ToArray();
            return new JsonResult(returnData);
        }

        /// <summary>
        /// Create new note-item
        /// </summary>
        [ActionName("Item")]
        [HttpPost("[Action]/{noteId}")]
        public async Task<IActionResult> CreateItem([FromRoute] long noteId, [FromForm] ViewModel.NoteItem_ListItem noteItem) {
            var data = await _noteService.AddItem(noteId, noteItem);
            return ViewComponent(typeof(TodoApp.WebMvc.Views.Shared.Components.NoteItem), new { noteItem = data });

            // var data = await _noteService.EditItem(noteItem);
            // return ViewComponent(typeof(TodoApp.WebMvc.Views.Shared.Components.NoteItem.NoteItem), new { noteItem = data });
        }

        /// <summary>
        /// Update existing note-item
        /// </summary>
        [ActionName("Item")]
        [HttpPatch("[Action]/{id}")]
        public async Task<IActionResult> EditItem([FromRouteAndForm] ViewModel.NoteItem_ListItem noteItem) {
            var data = await _noteService.EditItem(noteItem);
            return ViewComponent(typeof(TodoApp.WebMvc.Views.Shared.Components.NoteItem), new { noteItem = data });
        }

        /// <summary>
        /// Delete existing note-item
        /// </summary>
        [ActionName("Item")]
        [HttpDelete("[Action]/{id}")]
        public async Task<IActionResult> DeleteItem([FromRoute] ViewModel.NoteItem_ListItem noteItem) {
            var data = await _noteService.DeleteItem(noteItem);
            return new JsonResult(data);
        }

        [ActionName("Note")]
        [HttpPost("[Action]")]
        public async Task<IActionResult> CreateNote([FromForm] ViewModel.Note_ListItem note) {
            var data = await _noteService.AddNote(note);
            return ViewComponent(typeof(TodoApp.WebMvc.Views.Shared.Components.Note), new { note = data });
        }

        /// <summary>
        /// Update existing note-item
        /// </summary>
        [ActionName("Note")]
        [HttpPatch("[Action]/{id}")]
        public async Task<IActionResult> EditNote([FromRouteAndForm] ViewModel.Note_ListItem note) {
            var data = await _noteService.EditNote(note);
            return ViewComponent(typeof(TodoApp.WebMvc.Views.Shared.Components.Note), new { note = data });
        }

        /// <summary>
        /// Delete existing note
        /// </summary>
        [ActionName("Note")]
        [HttpDelete("[Action]/{id}")]
        public async Task<IActionResult> DeleteNote([FromRoute] ViewModel.Note_ListItem noteItem) {
            var data = await _noteService.DeleteNote(noteItem);
            return new JsonResult(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
