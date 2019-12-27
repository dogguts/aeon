using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApp.Service.Infrastructure {
    public interface INoteService {
        Task<IEnumerable<ViewModel.Note_ListItem>> GetNotes();

        Task<ViewModel.Note_ListItem> AddNote(ViewModel.Note_ListItem note);
        Task<ViewModel.Note_ListItem> EditNote(ViewModel.Note_ListItem item);
        Task<bool> DeleteNote(ViewModel.Note_ListItem note);

        Task<ViewModel.NoteItem_ListItem> AddItem(long noteId, ViewModel.NoteItem_ListItem item);
        Task<ViewModel.NoteItem_ListItem> EditItem(ViewModel.NoteItem_ListItem item);
        Task<bool> DeleteItem(ViewModel.NoteItem_ListItem item);

    }
}