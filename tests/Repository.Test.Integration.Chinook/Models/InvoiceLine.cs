using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Repository.Model {
    public partial class InvoiceLine {
        public long InvoiceLineId { get; set; }
        public long InvoiceId { get; set; }
        public long TrackId { get; set; }
        [Required]
        [Column(TypeName = "NUMERIC(10,2)")]
        public string UnitPrice { get; set; }
        public long Quantity { get; set; }

        [ForeignKey("InvoiceId")]
        [InverseProperty("InvoiceLine")]
        public Invoice Invoice { get; set; }
        [ForeignKey("TrackId")]
        [InverseProperty("InvoiceLine")]
        public Track Track { get; set; }
    }
}
