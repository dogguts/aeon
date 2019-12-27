// using System;

// namespace Chinook.Repository {
//     public class LastFmDbUnitOfWork : Infrastructure.ILastFmDbUnitOfWork {
//         private LastFmDbContext _context;

//         public LastFmDbUnitOfWork(LastFmDbContext context) {
//             _context = context;
//         }

//         public void Commit() {
//             System.Diagnostics.Debug.WriteLine("Commit :: LastFmDbContext=" + ((object)_context).GetHashCode());
//             _context.SaveChanges();
//         }
//     }
// }