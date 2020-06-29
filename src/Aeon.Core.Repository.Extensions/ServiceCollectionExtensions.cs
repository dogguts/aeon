using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Aeon.Core.Repository {
    /// <summary>
    /// Utility extensions on IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions {
        /// <summary>
        /// Create and register repositories for DbSets found in DbContext TDbContext
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext to scan for DbSets</typeparam>
        /// <param name="services">IServiceCollection</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddRepositories<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
            => AddRepositories(services, typeof(TDbContext));

        /// <summary>
        /// Create and register repositories for DbSets found in each dbContextTypes
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="dbContextTypes">the DbContexts to scan for DbSets</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services, params Type[] dbContextTypes) {
            foreach (var dbContextType in dbContextTypes.Where(c => typeof(DbContext).IsAssignableFrom(c))) {
                // enumerate only DbSet<> types
                var dbSetTypes = dbContextType.GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType?.GetGenericTypeDefinition() == typeof(DbSet<>));
                foreach (var dbSetType in dbSetTypes) {
                    // create IRepository service/interface type
                    var repositoryService = typeof(IRepository<>).MakeGenericType(dbSetType.PropertyType.GetGenericArguments());
                    // Build repository generic arguments
                    var repositoryImplementationArgs = new List<Type>(dbSetType.PropertyType.GetGenericArguments()) { dbContextType };
                    // create Repository<,> implementation type with  generic arguments
                    var repositoryImplementation = typeof(Repository<,>).MakeGenericType(repositoryImplementationArgs.ToArray());
                    // Register service/interface and implementation type
                    services.AddScoped(repositoryService, repositoryImplementation);
                }
            }
            return services;
        }
    }
}
