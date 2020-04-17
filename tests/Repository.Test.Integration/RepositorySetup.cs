using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chinook.Repository.Integration.Tests {

    public class RepositorySetup {

        public static ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => {
            builder.AddFilter((category, level) => level == LogLevel.Warning)
                   .AddConsole();
        });

        private readonly SqliteConnection connection;

        public ServiceProvider ServiceProvider { get; private set; }

        public RepositorySetup() {

            var services = new ServiceCollection();


            connection = new SqliteConnection("DataSource=:memory:");
            //connection = new SqliteConnection($"Filename=./{nameof(RepositorySetup)}.sqlite");

            connection.Open();

            services.AddDbContext<Chinook.Repository.ChinookDbContext>(opt => opt.UseSqlite(connection)
                                                                             .EnableSensitiveDataLogging()
                                                                             .UseLoggerFactory(MyLoggerFactory));

            // aeon: UoW; let's do something extra in this unit of work, log changes to console (on commit)
            services.AddScoped<Chinook.Repository.Infrastructure.IChinookDbUnitOfWork, ChinookDbUnitOfWorkWithConsoleLogging>();

            // aeon: Register repositories
            services.AddScoped<IRepository<Model.Album>, Repository<Model.Album, ChinookDbContext>>();
            services.AddScoped<IRepository<Model.Genre>, Repository<Model.Genre, ChinookDbContext>>();
            services.AddScoped<IRepository<Model.MediaType>, Repository<Model.MediaType, ChinookDbContext>>();
            services.AddScoped<IRepository<Model.Track>, Repository<Model.Track, ChinookDbContext>>();
            services.AddScoped<IRepository<Model.Artist>, Repository<Model.Artist, ChinookDbContext>>();

            // Create (memory) database
            ServiceProvider = services.BuildServiceProvider();
            var dbContext = ServiceProvider.GetService<Chinook.Repository.ChinookDbContext>();
            dbContext.Database.EnsureCreated();

            //Add some predefined records  
            dbContext.Artist.Add(new Model.Artist() { Name = "AC/DC" });
            dbContext.MediaType.Add(new Model.MediaType() { Name = "MP3" });
            dbContext.SaveChanges();
        }

        public void Dispose() {
            connection.Close();
        }

    }
}