using System;
using System.Collections.Generic;
using System.Linq;
using Chinook.Repository.Model;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Repository {
    /// <summary>
    /// (example of an)  extended (Invoice) repository implementation
    /// </summary>
    public class InvoiceRepository<TDbContext> : Aeon.Core.Repository.Repository<Model.Invoice, TDbContext>, Infrastructure.IInvoiceRepository where TDbContext : DbContext {

        public InvoiceRepository(TDbContext context) : base(context) { }

        /// <summary>
        /// Get Invoices for specific weekday(s), sorted on those weekday(s)
        /// </summary>
        public IEnumerable<Invoice> GetByWeekDays(params DayOfWeek[] daysOfWeek) {

            var r = _dbSet.Where(p => daysOfWeek.Contains(p.InvoiceDate.DayOfWeek))
                                    .Include(p => p.InvoiceLine)
                                    .Include(p => p.Customer).ThenInclude(m => m.SupportRep)
                                    .AsEnumerable();

            return r.OrderBy(p => daysOfWeek.ToList().IndexOf(p.InvoiceDate.DayOfWeek));
        }
    }
}