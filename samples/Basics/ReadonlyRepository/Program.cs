using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;
using Aeon.Samples.Basics.ReadonlyRepository.Models;
using System.Linq;

namespace Aeon.Samples.Basics.ReadonlyRepository {

    public class Runnable {

        private readonly IReadonlyRepository<PostCountByBlogs> _postCountByBlogsRepository;

        public Runnable(IReadonlyRepository<PostCountByBlogs> postCountByBlogsRepository) {
            _postCountByBlogsRepository = postCountByBlogsRepository;
        }

        public void Run() {
            // retrieve data from the "PostCountByBlogs" database View, including the Blog and from Blog its Posts
            var postCountByBlogsIncludeBlog = new RepositoryInclude<PostCountByBlogs>();
            postCountByBlogsIncludeBlog.Include(v => v.Blog).ThenInclude(b => b.Posts);

            var viewData = _postCountByBlogsRepository.All(postCountByBlogsIncludeBlog);

            foreach (var blogPostsCount in viewData) {
                Console.WriteLine($"{blogPostsCount.BlogUrl} (#Posts:{blogPostsCount.Total})");
                foreach (var post in blogPostsCount.Blog.Posts) {
                    Console.WriteLine($"\t[{post.PostId}]: {post.Title}");
                }
            }
        }
    }

    class Program {

        static void Main() {
            var services = new ServiceCollection().AddSingleton<Runnable>();
            services.AddDbContext<BloggingContext>(options => options.UseSqlite("Filename=./blog.db"));

            // aeon: Register repositories

            // aeon: Register a Blog-Repository as a IReadonlyRepository using the Blog Model and BloggingContext DbContext
            services.AddScoped<IReadonlyRepository<Blog>, Repository<Blog, BloggingContext>>();

            // aeon: Register a Post-ReadonlyRepository as a IReadonlyRepository using the Post Model and BloggingContext DbContext
            services.AddScoped<IReadonlyRepository<Post>, ReadonlyRepository<Post, BloggingContext>>();

            // aeon: Register a PostCountByBlogs-ReadonlyRepository as a IReadonlyRepository using the PostCountByBlogs Model and BloggingContext DbContext
            // aeon: PostCountByBlogs refers to a View in the database
            services.AddScoped<IReadonlyRepository<PostCountByBlogs>, ReadonlyRepository<PostCountByBlogs, BloggingContext>>();

            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<BloggingContext>();
            dbContext.Database.EnsureCreated();

            // re-create the database view "PostCountByBlogs"  
            dbContext.Database.ExecuteSqlRaw("DROP VIEW IF EXISTS PostCountByBlogs;");

            dbContext.Database.ExecuteSqlRaw(
            @"CREATE VIEW PostCountByBlogs 
                  AS 
                    SELECT Blog.BlogId, Blog.Url, COUNT(PostId) Total FROM Blog
                    INNER JOIN Post ON Post.BlogId = Blog.BlogId
                    GROUP BY Blog.Url 
                    ORDER BY COUNT(PostId) DESC"
            );


            serviceProvider.GetService<Runnable>().Run();
        }
    }

}
