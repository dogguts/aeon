using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Model {
    public partial class Note {
        public Note() {
            NoteItems = new HashSet<NoteItem>();
        }
        [Column("NoteId"), Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long NoteId { get; set; }

        public long CategoryId { get; set; }

        [Required]
        public string Title { get; set; }

        public Category Category { get; set; }

        [InverseProperty("Note")]
        public ICollection<NoteItem> NoteItems { get; set; }
    }

}