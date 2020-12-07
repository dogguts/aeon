using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Aeon.Samples.Extend.SoftDelete.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Aeon.Samples.Extend.SoftDelete {
    public class Runnable {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IUnitOfWork<BloggingContext> _bloggingDbUnitOfWork;

        private readonly DbContext _dbContext;

        public Runnable(IRepository<Blog> blogRepository, IRepository<Post> postRepository, IUnitOfWork<BloggingContext> bloggingDbUnitOfWork, BloggingContext dbContext) {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
            _bloggingDbUnitOfWork = bloggingDbUnitOfWork;
            _dbContext = dbContext; // DbContext is injected kept here only for illustration purposed (to show the EntityState later)
        }



        public void Run() {
            // Delete an existing Post

            // get an existing Post to delete
            var existingPostToDelete = _postRepository.Get(2);
            // current state = Post:2, EntityState:Unmodified, Deleted:false
            Console.WriteLine($"Deleting post:{existingPostToDelete.PostId}, EntityState:{_dbContext.Entry(existingPostToDelete).State}, Deleted:{existingPostToDelete.Deleted}");
            // (soft-)Delete the post
            _postRepository.Delete(existingPostToDelete);
            // current state = Post:2, EntityState:Modified, Deleted:true
            Console.WriteLine($"Deleted post:{existingPostToDelete.PostId}, EntityState:{_dbContext.Entry(existingPostToDelete).State}, Deleted:{existingPostToDelete.Deleted}");

            // persist above changes to database
            _bloggingDbUnitOfWork.Commit();
        }
    }

    class Program {
        /// <summary>
        /// Replace the default Repositoty-Delete operation with a soft Delete-property
        /// </summary>
        static void Main() {
            var services = new ServiceCollection().AddSingleton<Runnable>();

            // since this example modifies data, be sure to start with a freshly created database each run
            System.IO.File.Delete("blog.db");

            services.AddDbContext<BloggingContext>(options => options.UseSqlite("Filename=./blog.db"));

            // aeon: Register default UnitOfWork with dependency injection 
            services.AddScoped<IUnitOfWork<BloggingContext>, DefaultDbUnitOfWork<BloggingContext>>();

            services.AddScoped<IRepository<Blog>, Repository<Blog, BloggingContext>>();
            // Register a custom implementation for IRepository<Post>:  SoftDeleteRepository<,>
            services.AddScoped<IRepository<Post>, Infrastructure.SoftDeleteRepository<Post, BloggingContext>>(svc => {
                var dbContext = svc.GetRequiredService<BloggingContext>();
                return new Infrastructure.SoftDeleteRepository<Post, BloggingContext>(dbContext, nameof(Post.Deleted));
            });

            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<BloggingContext>();
            dbContext.Database.EnsureCreated();

            serviceProvider.GetService<Runnable>().Run();
        }
    }
}
