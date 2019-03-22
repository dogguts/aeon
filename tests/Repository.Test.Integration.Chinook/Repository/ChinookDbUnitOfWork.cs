
using System;

namespace Chinook.Repository {
    public class ChinookDbUnitOfWork : Infrastructure.IChinookDbUnitOfWork {
        private ChinookDbContext _context;

        public ChinookDbUnitOfWork(ChinookDbContext context) {
            _context = context;
        }

        public void Commit() {
            System.Diagnostics.Debug.WriteLine("Commit :: ChinookDbContext=" + ((object)_context).GetHashCode());
            _context.SaveChanges();
        }
    }
}