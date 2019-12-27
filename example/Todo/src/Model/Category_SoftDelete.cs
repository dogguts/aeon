using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace TodoApp.Model {
    public partial class Category : Infrastructure.ISoftDelete {
        
        public bool Deleted { get; set; }
    }
}