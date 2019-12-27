﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TodoApp.Repository;

namespace Repository.Migrations
{
    [DbContext(typeof(TodoAppDbContext))]
    partial class TodoAppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099");

            modelBuilder.Entity("TodoApp.Model.Category", b =>
                {
                    b.Property<long>("CategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Color")
                        .IsRequired();

                    b.Property<bool>("Deleted");

                    b.Property<string>("FaIcon");

                    b.Property<string>("Title");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new { CategoryId = 1L, Color = "D7AEFB", Deleted = false, FaIcon = "fa-tasks", Title = "Tasks" },
                        new { CategoryId = 2L, Color = "FFF475", Deleted = false, FaIcon = "fa-shopping-cart", Title = "Shopping list" },
                        new { CategoryId = 3L, Color = "CCFF90", Deleted = false, FaIcon = "fa-parachute-box", Title = "Supplies" },
                        new { CategoryId = 4L, Color = "A7FFEB", Deleted = false, FaIcon = "fa-film", Title = "Movies" }
                    );
                });

            modelBuilder.Entity("TodoApp.Model.Note", b =>
                {
                    b.Property<long>("NoteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("NoteId");

                    b.Property<long>("CategoryId");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("NoteId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Notes");

                    b.HasData(
                        new { NoteId = 1L, CategoryId = 2L, Title = "Shopping List" },
                        new { NoteId = 2L, CategoryId = 4L, Title = "Movies To See" },
                        new { NoteId = 3L, CategoryId = 1L, Title = "Miscellaneous" }
                    );
                });

            modelBuilder.Entity("TodoApp.Model.NoteItem", b =>
                {
                    b.Property<long>("NoteItemId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Completed");

                    b.Property<long?>("NoteId")
                        .IsRequired();

                    b.Property<string>("Title")
                        .IsRequired();

                    b.HasKey("NoteItemId");

                    b.HasIndex("NoteId");

                    b.ToTable("NoteItem");

                    b.HasData(
                        new { NoteItemId = 1L, Completed = false, NoteId = 1L, Title = "Bread" },
                        new { NoteItemId = 2L, Completed = false, NoteId = 1L, Title = "Lotus Biscoff Spread" },
                        new { NoteItemId = 3L, Completed = false, NoteId = 1L, Title = "Suger Cubes" },
                        new { NoteItemId = 4L, Completed = false, NoteId = 1L, Title = "Water" },
                        new { NoteItemId = 5L, Completed = false, NoteId = 2L, Title = "Ad Astra" },
                        new { NoteItemId = 6L, Completed = false, NoteId = 2L, Title = "It 2" },
                        new { NoteItemId = 7L, Completed = false, NoteId = 3L, Title = "Build time machine" },
                        new { NoteItemId = 8L, Completed = false, NoteId = 3L, Title = "Grow epic beard" },
                        new { NoteItemId = 9L, Completed = false, NoteId = 3L, Title = "Plant lettuce" },
                        new { NoteItemId = 10L, Completed = false, NoteId = 3L, Title = "World domination" }
                    );
                });

            modelBuilder.Entity("TodoApp.Model.Note", b =>
                {
                    b.HasOne("TodoApp.Model.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("TodoApp.Model.NoteItem", b =>
                {
                    b.HasOne("TodoApp.Model.Note", "Note")
                        .WithMany("NoteItems")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
