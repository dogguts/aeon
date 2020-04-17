using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chinook.Repository.Model {
    public partial class Employee {
        public Employee() {
            Customer = new HashSet<Customer>();
            InverseReportsToNavigation = new HashSet<Employee>();
        }

        public long EmployeeId { get; set; }
        [Required]
        [Column(TypeName = "NVARCHAR(20)")]
        public string LastName { get; set; }
        [Required]
        [Column(TypeName = "NVARCHAR(20)")]
        public string FirstName { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string Title { get; set; }
        public long? ReportsTo { get; set; }
        [Column(TypeName = "DATETIME")]
        public string BirthDate { get; set; }
        [Column(TypeName = "DATETIME")]
        public string HireDate { get; set; }
        [Column(TypeName = "NVARCHAR(70)")]
        public string Address { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string City { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string State { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string Country { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string PostalCode { get; set; }
        [Column(TypeName = "NVARCHAR(24)")]
        public string Phone { get; set; }
        [Column(TypeName = "NVARCHAR(24)")]
        public string Fax { get; set; }
        [Column(TypeName = "NVARCHAR(60)")]
        public string Email { get; set; }

        [ForeignKey("ReportsTo")]
        [InverseProperty("InverseReportsToNavigation")]
        public Employee ReportsToNavigation { get; set; }
        [InverseProperty("SupportRep")]
        public ICollection<Customer> Customer { get; set; }
        [InverseProperty("ReportsToNavigation")]
        public ICollection<Employee> InverseReportsToNavigation { get; set; }
    }
}
