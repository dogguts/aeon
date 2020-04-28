using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Aeon.Samples.BasicSetup {
    public class BloggingContext : DbContext {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        public BloggingContext(DbContextOptions<BloggingContext> options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Blog>(entity => {
                entity.Property(b => b.Url).IsRequired();
            });

            modelBuilder.Entity<Post>(entity => {
                entity.HasOne(p => p.Blog)
                    .WithMany(b => b.Posts)
                    .HasForeignKey(p => p.BlogId);
            });

            // "Blog" seed
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 1, Url = "http://www.surfbirds.com/index.php" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 2, Url = "http://www.audubon.org/birding" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 3, Url = "https://www.allaboutbirds.org/" });
        }
    }

    public class Blog {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>();
    }

    public class Post {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }

    }
}



