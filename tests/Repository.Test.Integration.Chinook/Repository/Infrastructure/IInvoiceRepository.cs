using System;
using System.Collections.Generic;
using Aeon.Core.Repository.Infrastructure;

namespace Chinook.Repository.Infrastructure {
    /// <summary>
    /// (example of an) extended repository (Invoice) interface/definition 
    /// </summary>
    public interface IInvoiceRepository : IRepository<Model.Invoice> {

        IEnumerable<Model.Invoice> GetByWeekDays(params DayOfWeek[] dayOfWeek);

    }
}