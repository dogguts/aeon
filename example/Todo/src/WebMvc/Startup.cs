using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;

namespace TodoApp.WebMvc {
    public class Startup {
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] {
        new ConsoleLoggerProvider((category, level)
            => level == LogLevel.Trace, true)
        });

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddDbContext<TodoApp.Repository.TodoAppDbContext>(opt => opt.UseSqlite("Data Source=TodoAppDb.sqlite")
                                                                               .UseLoggerFactory(MyLoggerFactory));


            // IHttpClientFactory
            services.AddHttpClient();

            // Add automapper and mappings 
            services.AddAutoMapper(cfg => {
                //don't map:
                // * System.Nullable with value==null (aka !value.HasValue)
                // * System.String with value==null (empty strings are mapped!)
                /*cfg.ForAllPropertyMaps(
                    pm => (pm.SourceType != null && (Nullable.GetUnderlyingType(pm.SourceType) == pm.DestinationType || pm.SourceType == typeof(string))),
                    (pm, c) => c.MapFrom<object, object, object, object>(new Mapping.IgnoreNullSourceMemberValueResolver(), pm.SourceMember.Name));*/
            }, typeof(TodoApp.Mapping.AssemblyHook));

            // aeon: UoW
            services.AddScoped<TodoApp.Repository.Infrastructure.ITodoAppDbUnitOfWork, TodoApp.Repository.TodoAppSimpleDbUnitOfWork>();

            // aeon: Register repositories
            services.AddScoped<IRepository<TodoApp.Model.Note>, Repository<TodoApp.Model.Note, TodoApp.Repository.TodoAppDbContext>>();
            services.AddScoped<IRepository<TodoApp.Model.NoteItem>, Repository<TodoApp.Model.NoteItem, TodoApp.Repository.TodoAppDbContext>>();
            services.AddScoped<IRepository<TodoApp.Model.Category>, Repository<TodoApp.Model.Category, TodoApp.Repository.TodoAppDbContext>>();

            // services 
            services.AddScoped<Service.Infrastructure.INoteService, Service.NoteService>();
            services.AddScoped<Service.Infrastructure.ICategoryService, Service.CategoryService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AutoMapper.IConfigurationProvider autoMapperConfigProvider) {
            //validate AutoMapper config/maps
            autoMapperConfigProvider.AssertConfigurationIsValid();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
