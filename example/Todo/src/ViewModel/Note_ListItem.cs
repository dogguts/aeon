using System;
using System.Collections.Generic;

namespace TodoApp.ViewModel {
    public class Note_ListItem {
        public long? Id { get; set; }
        public Category Category { get; set; }
        /*public string CategoryIcon { get; set; }
        public string CategoryColor { get; set; }*/
        public string Title { get; set; }

        public ICollection<NoteItem_ListItem> Items { get; set; } = new List<NoteItem_ListItem>();
    }
}
