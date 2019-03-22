using System;
using Chinook.Repository.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


/*
~/Dropbox/Projects/Pantheon/Chinook/src/Repository$ dotnet ef dbcontext scaffold "Filename=/home/dogguts/Dropbox/Projects/Pantheon/Chinook/src/Chinook_Sqlite_AutoIncrementPKs.sqlite" Microsoft.EntityFrameworkCore.Sqlite -s ../test/ --force -o Model -d

NOTE: https://github.com/aspnet/EntityFrameworkCore/issues/11961 (ValueGeneratedOnNever -> ValueGeneratedOnAdd)
*/

namespace Chinook.Repository {
    public partial class ChinookDbContext : DbContext {
        public ChinookDbContext() {

        }

        public ChinookDbContext(DbContextOptions<ChinookDbContext> options)
            : base(options) {
        }

        public virtual DbSet<Album> Album { get; set; }
        public virtual DbSet<Artist> Artist { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<Genre> Genre { get; set; }
        public virtual DbSet<Invoice> Invoice { get; set; }
        public virtual DbSet<InvoiceLine> InvoiceLine { get; set; }
        public virtual DbSet<MediaType> MediaType { get; set; }
        public virtual DbSet<Playlist> Playlist { get; set; }
        public virtual DbSet<PlaylistTrack> PlaylistTrack { get; set; }
        public virtual DbSet<Track> Track { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                optionsBuilder.UseSqlite("Filename=chinook.sqlite");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Album>(entity => {
                entity.HasIndex(e => e.AlbumId)
                    .HasName("IPK_Album")
                    .IsUnique();

                entity.HasIndex(e => e.ArtistId)
                    .HasName("IFK_AlbumArtistId");

                entity.Property(e => e.AlbumId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Artist)
                    .WithMany(p => p.Album)
                    .HasForeignKey(d => d.ArtistId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Artist>(entity => {
                entity.HasIndex(e => e.ArtistId)
                    .HasName("IPK_Artist")
                    .IsUnique();

                entity.Property(e => e.ArtistId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Customer>(entity => {
                entity.HasIndex(e => e.CustomerId)
                    .HasName("IPK_Customer")
                    .IsUnique();

                entity.HasIndex(e => e.SupportRepId)
                    .HasName("IFK_CustomerSupportRepId");

                entity.Property(e => e.CustomerId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Employee>(entity => {
                entity.HasIndex(e => e.EmployeeId)
                    .HasName("IPK_Employee")
                    .IsUnique();

                entity.HasIndex(e => e.ReportsTo)
                    .HasName("IFK_EmployeeReportsTo");

                entity.Property(e => e.EmployeeId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Genre>(entity => {
                entity.HasIndex(e => e.GenreId)
                    .HasName("IPK_Genre")
                    .IsUnique();

                entity.Property(e => e.GenreId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Invoice>(entity => {
                entity.HasIndex(e => e.CustomerId)
                    .HasName("IFK_InvoiceCustomerId");

                entity.HasIndex(e => e.InvoiceId)
                    .HasName("IPK_Invoice")
                    .IsUnique();

                entity.Property(e => e.InvoiceId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Invoice)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<InvoiceLine>(entity => {
                entity.HasIndex(e => e.InvoiceId)
                    .HasName("IFK_InvoiceLineInvoiceId");

                entity.HasIndex(e => e.InvoiceLineId)
                    .HasName("IPK_InvoiceLine")
                    .IsUnique();

                entity.HasIndex(e => e.TrackId)
                    .HasName("IFK_InvoiceLineTrackId");

                entity.Property(e => e.InvoiceLineId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.Invoice)
                    .WithMany(p => p.InvoiceLine)
                    .HasForeignKey(d => d.InvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Track)
                    .WithMany(p => p.InvoiceLine)
                    .HasForeignKey(d => d.TrackId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<MediaType>(entity => {
                entity.HasIndex(e => e.MediaTypeId)
                    .HasName("IPK_MediaType")
                    .IsUnique();

                entity.Property(e => e.MediaTypeId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Playlist>(entity => {
                entity.HasIndex(e => e.PlaylistId)
                    .HasName("IPK_Playlist")
                    .IsUnique();

                entity.Property(e => e.PlaylistId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<PlaylistTrack>(entity => {
                entity.HasKey(e => new { e.PlaylistId, e.TrackId });

                entity.HasIndex(e => e.TrackId)
                    .HasName("IFK_PlaylistTrackTrackId");

                entity.HasIndex(e => new { e.PlaylistId, e.TrackId })
                    .HasName("IPK_PlaylistTrack")
                    .IsUnique();

                entity.HasOne(d => d.Playlist)
                    .WithMany(p => p.PlaylistTrack)
                    .HasForeignKey(d => d.PlaylistId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Track)
                    .WithMany(p => p.PlaylistTrack)
                    .HasForeignKey(d => d.TrackId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Track>(entity => {
                entity.HasIndex(e => e.AlbumId)
                    .HasName("IFK_TrackAlbumId");

                entity.HasIndex(e => e.GenreId)
                    .HasName("IFK_TrackGenreId");

                entity.HasIndex(e => e.MediaTypeId)
                    .HasName("IFK_TrackMediaTypeId");

                entity.HasIndex(e => e.TrackId)
                    .HasName("IPK_Track")
                    .IsUnique();

                entity.Property(e => e.TrackId).ValueGeneratedOnAdd();

                entity.HasOne(d => d.MediaType)
                    .WithMany(p => p.Track)
                    .HasForeignKey(d => d.MediaTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
