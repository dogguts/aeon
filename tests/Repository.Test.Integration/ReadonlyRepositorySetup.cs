using System;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Chinook.Repository;
using Chinook.Repository.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Chinook.Repository.Integration.Tests {

    public class ReadonlyRepositorySetup {

        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] {
        new ConsoleLoggerProvider((category, level)
            => level == LogLevel.Information, true)
        });

        private SqliteConnection connection;

        public ServiceProvider ServiceProvider { get; private set; }

        public ReadonlyRepositorySetup() {

            var services = new ServiceCollection();


            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<Chinook.Repository.ChinookDbContext>(opt => opt.UseSqlite(connection)
                                                                             .UseLoggerFactory(MyLoggerFactory));

            // aeon: UoW
            services.AddScoped<Chinook.Repository.Infrastructure.IChinookDbUnitOfWork, ChinookDbUnitOfWork>();

            // aeon: Register repositories
            services.AddScoped<IRepository<Album>, Repository<Album, ChinookDbContext>>();
            services.AddScoped<IRepository<Genre>, Repository<Genre, ChinookDbContext>>();
            services.AddScoped<IRepository<MediaType>, Repository<MediaType, ChinookDbContext>>();
            services.AddScoped<IRepository<Artist>, Repository<Artist, ChinookDbContext>>();
            // aeon: explicitly register repository as readonly (as example)
            services.AddScoped<IReadonlyRepository<Genre>, Repository<Genre, ChinookDbContext>>();


            // Create (memory) database
            ServiceProvider = services.BuildServiceProvider();
            var dbContext = ServiceProvider.GetService<Chinook.Repository.ChinookDbContext>();
            dbContext.Database.EnsureCreated();


            // Fill (memory) database from sql script (might take a while)
            Console.WriteLine("Initializing data (this might take a while)... ");
            var dataSql = System.IO.File.ReadAllText("Chinook_DataSqlite.sql");
            dbContext.Database.ExecuteSqlCommand(dataSql);
        }

        public void Dispose() {
            connection.Close();
        }

    }
}