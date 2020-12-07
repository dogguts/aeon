using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;



namespace Aeon.Core.Repository {
    /// <summary>
    /// Utility extensions on IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions {

        /// <summary>
        /// Create and register a single repository TRepositoryService with specific implementation TRepositoryImplementation
        /// Synonymous with IServiceCollection.AddScoped&lt;TRepositoryService, TRepositoryImplementation&gt;();
        /// </summary>
        /// <typeparam name="TRepositoryService">The repository service type</typeparam>
        /// <typeparam name="TRepositoryImplementation">The repository implementation type</typeparam>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddRepository<TRepositoryService, TRepositoryImplementation>(this IServiceCollection services)
            where TRepositoryService : class
            where TRepositoryImplementation : class, TRepositoryService => services.AddScoped<TRepositoryService, TRepositoryImplementation>();


        /// <summary>
        /// Create and register repositories (both as Repository&lt;,&gt;:IRepository&lt;&gt; and ReadonlyRepository&lt;,&gt;:IReadonlyRepository&lt;&gt;) for all public DbSet&lt;&gt; properties found in DbContext TDbContext
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext to scan for DbSets</typeparam>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddRepositories<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
            => AddRepositories(services, typeof(TDbContext));

        /// <summary>
        /// Create and register readonly-repositories (as ReadonlyRepository&lt;,&gt;:IReadonlyRepository&lt;&gt;) for all public DbSet&lt;&gt; properties in DbContext TDbContext
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext to scan for DbSets</typeparam>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddReadonlyRepositories<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
            => AddReadonlyRepositories(services, typeof(TDbContext));

        /// <summary>
        /// Create and register readonly-repositories (as {implementationType}:IReadonlyRepository&lt;&gt;) for all public DbSet&lt;&gt; properties in DbContext TDbContext
        /// </summary>
        /// <typeparam name="TDbContext">The DbContext to scan for DbSets</typeparam>
        /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services to.</param>
        /// <param name="implementationType">The  System.Type of the repository implementation</param>
        /// <returns>The same service collection so that multiple calls can be chained.</returns>
        public static IServiceCollection AddReadonlyRepositories<TDbContext>(this IServiceCollection services, System.Type implementationType ) where TDbContext : DbContext
           => AddRepositoriesFromDbContexts(services, (typeof(IReadonlyRepository<>), implementationType), typeof(TDbContext));

        #region Private

        //NOTE: currently private, unsure if we want to keep this signature with System.Type, since System.Type can't be constrained at compile time (and since C# understandable doesn't do variadic generic arguments...)
        /// <summary>
        /// Create and register repositories (both as IRepository&lt;&gt; and IReadonlyRepository&lt;&gt;) for all DbSets found in each dbContextTypes
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="dbContextTypes">the DbContexts to scan for DbSets</param>
        /// <returns>IServiceCollection</returns>
        private static IServiceCollection AddRepositories(this IServiceCollection services, params Type[] dbContextTypes) =>
             AddRepositoriesFromDbContexts(services, (typeof(IRepository<>), typeof(Repository<,>)), dbContextTypes)
                    .AddReadonlyRepositories(dbContextTypes);

        //NOTE: currently private, unsure if we want to keep this signature with System.Type, since System.Type can't be constrained at compile time (and since C# understandable doesn't do variadic generic arguments...)
        /// <summary>
        /// Create and register readonly-repositories (IReadonlyRepository&lt;&gt;) for all DbSets found in each dbContextTypes
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="dbContextTypes">the DbContexts to scan for DbSets</param>
        /// <returns>IServiceCollection</returns>
        private static IServiceCollection AddReadonlyRepositories(this IServiceCollection services, params Type[] dbContextTypes) =>
            AddRepositoriesFromDbContexts(services, (typeof(IReadonlyRepository<>), typeof(ReadonlyRepository<,>)), dbContextTypes);

        //private static IServiceCollection AddReadonlyRepositories<TImplementation>(this IServiceCollection services, params Type[] dbContextTypes) =>
        //    AddRepositoriesFromDbContexts(services, (typeof(IReadonlyRepository<>), typeof(TImplementation)), dbContextTypes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="repositoryType">register as specific repository type; typeof(IRepository&lt;&gt;) or typeof(IReadonlyRepository&lt;&gt;)</param>
        /// <param name="dbContextTypes">the DbContexts to scan for DbSets</param>
        /// <returns>IServiceCollection</returns>
        /// <exception cref="NotSupportedException">The <paramref name="repositoryType"/> is not a supported type </exception>
        private static IServiceCollection AddRepositoriesFromDbContexts(this IServiceCollection services, (System.Type repositoryServiceType, System.Type repositoryImplementationType) repositoryType, params Type[] dbContextTypes) {
            if (repositoryType.repositoryServiceType != typeof(IRepository<>) && repositoryType.repositoryServiceType != typeof(IReadonlyRepository<>)) {
                throw new NotSupportedException(repositoryType.ToString());
            }

            foreach (var dbContextType in dbContextTypes.Where(c => typeof(DbContext).IsAssignableFrom(c))) {
                // enumerate only DbSet<> types
                var dbSetTypes = dbContextType.GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType?.GetGenericTypeDefinition() == typeof(DbSet<>));
                foreach (var dbSetType in dbSetTypes) {
                    // create IRepository service/interface type
                    var repositoryService = repositoryType.repositoryServiceType.MakeGenericType(dbSetType.PropertyType.GetGenericArguments());
                    // Build repository generic arguments
                    var repositoryImplementationArgs = new List<Type>(dbSetType.PropertyType.GetGenericArguments()) { dbContextType };
                    // create Repository<,> implementation type with  generic arguments
                    var repositoryImplementation = repositoryType.repositoryImplementationType.MakeGenericType(repositoryImplementationArgs.ToArray());
                    // Register service/interface and implementation type
                    services.AddScoped(repositoryService, repositoryImplementation);
                }
            }
            return services;
        }

        #endregion 
    }
}
