using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;
using Aeon.Samples.Basics.Repository.Models;
using System.Linq;
using System.ComponentModel;

namespace Aeon.Samples.Basics.Repository {

    public class Runnable {
        private readonly IRepository<Blog> _blogRepository;

        public Runnable(IRepository<Blog> blogRepository) {
            _blogRepository = blogRepository;
        }

        private async void GetAllBlogs() {
            // get all Blogs
            Console.WriteLine("All Blogs");
            var allBlogs = await _blogRepository.AllAsync();
            foreach (var blog in allBlogs) {
                Console.WriteLine(blog.Url);
            }
        }

        private async void GetAllBlogsIncludingPosts() {
            // get all Blogs, including posts
            Console.WriteLine("All Blogs including Posts");
            var blogIncludePosts = new RepositoryInclude<Blog>();
            blogIncludePosts.Include(b => b.Posts);
            var allBlogsWithPosts = await _blogRepository.AllAsync(blogIncludePosts);

            foreach (var blog in allBlogsWithPosts) {
                Console.WriteLine(blog.Url);
                foreach (var post in blog.Posts) {
                    Console.WriteLine($"\t{post.Title}");
                }
            }
        }

        private async void GetSingleBlog() {
            // get single Blog
            Console.WriteLine("Single Blog");
            var singleBlog = await _blogRepository.GetAsync(2);
            Console.WriteLine(singleBlog.Url);
        }

        private async void GetSingleBlogIncludingPosts() {
            // get single Blog, including Posts 
            Console.WriteLine("Single Blog including Posts");
            // includes
            var blogIncludePosts = new RepositoryInclude<Blog>();
            blogIncludePosts.Include(b => b.Posts);

            var singleBlogWithPosts = await _blogRepository.GetAsync(blogIncludePosts, 3);
            Console.WriteLine(singleBlogWithPosts.Url);
            foreach (var post in singleBlogWithPosts.Posts) {
                Console.WriteLine($"\t{post.Title}");
            }
        }
        private async void GetAllBlogsPaged() {
            // get blogs, paged (5 items per page)
            Console.WriteLine("Blogs; paged");
            // starting page
            var currentPage = 1;
            // get first page of entities
            var blogsPaged = await _blogRepository.GetWithFilterAsync(filter: null, sorts: null, (currentPage, 5));
            // print out result
            Console.WriteLine($"Total items: {blogsPaged.Total}");
            while (blogsPaged.Data.Count() > 0) {
                Console.WriteLine($"Page: {currentPage}");
                foreach (var blog in blogsPaged.Data) {
                    Console.WriteLine($"\t{blog.BlogId}: {blog.Url}");
                }
                // get next page of entities
                blogsPaged = await _blogRepository.GetWithFilterAsync(filter: null, sorts: null, (++currentPage, 5));
            }
        }
        private async void GetAllBlogsPagedSortedByUrl() {
            // get blogs, paged (5 items per page), sorted by url
            Console.WriteLine("Blogs; paged & sorted");
            // starting page
            var currentPage = 1;
            // sort specification
            var blogsUrlAscSort = new RepositorySort<Blog>((ListSortDirection.Ascending, f => f.Url));
            // get first page of entities
            var blogsPaged = await _blogRepository.GetWithFilterAsync(filter: null, sorts: blogsUrlAscSort, (currentPage, 5));
            // print out result
            Console.WriteLine($"Total items: {blogsPaged.Total}");
            while (blogsPaged.Data.Count() > 0) {
                Console.WriteLine($"Page: {currentPage}");
                foreach (var blog in blogsPaged.Data) {
                    Console.WriteLine($"\t{blog.BlogId}: {blog.Url}");
                }
                // get next page of entities
                blogsPaged = await _blogRepository.GetWithFilterAsync(filter: null, sorts: null, (++currentPage, 5));
            }
        }

        public void Run() {
            GetAllBlogs();
            GetAllBlogsIncludingPosts();
            GetSingleBlog();
            GetSingleBlogIncludingPosts();
            GetAllBlogsPaged();
            GetAllBlogsPagedSortedByUrl();
        }
    }

    class Program {
        /// <summary>
        /// See samples/01.Basics/01.Setup 
        /// </summary>
        static void Main() {
            var services = new ServiceCollection().AddSingleton<Runnable>();

            services.AddDbContext<BloggingContext>(options => options.UseSqlite("Filename=./blog.db"));
             
            services.AddScoped<IRepository<Blog>, Repository<Blog, BloggingContext>>();
            services.AddScoped<IRepository<Post>, Repository<Post, BloggingContext>>();

            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<BloggingContext>();
            dbContext.Database.EnsureCreated();

            serviceProvider.GetService<Runnable>().Run();
        }
    }

}
