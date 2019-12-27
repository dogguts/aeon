using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VM = TodoApp.ViewModel;

namespace TodoApp.WebMvc.Views.Shared.Components {

    public class Note : ViewComponent {

        public Note() {
        }


        public async Task<IViewComponentResult> InvokeAsync(VM.Note_ListItem note = null) {
            if (note == null) {
                throw new ArgumentNullException(nameof(note));
            }
            return await Task.FromResult(View(note));
        }
    }
}
