using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Chinook.Repository.Infrastructure;
using Chinook.Repository.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Chinook.Repository.Integration.Tests {

    public class ExtendedRepositorySetup {

        public static ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => {
            builder.AddFilter((category, level) => level >= LogLevel.Warning)
                   .AddConsole();
        });

        private readonly SqliteConnection connection;

        public ServiceProvider ServiceProvider { get; private set; }

        public ExtendedRepositorySetup() {
            var services = new ServiceCollection();

            connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            services.AddDbContext<Chinook.Repository.ChinookDbContext>(opt => opt.UseSqlite(connection)
                                                                             .UseLoggerFactory(MyLoggerFactory));

            // aeon: UoW
            services.AddScoped<Chinook.Repository.Infrastructure.IChinookDbUnitOfWork, ChinookDbUnitOfWork>();

            // aeon: Register generic/default invoice repository (optional in this case)
            services.AddScoped<IRepository<Invoice>, Repository<Invoice, ChinookDbContext>>();
            // aeon: Register extended invoice repository
            services.AddScoped<IInvoiceRepository, InvoiceRepository<ChinookDbContext>>();

            // Create (memory) database
            ServiceProvider = services.BuildServiceProvider();
            var dbContext = ServiceProvider.GetService<Chinook.Repository.ChinookDbContext>();
            dbContext.Database.EnsureCreated();


            // Fill (memory) database from sql script (might take a while)
            Console.WriteLine("Initializing data (this might take a while)... ");
            var dataSql = System.IO.File.ReadAllText("Chinook_DataSqlite.sql");
            dbContext.Database.ExecuteSqlRaw(dataSql);
        }

        public void Dispose() {
            connection.Close();
        }

    }


}