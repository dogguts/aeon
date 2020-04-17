using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Aeon.Samples.UnitOfWork.CustomSimple.Models;
using Aeon.Samples.UnitOfWork.CustomSimple.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aeon.Samples.UnitOfWork.CustomSimple {

    public class Runnable {
        private readonly IRepository<Blog> _blogRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IBloggingDbUnitOfWork bloggingDbUnitOfWork;
        public Runnable(IRepository<Blog> blogRepository, IRepository<Post> postRepository, IBloggingDbUnitOfWork bloggingDbUnitOfWork) {
            _blogRepository = blogRepository;
            _postRepository = postRepository;
            this.bloggingDbUnitOfWork = bloggingDbUnitOfWork;
        }

        public void RepositoryAdd() {
            // get a Blog to which we will add a Post
            var birdwatchinghqBlog = _blogRepository.Get(6);

            // create a new Post
            var newPost = new Post() {
                Blog = birdwatchinghqBlog,
                Title = "8 Ways to Get Rid of Pigeons From Your House",
                Content = "How do you get rid of pigeons? Almost everyone that has attracted this invasive species to their yard has asked themselves this question."
            };

            // add the Post to the Blog using the Post Repository
            _postRepository.Add(newPost);
        }

        public void RepositoryUpdate() {
            // get an existing Post (including its Blog) which we will modify
            var postIncludeBlog = new RepositoryInclude<Post>();
            postIncludeBlog.Include(p => p.Blog);
            var existingPostToModify = _postRepository.Get(postIncludeBlog, 2);

            // update the Post
            existingPostToModify.Title = "The 5 Keys to Bird Identification";
            existingPostToModify.Content = "To identify an unfamiliar bird, focus first on five keys to identification.";

            // register this Post as updated to the Post Repository
            _postRepository.Update(existingPostToModify);
        }

        public void RepositoryDelete() {
            // get te Post to delete
            var existingPostToDelete = _postRepository.Get(2);

            // register this Post for deletion with the Post Repository
            _postRepository.Delete(existingPostToDelete);
        }

        public void Run() {
            RepositoryAdd();
            RepositoryUpdate();
            RepositoryDelete();

            // persist above changes to database
            bloggingDbUnitOfWork.Commit();
        }
    }

    class Program {
        /// <summary>
        /// See samples/01.Basics/01.Setup 
        /// </summary>
        static void Main() {
            var services = new ServiceCollection().AddSingleton<Runnable>();

            // since this example modifies data, be sure to start with a freshly created database each run
            System.IO.File.Delete("blog.db");

            services.AddDbContext<BloggingContext>(options => options.UseSqlite("Filename=./blog.db"));

            // aeon: Register a custom (very simple) UnitOfWork with dependency injection 
            services.AddScoped<IBloggingDbUnitOfWork, BloggingDbUnitOfWork>();

            services.AddScoped<IRepository<Blog>, Repository<Blog, BloggingContext>>();
            services.AddScoped<IRepository<Post>, Repository<Post, BloggingContext>>();

            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetService<BloggingContext>();
            dbContext.Database.EnsureCreated();

            serviceProvider.GetService<Runnable>().Run();
        }
    }

}
