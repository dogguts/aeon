using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((services) => {
        services.AddDbContext<BloggingContext>(options => options.UseSqlite($"Filename=./blog.db"));

        // aeon: Register repositories

        // aeon: Register a Blog-Repository as a IReadonlyRepository using the Blog Model and BloggingContext DbContext
        services.AddScoped<IReadonlyRepository<Blog>, Repository<Blog, BloggingContext>>();

        // aeon: Register a Post-ReadonlyRepository as a IReadonlyRepository using the Post Model and BloggingContext DbContext
        services.AddScoped<IReadonlyRepository<Post>, ReadonlyRepository<Post, BloggingContext>>();

        // aeon: Register a PostCountByBlogs-ReadonlyRepository as a IReadonlyRepository using the PostCountByBlogs Model and BloggingContext DbContext
        // aeon: PostCountByBlogs refers to a View in the database
        services.AddScoped<IReadonlyRepository<PostCountByBlogs>, ReadonlyRepository<PostCountByBlogs, BloggingContext>>();

        services.AddHostedService<ConsoleApplication>();
    })
    .ConfigureLogging(logging => logging.AddFilter(level => level > LogLevel.Information))
    .RunConsoleAsync(new CancellationToken());



