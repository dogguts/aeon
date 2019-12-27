using System;
using System.Collections.Generic;

namespace TodoApp.ViewModel {

    public class Category {
        public long? Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
        public bool? Deleted { get; set; }
    }
}