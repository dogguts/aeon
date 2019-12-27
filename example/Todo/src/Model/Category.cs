using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace TodoApp.Model {

    public partial class Category {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long CategoryId { get; set; }
        public string Title { get; set; }
        public string FaIcon { get; set; }
        public Color Color { get; set; } = Color.White;

    }
}