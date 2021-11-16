using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;

internal sealed class ConsoleApplication : IHostedService {

    private readonly IReadonlyRepository<PostCountByBlogs> _postCountByBlogsRepository;

    /// <summary>c'tor</summary> 
    /// <param name="postCountByBlogsRepository"></param>
    /// <param name="serviceProvider">Only injected so we can access DbContext to create our database in SetupDatabase</param>
    public ConsoleApplication(IReadonlyRepository<PostCountByBlogs> postCountByBlogsRepository,
                              IServiceProvider serviceProvider) {
        _postCountByBlogsRepository = postCountByBlogsRepository;
        SetupDatabase(serviceProvider);
    }

    /// <summary>(re)-create the sample database</summary>
    private static void SetupDatabase(IServiceProvider serviceProvider) {
        using (var scope = serviceProvider.CreateScope()) {
            var dbContext = scope.ServiceProvider.GetService<BloggingContext>();
            // (re-)create the database
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            // create the database view "PostCountByBlogs"              
            dbContext.Database.ExecuteSqlRaw(
                @"CREATE VIEW PostCountByBlogs 
                              AS 
                                SELECT Blog.BlogId, Blog.Name, COUNT(PostId) Total FROM Blog
                                INNER JOIN Post ON Post.BlogId = Blog.BlogId
                                GROUP BY Blog.BlogId 
                                ORDER BY COUNT(PostId) DESC"
            );
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken) {
        // construct an include directive for the "PostCountByBlogs" database view, including the Blog and the Blog's Posts
        var postCountByBlogsIncludeBlog = new RepositoryInclude<PostCountByBlogs>();
        postCountByBlogsIncludeBlog.Include(v => v.Blog).ThenInclude(b => b.Posts);

        // get all PostCountByBlogs with the constructed include
        var viewData = await _postCountByBlogsRepository.AllAsync(postCountByBlogsIncludeBlog);

        // print all 
        foreach (var blogPostsCount in viewData) {
            Console.WriteLine($"-= {blogPostsCount.BlogName} =- (#{ blogPostsCount.Total})");
            foreach (var post in blogPostsCount.Blog.Posts) {
                Console.WriteLine($" ** {post.PostId}: {post.Title}");
            }
        }

        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

}
