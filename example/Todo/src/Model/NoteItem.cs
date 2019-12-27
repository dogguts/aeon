using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Model {
    [Table("NoteItem")]
    public partial class NoteItem {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long NoteItemId { get; set; }
        public long? NoteId { get; set; }
        [Required]
        public string Title { get; set; }
        public bool Completed { get; set; }

        [ForeignKey("NoteId"), Required()   ]
        [InverseProperty("NoteItems")]
        public Note Note { get; set; }
    }

}
