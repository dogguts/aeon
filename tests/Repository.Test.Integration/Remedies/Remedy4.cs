using System;
using System.Linq;
using Aeon.Core.Repository.Infrastructure;
using Model = Chinook.Repository.Model;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Aeon.Core.Repository;
using System.ComponentModel;
using System.Reflection;
using System.Collections;

namespace Chinook.Repository.Integration.Tests.Remedies {
    /// <summary>
    /// https://github.com/dogguts/aeon/issues/4   
    /// </summary>
    public class TestRemedy4 : IClassFixture<Remedy4Setup> {

        private readonly IRepository<Model.MediaType> _mediaTypeRepository;
        private readonly ServiceProvider _serviceProvider;
        public TestRemedy4(Remedy4Setup serviceSetup) {
            _serviceProvider = serviceSetup.ServiceProvider;
            _mediaTypeRepository = _serviceProvider.GetRequiredService<IRepository<Model.MediaType>>();
        }

        private static int MemoryCacheEntryCount(ServiceProvider _serviceProvider) {
            var compiledQueryCache = (Microsoft.EntityFrameworkCore.Query.Internal.CompiledQueryCache)_serviceProvider.GetRequiredService<Microsoft.EntityFrameworkCore.Query.Internal.ICompiledQueryCache>();
            var memoryCacheFieldInfo = compiledQueryCache.GetType().GetField("_memoryCache", BindingFlags.Instance | BindingFlags.NonPublic);
            var memoryCache = (Microsoft.Extensions.Caching.Memory.MemoryCache)memoryCacheFieldInfo.GetValue(compiledQueryCache);
            Console.WriteLine($"Cached #{memoryCache.Count}");
            return memoryCache.Count;
        }

        private static int? InitialCacheEntryCount = null;

        /// <summary>
        /// Subsequent queries with different parameters should only get cached once
        /// </summary>
        [Theory]
        [InlineData(1, "MP3")]
        [InlineData(2, "OGG")]
        [InlineData(3, "FLAC")]
        [InlineData(4, "OPUS")]
        [InlineData(5, "VOC")]
        public void Get(long id, string expectedMediaType) {
            if (!InitialCacheEntryCount.HasValue) {
                //get initial cache count
                InitialCacheEntryCount = MemoryCacheEntryCount(_serviceProvider);
            }

            var mediaType = _mediaTypeRepository.Get(id);
            Assert.Equal(expectedMediaType, mediaType.Name);

            var currentCacheEntryCount = MemoryCacheEntryCount(_serviceProvider);
            // the querycache should only grow with max. 2 entries over all the test data in this specific test.
            // +2 = 1(query .Count) +1(query)
            Assert.InRange(currentCacheEntryCount, InitialCacheEntryCount.Value, InitialCacheEntryCount.Value + 2);

        }

    }
}