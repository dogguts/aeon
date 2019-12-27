using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using AspNetCore.RouteAnalyzer;
using AutoMapper;
using Chinook.Model;
using Chinook.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Chinook.Web {
    public class Startup {
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] {
        new ConsoleLoggerProvider((category, level)
            => level == LogLevel.Trace, true)
        });

        public Startup(Microsoft.Extensions.Configuration.IConfiguration configuration) {
            Configuration = configuration;
        }

        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddDbContext<Chinook.Repository.ChinookDbContext>(opt => opt.UseSqlite("Data Source=chinook.sqlite")
                                                                                 .UseLoggerFactory(MyLoggerFactory));
            // services.AddDbContext<Chinook.Repository.LastFmDbContext>(opt => opt.UseSqlite("Data Source=lastfm.sqlite")
            //                                                                      .UseLoggerFactory(MyLoggerFactory));


            // IHttpClientFactory
            services.AddHttpClient();

            // used to cache lastfm api results
            services.AddMemoryCache();

            // Add automapper and mappings 
            services.AddAutoMapper(cfg => { }, typeof(Chinook.Mapping.AssemblyHook));

            // aeon: UoW
            services.AddScoped<Chinook.Repository.Infrastructure.IChinookDbUnitOfWork, ChinookDbUnitOfWork>();
            // services.AddScoped<Chinook.Repository.Infrastructure.ILastFmDbUnitOfWork, LastFmDbUnitOfWork>();

            // aeon: Register repositories
            services.AddScoped<IRepository<Album>, Repository<Album, ChinookDbContext>>();
            services.AddScoped<IRepository<Artist>, Repository<Artist, ChinookDbContext>>();
            services.AddScoped<IRepository<Track>, Repository<Track, ChinookDbContext>>();
            services.AddScoped<IRepository<Genre>, Repository<Genre, ChinookDbContext>>();
            services.AddScoped<IRepository<MediaType>, Repository<MediaType, ChinookDbContext>>();
            services.AddScoped<IRepository<InvoiceLine>, Repository<InvoiceLine, ChinookDbContext>>();

            // services.AddScoped<IRepository<Model.LastFm.Artist>, Repository<Model.LastFm.Artist, LastFmDbContext>>();
            //TODO: summernote for editing BBcode

            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            //Inject UrlHelper, used in GlobalSearchService to generate links
            //TODO: from 2.2-preview1 and up, should use LinkGenerator 
            services.AddScoped<IUrlHelper>(x => {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });

            //register services (general)
            services.AddScoped<Chinook.Service.IDateTimeService, Chinook.Service.DateTimeService>();
            services.AddScoped<Chinook.Service.IOverviewService, Chinook.Service.OverviewService>();
            services.AddScoped<Chinook.Service.IGlobalSearchService, Chinook.Service.GlobalSearchService>();

            //register services (viewmodel)

            services.AddScoped<Chinook.Service.ViewModel.IArtistViewModelService, Chinook.Service.ViewModel.ArtistViewModelService>();
            services.AddScoped<Chinook.Service.ViewModel.IAlbumViewModelService, Chinook.Service.ViewModel.AlbumViewModelService>();

            //register services (last.fm) 
            //TODO: dotnet user-secrets set "lastfm:key" "API key"
            //TODO: dotnet user-secrets set "lastfm:secret" "Shared secret"

            var lastfmSettings = Configuration.GetSection("lastfm")
                                              .Get<Service.LastFmService.LastFmSettings>();
            services.AddTransient<IF.Lastfm.Core.Api.LastfmClient>(svp => new IF.Lastfm.Core.Api.LastfmClient(lastfmSettings.Key, lastfmSettings.Secret));
            services.AddScoped<Chinook.Service.ILastFmService, Chinook.Service.LastFmService>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AutoMapper.IConfigurationProvider autoMapperConfigProvider) {
            //validate AutoMapper config/maps
            autoMapperConfigProvider.AssertConfigurationIsValid();


            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes => {
                if (env.IsDevelopment()) {
                    routes.MapRouteAnalyzer("/routes"); // RouteAnalyzer
                }

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Edit}/{id?}");
            });
        }
    }
}
