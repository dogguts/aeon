using System;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodoApp.Model;


namespace TodoApp.Repository {
    public partial class TodoAppDbContext : DbContext {
        public TodoAppDbContext() {
        }

        public TodoAppDbContext(DbContextOptions<TodoAppDbContext> options)
            : base(options) {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<NoteItem> NoteItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {

                optionsBuilder.UseSqlite("Filename=TodoAppDb.sqlite");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            /* Model */
            var colorConverter = new ValueConverter<Color, string>(
                v => $"{v.R:X2}{v.G:X2}{v.B:X2}",
                v => Color.FromArgb(Convert.ToInt32(v, 16)));

            modelBuilder.Entity<Category>()
                        .Property(p => p.Color)
                        .HasConversion(colorConverter);

            modelBuilder.Entity<Note>(entity => { });

            modelBuilder.Entity<NoteItem>()
                .HasOne(i => i.Note)
                .WithMany(i => i.NoteItems)
                .OnDelete(DeleteBehavior.Cascade);

            /* Data seed */
            modelBuilder.Entity<Category>().HasData(new Category() { CategoryId = 1, Title = "Tasks", FaIcon = "fas fa-tasks", Color = Color.FromArgb(0xFF, 0xD7, 0xAE, 0xFB) });
            modelBuilder.Entity<Category>().HasData(new Category() { CategoryId = 2, Title = "Shopping list", FaIcon = "fas fa-shopping-cart", Color = Color.FromArgb(0xFF, 0xFF, 0xF4, 0x75) });
            modelBuilder.Entity<Category>().HasData(new Category() { CategoryId = 3, Title = "Supplies", FaIcon = "fas fa-parachute-box", Color = Color.FromArgb(0xFF, 0xCC, 0xFF, 0x90) });
            modelBuilder.Entity<Category>().HasData(new Category() { CategoryId = 4, Title = "Movies", FaIcon = "fas fa-film", Color = Color.FromArgb(0xFF, 0xA7, 0xFF, 0xEB) });

            modelBuilder.Entity<Note>().HasData(new Note() { NoteId = 1, Title = "Shopping List", CategoryId = 2 });
            modelBuilder.Entity<NoteItem>().HasData(new NoteItem() { NoteItemId = 1, NoteId = 1, Title = "Bread", Completed = false });
            modelBuilder.Entity<NoteItem>().HasData(new NoteItem() { NoteItemId = 2, NoteId = 1, Title = "Lotus Biscoff Spread", Completed = false });
            modelBuilder.Entity<NoteItem>().HasData(new NoteItem() { NoteItemId = 3, NoteId = 1, Title = "Suger Cubes", Completed = false });
            modelBuilder.Entity<NoteItem>().HasData(new NoteItem() { NoteItemId = 4, NoteId = 1, Title = "Water", Completed = false });

            modelBuilder.Entity<Note>().HasData(new Note() { NoteId = 2, Title = "Movies To See", CategoryId = 4 });
            modelBuilder.Entity<NoteItem>().HasData(new NoteItem() { NoteItemId = 5, NoteId = 2, Title = "Ad Astra", Completed = false });
            modelBuilder.Entity<NoteItem>().HasData(new NoteItem() { NoteItemId = 6, NoteId = 2, Title = "It 2", Completed = false });

            modelBuilder.Entity<Note>().HasData(new Note() { NoteId = 3, Title = "Miscellaneous", CategoryId = 1 });
            modelBuilder.Entity<NoteItem>().HasData(new NoteItem() { NoteItemId = 7, NoteId = 3, Title = "Build time machine", Completed = false });
            modelBuilder.Entity<NoteItem>().HasData(new NoteItem() { NoteItemId = 8, NoteId = 3, Title = "Grow epic beard", Completed = false });
            modelBuilder.Entity<NoteItem>().HasData(new NoteItem() { NoteItemId = 9, NoteId = 3, Title = "Plant lettuce", Completed = false });
            modelBuilder.Entity<NoteItem>().HasData(new NoteItem() { NoteItemId = 10, NoteId = 3, Title = "World domination", Completed = false });


        }



    }
}
