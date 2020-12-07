using Aeon.Core.Repository.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using View = Chinook.Repository.Model.View;

namespace Chinook.Repository.Integration.Tests {
    public class ServiceCollectionExtensionsTest : IClassFixture<ServiceCollectionExtensionsSetup> {
        private readonly ServiceProvider _serviceProvider;
        private readonly IServiceCollection _serviceCollection;

        public static IEnumerable<object[]> ExpectRepositories =>
            new List<object[]> {
                 new object[] {typeof(IRepository<Model.Album>)},
                 new object[] {typeof(IRepository<Model.Artist>)},
                 new object[] {typeof(IRepository<Model.Customer>)},
                 new object[] {typeof(IRepository<Model.Employee>)},
                 new object[] {typeof(IRepository<Model.Genre>)},
                 new object[] {typeof(IRepository<Model.Invoice>)},
                 new object[] {typeof(IRepository<Model.InvoiceLine>)},
                 new object[] {typeof(IRepository<Model.MediaType>)},
                 new object[] {typeof(IRepository<Model.Playlist>)},
                 new object[] {typeof(IRepository<Model.PlaylistTrack>)},
                 new object[] {typeof(IRepository<Model.Track>)},
                 new object[] {typeof(IRepository<View.AlbumCountByArtists>)}
            };

        public ServiceCollectionExtensionsTest(ServiceCollectionExtensionsSetup serviceSetup) {
            _serviceProvider = serviceSetup.ServiceProvider;
            _serviceCollection = serviceSetup.Services;
        }


        /// <summary>
        /// Check if number of expected repositories equals the actual registered number of repositories
        /// </summary>
        [Fact]
        public void NumberOfRegisteredRepositories() {
            var services = _serviceCollection.Select(sd => sd.ServiceType).ToList();
            var repositoryServices = services.Where(svc => svc.IsGenericType && svc.GetGenericTypeDefinition() == typeof(Aeon.Core.Repository.Infrastructure.IRepository<>));
            Assert.Equal(ExpectRepositories.Count(), repositoryServices.Count());
        }

        /// <summary>
        /// Try get all expected repositories
        /// </summary>
        [Theory]
        [MemberData(nameof(ExpectRepositories))]
        public void AddRepositoriesExtension(System.Type expectRepository) {
            _serviceProvider.GetRequiredService(expectRepository);
        }
    }
}
