using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Aeon.Samples.BasicSetup {

    public class Runnable {
        private readonly IRepository<Blog> _blogRepository;
        public Runnable(IRepository<Blog> blogRepository) {
            _blogRepository = blogRepository;
        }
        public void Run() {
            // do something with our setup;  get a single Blog with keyvalue=={1}
            var blogWithPk1 = _blogRepository.Get(1);
            Console.WriteLine(blogWithPk1.Url);
        }
    }

    class Program {

        static void Main() {
            var services = new ServiceCollection().AddSingleton<Runnable>();

            // Register/setups the context for dependency injection
            services.AddDbContext<BloggingContext>(options => options.UseSqlite("Filename=./blog.db"));

            // aeon: Register repositories
            // aeon: Register a Blog-Repository using the Blog Model and BloggingContext DbContext
            services.AddScoped<IRepository<Blog>, Repository<Blog, BloggingContext>>();
            // aeon: Register a Post-Repository using the Post Model and BloggingContext DbContext
            services.AddScoped<IRepository<Post>, Repository<Post, BloggingContext>>();

            var serviceProvider = services.BuildServiceProvider();

            // Create/seed database
            var dbContext = serviceProvider.GetService<BloggingContext>();
            dbContext.Database.EnsureCreated();

            // Run Runnable
            serviceProvider.GetService<Runnable>().Run();
        }


    }
}
