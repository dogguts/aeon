using System;
using System.Linq;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace TodoApp.Test {
    public class Vm {
        public long? VmId { get; set; }
        public string VmTitle { get; set; }
        public bool? VmCompleted { get; set; }
    }
    public class Db {
        public long DbId { get; set; }
        public string DbTitle { get; set; }
        public bool DbCompleted { get; set; }
    }
    class AutoMapperNullProfile : Profile {
        public AutoMapperNullProfile() {
            //CreateMap<bool?, bool>()

            CreateMap<Vm, Db>()
                .ForMember(dst => dst.DbId, opt => opt.MapFrom(src => src.VmId))
                .ForMember(dst => dst.DbTitle, opt => opt.MapFrom(src => src.VmTitle))
                .ForMember(dst => dst.DbCompleted, opt => {
                    //opt.Condition(src => src.VmCompleted.HasValue);
                    opt.MapFrom(src => src.VmCompleted);
                });


            // .ForAllMembers(opt => opt.Condition((src, dst, srcm, dstm) => srcm != null));

            //.ForAllMembers(opt=>     opt.Condition(src => !src. ));
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

            services.AddDbContext<TodoApp.Repository.TodoAppDbContext>(opt => opt.UseSqlite("Data Source=TodoAppDb.sqlite")
                                                                                 .UseLoggerFactory(MyLoggerFactory));

            // automapper
            services.AddAutoMapper(cfg => {
                /*cfg.ForAllPropertyMaps(pm => (pm.SourceType != null && (Nullable.GetUnderlyingType(pm.SourceType) == pm.DestinationType || pm.SourceType == typeof(string))),
                (pm, c) => c.MapFrom<object, object, object, object>(new Mapping.IgnoreNullSourceMemberValueResolver(), pm.SourceMember.Name));*/
            }, typeof(TodoApp.Mapping.AssemblyHook), typeof(TodoApp.Test.AutoMapperNullProfile));


            // aeon: UoW
            services.AddScoped<TodoApp.Repository.Infrastructure.ITodoAppDbUnitOfWork, TodoApp.Repository.TodoAppSimpleDbUnitOfWork>();

            // aeon: Register repositories
            services.AddScoped<IRepository<TodoApp.Model.Note>, Repository<TodoApp.Model.Note, TodoApp.Repository.TodoAppDbContext>>();
            services.AddScoped<IRepository<TodoApp.Model.NoteItem>, Repository<TodoApp.Model.NoteItem, TodoApp.Repository.TodoAppDbContext>>();
            services.AddScoped<IRepository<TodoApp.Model.Category>, Repository<TodoApp.Model.Category, TodoApp.Repository.TodoAppDbContext>>();

            // services 
            services.AddScoped<Service.Infrastructure.INoteService, Service.NoteService>();
            // services.AddScoped<Chinook.Service.IOverviewService, Chinook.Service.OverviewService>();

            // automapper
            services.AddScoped<Program>();

            ServiceProvider = services.BuildServiceProvider();

            //var dx = new TodoApp.Repository.TodoAppDbContext();
            //dx.Categories.Include(x=>x.Deleted) .IgnoreQueryFilters   ();
            var dbx = ServiceProvider.GetRequiredService<TodoApp.Repository.TodoAppDbContext>();
            

            var uow = ServiceProvider.GetRequiredService<TodoApp.Repository.Infrastructure.ITodoAppDbUnitOfWork>();
            var catRepo = ServiceProvider.GetRequiredService<IRepository<TodoApp.Model.Category>>();

            var cat = catRepo.All().First();
            catRepo.Delete(cat);
            uow.Commit();

            Console.ReadLine();
        }
    }
}
