using System;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Chinook.Model;
using Chinook.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Linq;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Internal;
using Chinook.Service;
using System.Collections;

using System.Reflection;
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel;

namespace console1 {


    public class TextFilter<T> {
        public TextFilter() { }
    }

    static class TextFilterSpecification {
        public static (IRepositoryFilter<Artist> Filter, RepositorySort<Artist> Sort) SearchFor(this TextFilter<Artist> cls, string searchString) {
            var filter = new RepositoryFilter<Artist>(p => p.Name.ToLower().Contains(searchString.ToLower()));
            var sort = new RepositorySort<Artist>((ListSortDirection.Ascending, a => a.Name));
            return (filter, sort);
        }
    }


    class Program {
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] {
        new ConsoleLoggerProvider((category, level)
            => level == LogLevel.Information, true)
        });

        public static ServiceProvider ServiceProvider { get; private set; }

        static void Main(string[] args) {
            Console.WriteLine("Hello World!");

            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables();
            builder.AddUserSecrets<Program>();
            var Configuration = builder.Build();





            var services = new ServiceCollection();

            services.AddEntityFrameworkSqlite();

            services.AddDbContext<Chinook.Repository.ChinookDbContext>(opt => opt.UseSqlite("Data Source=chinook.sqlite")
                                                                                 .UseLoggerFactory(MyLoggerFactory));

            // services.AddDbContext<Chinook.Repository.LastFmDbContext>(opt => opt.UseSqlite("Data Source=lastfm.sqlite")
            //                                                                      .UseLoggerFactory(MyLoggerFactory));

            // IHttpClientFactory
            services.AddHttpClient();

            // aeon: UoW
            services.AddScoped<Chinook.Repository.Infrastructure.IChinookDbUnitOfWork, ChinookDbUnitOfWork>();
            //services.AddScoped<Chinook.Repository.Infrastructure.ILastFmDbUnitOfWork, LastFmDbUnitOfWork>();

            // aeon: Register repositories
            services.AddScoped<IRepository<Album>, Repository<Album, ChinookDbContext>>();
            services.AddScoped<IRepository<Genre>, Repository<Genre, ChinookDbContext>>();
            services.AddScoped<IRepository<MediaType>, Repository<MediaType, ChinookDbContext>>();
            services.AddScoped<IRepository<Artist>, Repository<Artist, ChinookDbContext>>();

            services.AddScoped<IRepository<Invoice>, Repository<Invoice, ChinookDbContext>>();
            services.AddScoped<IRepository<InvoiceLine>, Repository<InvoiceLine, ChinookDbContext>>();
            services.AddScoped<IRepository<PlaylistTrack>, Repository<PlaylistTrack, ChinookDbContext>>();
            //services.AddScoped<IRepository<Chinook.Model.LastFm.Artist>, Repository<Chinook.Model.LastFm.Artist, LastFmDbContext>>();

            // services 
            services.AddScoped<Chinook.Service.IDateTimeService, Chinook.Service.DateTimeService>();
            services.AddScoped<Chinook.Service.IOverviewService, Chinook.Service.OverviewService>();

            // services: lastfm
            var lastfmSettings = Configuration.GetSection("lastfm").Get<LastFmService.LastFmSettings>();

            //services.AddTransient<LastfmClient>(x => new LastfmClient(lastfmSettings.Key, lastfmSettings.Secret));

            services.AddScoped<Chinook.Service.ILastFmService, Chinook.Service.LastFmService>();



            // automapper
            services.AddAutoMapper(cfg => { }, typeof(Chinook.Mapping.AssemblyHook).Assembly);

            services.AddScoped<Program>();

            ServiceProvider = services.BuildServiceProvider();

            var mapper = ServiceProvider.GetRequiredService<IMapper>();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            var prg = ServiceProvider.GetRequiredService<Program>();
            prg.Test();
            Console.ReadLine();
        }

        //public Program() { }

        private IRepository<Album> _albumRepo;
        public Program(IRepository<Album> albumRepo, IRepository<PlaylistTrack> ptRepo, IRepository<Artist> _artistRepo) {
            _albumRepo = albumRepo;

            // analysis: exhuberant caching of compiled queries
            //  https://github.com/aspnet/EntityFrameworkCore/issues/10535#issuecomment-375116203
	    //  https://github.com/aspnetboilerplate/aspnetboilerplate/blob/master/src/Abp/Domain/Repositories/AbpRepositoryBase.cs

            var a1 = _albumRepo.Get(1);
            var a2 = _albumRepo.Get(2);
            var a3 = _albumRepo.Get(3);

            var artistFilter = new TextFilter<Artist>().SearchFor("Iron");

            var artist1 = _artistRepo.GetWithFilter(new TextFilter<Artist>().SearchFor("Iron"));
            var artist2 = _artistRepo.GetWithFilter(new TextFilter<Artist>().SearchFor("Black"));
            var pt1 = ptRepo.Get(1, 3390);


            var compiledQueryCache = (Microsoft.EntityFrameworkCore.Query.Internal.CompiledQueryCache)ServiceProvider.GetRequiredService<Microsoft.EntityFrameworkCore.Query.Internal.ICompiledQueryCache>();
            var memoryCacheFieldInfo = compiledQueryCache.GetType().GetField("_memoryCache", BindingFlags.Instance | BindingFlags.NonPublic);

            var memoryCache = (Microsoft.Extensions.Caching.Memory.MemoryCache)memoryCacheFieldInfo.GetValue(compiledQueryCache);

            var memoryCacheEntriesFieldInfo = memoryCache.GetType().GetField("_entries", BindingFlags.Instance | BindingFlags.NonPublic);

            var memoryCacheEntries = (IDictionary)memoryCacheEntriesFieldInfo.GetValue(memoryCache); //(ConcurrentDictionary<object, ICacheEntry>)
            Console.WriteLine($"Cached #{memoryCacheEntries.Count}");
            foreach (var key in memoryCacheEntries.Keys) {
                Console.WriteLine(key.GetHashCode());
                var cacheEntry = (Microsoft.Extensions.Caching.Memory.ICacheEntry)memoryCacheEntries[key];
                var cacheEntryTarget = ((System.Delegate)cacheEntry.Value).Target;

                //gdamn where's closure gone!?
                var closureType = typeof(Expression).GetTypeInfo().Assembly.GetType("System.Runtime.CompilerServices.Closure");
                object[] closureConstants = null;
                if (cacheEntryTarget.GetType() == closureType) {
                    closureConstants = (object[])closureType.GetField("Constants").GetValue(cacheEntryTarget);
                } else {
                    closureConstants = new object[] { cacheEntryTarget };
                }
                var shaperCommandContexts = new List<ShaperCommandContext>();
                var l2 = closureConstants.Where(cx => cx is object[]).Cast<object[]>().SelectMany(xz => xz).OfType<ShaperCommandContext>();
                var l1 = closureConstants.Where(cx => cx is ShaperCommandContext).Cast<ShaperCommandContext>().ToList();
                shaperCommandContexts.AddRange(l2);
                shaperCommandContexts.AddRange(l1);

                Console.WriteLine(shaperCommandContexts.FirstOrDefault()?.QuerySqlGeneratorFactory?.Target?.ToString() ?? "blah blah compiledquery");

            }
        }

        public async void Test() {

        }
    }
}

