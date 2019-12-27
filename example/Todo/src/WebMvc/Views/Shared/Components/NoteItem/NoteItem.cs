using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VM = TodoApp.ViewModel;

namespace TodoApp.WebMvc.Views.Shared.Components {

    public class NoteItem : ViewComponent {

        public NoteItem() {
        }


        public async Task<IViewComponentResult> InvokeAsync(VM.NoteItem_ListItem noteItem = null) {
            if (noteItem == null) {
                throw new ArgumentNullException(nameof(noteItem));
            }
            return await Task.FromResult(View(noteItem));
        }
    }
}