using System;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Chinook.Repository;
using Model = Chinook.Repository.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using View = Chinook.Repository.Model.View;

namespace Chinook.Repository.Integration.Tests {

    public class ReadonlyRepositorySetup {

        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] {
        new ConsoleLoggerProvider((category, level)
            => level == LogLevel.Information, true)
        });

        private readonly SqliteConnection connection;

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
            //  full repositories
            services.AddScoped<IRepository<Model.Album>, Repository<Model.Album, ChinookDbContext>>();
            services.AddScoped<IRepository<Model.Genre>, Repository<Model.Genre, ChinookDbContext>>();
            services.AddScoped<IRepository<Model.MediaType>, Repository<Model.MediaType, ChinookDbContext>>();
            services.AddScoped<IRepository<Model.Artist>, Repository<Model.Artist, ChinookDbContext>>();
            // aeon: explicitly register repositories as readonly 
            //  full repositories as readonly 
            services.AddScoped<IReadonlyRepository<Model.Album>, Repository<Model.Album, ChinookDbContext>>();
            services.AddScoped<IReadonlyRepository<Model.Genre>, Repository<Model.Genre, ChinookDbContext>>();
            services.AddScoped<IReadonlyRepository<Model.MediaType>, Repository<Model.MediaType, ChinookDbContext>>();
            services.AddScoped<IReadonlyRepository<Model.Artist>, Repository<Model.Artist, ChinookDbContext>>();

            // aeon: explicitly register ReadonlyRepository as IReadonlyRepository (eg. use this for database views)
            services.AddScoped<IReadonlyRepository<View.AlbumCountByArtists>, ReadonlyRepository<View.AlbumCountByArtists, ChinookDbContext>>();

            // Create (memory) database
            ServiceProvider = services.BuildServiceProvider();
            var dbContext = ServiceProvider.GetService<Chinook.Repository.ChinookDbContext>();
            dbContext.Database.EnsureCreated();

            // Fill (memory) database from sql script (might take a while)
            Console.WriteLine("Initializing data (this might take a while)... ");
            var dataSql = System.IO.File.ReadAllText("Chinook_DataSqlite.sql");
            dbContext.Database.ExecuteSqlCommand(dataSql);

            //create view on database
            dbContext.Database.ExecuteSqlCommand(
                @"CREATE VIEW AlbumCountByArtists 
                  AS 
                    SELECT Artist.ArtistId, Artist.Name, COUNT(AlbumId) Total FROM Artist
                    INNER JOIN Album ON Album.ArtistId = Artist.ArtistId
                    GROUP BY Artist.Name 
                    ORDER BY COUNT(AlbumId) DESC"
            );
        }

        public void Dispose() {
            connection.Close();
        }

    }
}