// using System;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata;

// namespace Chinook.Repository {
//     public partial class LastFmDbContext : DbContext {
//         public LastFmDbContext() {
//         }

//         public LastFmDbContext(DbContextOptions<LastFmDbContext> options)
//             : base(options) {
//         }

//         public virtual DbSet<Artist> Artist { get; set; }

//         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
//             if (!optionsBuilder.IsConfigured) {
//                 optionsBuilder.UseSqlite("Filename=lastfm.sqlite");
//             }
//         }

//         protected override void OnModelCreating(ModelBuilder modelBuilder) {
//             modelBuilder.Entity<Artist>(entity => {
//                 entity.HasIndex(e => e.Id)
//                     .IsUnique();

//                 entity.Property(e => e.Id).ValueGeneratedNever();
//             });
//         }
//     }
// }
