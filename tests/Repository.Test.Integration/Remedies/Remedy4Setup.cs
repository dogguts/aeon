using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chinook.Repository.Integration.Tests.Remedies {

    public class Remedy4Setup {

        public static ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => {
            builder.AddFilter((category, level) => level >= LogLevel.Warning)
                   .AddConsole();
        });
        private readonly SqliteConnection connection;

        public ServiceProvider ServiceProvider { get; private set; }

        // public static MemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());

        public Remedy4Setup() {

            var services = new ServiceCollection();

            services.AddEntityFrameworkSqlite();

            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();




            services.AddDbContext<Chinook.Repository.ChinookDbContext>(opt => opt.UseSqlite(connection)
                                                                             // .UseMemoryCache(MemoryCache)
                                                                             .EnableSensitiveDataLogging()
                                                                             .UseLoggerFactory(MyLoggerFactory));

            services.AddScoped<IRepository<Model.MediaType>, Repository<Model.MediaType, ChinookDbContext>>();

            ServiceProvider = services.BuildServiceProvider();
            var dbContext = ServiceProvider.GetService<Chinook.Repository.ChinookDbContext>();
            dbContext.Database.EnsureCreated();

            //Add some predefined records  
            dbContext.MediaType.AddRange(new Model.MediaType() { Name = "MP3" },
                                         new Model.MediaType() { Name = "OGG" },
                                         new Model.MediaType() { Name = "FLAC" },
                                         new Model.MediaType() { Name = "OPUS" },
                                         new Model.MediaType() { Name = "VOC" });

            dbContext.SaveChanges();
        }

        public void Dispose() {
            connection.Close();
        }

    }
}