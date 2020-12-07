using Aeon.Core.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Chinook.Repository.Integration.Tests {
    public class ServiceCollectionExtensionsSetup {

        public static ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => {
            builder.AddFilter((category, level) => level >= LogLevel.Warning)
                   .AddConsole();
        });

        private readonly SqliteConnection connection;

        public ServiceProvider ServiceProvider { get; private set; }
        //exposed for unit tests only, you should never do this!
        public ServiceCollection Services { get; private set; }

        public ServiceCollectionExtensionsSetup() {
            Services = new ServiceCollection();

            connection = new SqliteConnection("DataSource=:memory:");

            connection.Open();

            Services.AddDbContext<Chinook.Repository.ChinookDbContext>(opt => opt.UseSqlite(connection)
                                                                             .EnableSensitiveDataLogging()
                                                                             .UseLoggerFactory(MyLoggerFactory));

            //// aeon: UoW; let's do something extra in this unit of work, log changes to console (on commit)
            //services.AddScoped<Chinook.Repository.Infrastructure.IChinookDbUnitOfWork, ChinookDbUnitOfWorkWithConsoleLogging>();

            // aeon: Register repositories
            //services.AddScoped<IRepository<Model.Album>, Repository<Model.Album, ChinookDbContext>>();
            //services.AddScoped<IRepository<Model.Genre>, Repository<Model.Genre, ChinookDbContext>>();
            //services.AddScoped<IRepository<Model.MediaType>, Repository<Model.MediaType, ChinookDbContext>>();
            //services.AddScoped<IRepository<Model.Track>, Repository<Model.Track, ChinookDbContext>>();
            //services.AddScoped<IRepository<Model.Artist>, Repository<Model.Artist, ChinookDbContext>>();
            Services.AddRepositories<ChinookDbContext>();

            ServiceProvider = Services.BuildServiceProvider();
        }

        public void Dispose() {
            connection.Close();
        }

    }
}


