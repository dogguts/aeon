using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Aeon.Samples.Basics.Repository.Models {
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
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 4, Url = "https://ornithology.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 5, Url = "https://blog.lauraerickson.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 6, Url = "http://birdwatchinghq.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 7, Url = "http://www.birdchick.com/blog" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 8, Url = "https://birdingthroughglass.blogspot.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 9, Url = "https://animalperspectives.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 10, Url = "https://www.ornithologistsblog.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 11, Url = "https://www.birdoculars.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 12, Url = "https://birdfreak.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 13, Url = "http://www.birdcouple.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 14, Url = "http://seagullsteve.blogspot.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 15, Url = "https://dantallmansbirdblog.blogspot.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 16, Url = "http://www.10000birds.com/category/birding" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 17, Url = "https://wildbirdsunlimited.typepad.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 18, Url = "https://theaccidentalbirder.com/" });
            modelBuilder.Entity<Blog>().HasData(new Blog { BlogId = 19, Url = "https://otwtb.birdwatchersdigest.com/" });

            // "Post" seed
            modelBuilder.Entity<Post>().HasData(new Post {
                PostId = 1,
                BlogId = 3,
                Title = "How to Learn Bird Songs and Calls",
                Content = "When a bird sings, it's telling you what it is and where it is.\r\nLearn bird calls and open a new window on your birding.",
            });
            modelBuilder.Entity<Post>().HasData(new Post {
                PostId = 2,
                BlogId = 3,
                Title = "The 4 Keys to Bird Identification",
                Content = "To identify an unfamiliar bird, focus first on four keys to identification.",
            });
            modelBuilder.Entity<Post>().HasData(new Post {
                PostId = 3,
                BlogId = 3,
                Title = "How To Choose The Right Kind Of Bird Feeder",
                Content = "The ideal bird feeder is sturdy enough to withstand winter weather and squirrels, tight enough to keep seeds dry, easy to assemble and, most important of all, easy to keep clean.",
            });
            modelBuilder.Entity<Post>().HasData(new Post {
                PostId = 4,
                BlogId = 1,
                Title = "Identification Summary Of Vaux's and Chimney Swifts",
                Content = "Back in 2010, David Sibley illustrated a very useful chart highlighting the structural differences in the shape of the wings between Vaux's and Chimney Swift.",
            });

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

