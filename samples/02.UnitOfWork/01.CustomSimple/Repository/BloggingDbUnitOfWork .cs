using Aeon.Core.Repository.Infrastructure;
using System;

namespace Aeon.Samples.UnitOfWork.CustomSimple.Repository {
    public interface IBloggingDbUnitOfWork : IUnitOfWork {
    }
    public class BloggingDbUnitOfWork : IBloggingDbUnitOfWork {
        private readonly Models.BloggingContext _context;

        public BloggingDbUnitOfWork(Models.BloggingContext context) {
            Console.WriteLine($"{nameof(BloggingDbUnitOfWork)} c'tor ");
            _context = context;
        }

        public void Commit() {
            Console.WriteLine($"{nameof(BloggingDbUnitOfWork)}.Commit");
            _context.SaveChanges();
        }
    }
}
