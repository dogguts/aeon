using System;
using System.Linq;
using Aeon.Core.Repository.Infrastructure;
using Model = Chinook.Repository.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Aeon.Core.Repository;
using System.ComponentModel;
using Chinook.Repository.Infrastructure;
using System.Collections.Generic;

namespace Chinook.Repository.Integration.Tests {
    public class ExtendedRepositoryTest : IClassFixture<ExtendedRepositorySetup> {
        private readonly IInvoiceRepository _invoiceRepository;
        //IInvoiceRepository, InvoiceRepository<ChinookDbContext>>();


        public ExtendedRepositoryTest(ExtendedRepositorySetup serviceSetup) {
            _invoiceRepository = serviceSetup.ServiceProvider.GetRequiredService<IInvoiceRepository>();
        }
        /// <summary>
        /// Extended repository get invoices for a specific weekday
        /// </summary>
        [Theory]
        [InlineData(DayOfWeek.Monday)]
        [InlineData(DayOfWeek.Friday)]
        public void InvoicesByWeekday(DayOfWeek dayOfWeek) {
            var results = _invoiceRepository.GetByWeekDays(dayOfWeek).ToList();

            Assert.All(results, r => Assert.Equal(r.InvoiceDate.DayOfWeek, dayOfWeek));

        }

        /// <summary>
        /// Extended repository get invoices for specific weekdays
        /// </summary>
        [Theory]
        [InlineData(DayOfWeek.Tuesday, DayOfWeek.Wednesday)]
        public void InvoicesByWeekdays(DayOfWeek dayOfWeek1, DayOfWeek dayOfWeek2) {

            var results = _invoiceRepository.GetByWeekDays(dayOfWeek1, dayOfWeek2).ToList();

            var expected = new List<DayOfWeek>() { dayOfWeek1, dayOfWeek2 };

            Assert.All(results, r => Assert.Contains(r.InvoiceDate.DayOfWeek, expected));
        }

    }
}