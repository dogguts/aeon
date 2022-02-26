using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// Defines the basic functionality of a repository filter.
    /// </summary>
    /// <typeparam name="TEntity">The Entity type for the Filter</typeparam>
    public interface IRepositoryFilter<TEntity> : IRepositoryInclude<TEntity> where TEntity : class {

        /// <summary>
        /// The Tags associated with this repository filter
        /// </summary>
        public IEnumerable<string> Tags { get; }

        /// <summary>
        /// Criteria for the filter
        /// </summary>
        Expression<Func<TEntity, bool>> Criteria { get; }

        /// <summary>
        /// Adds a tag to the collection of tags associated with a repository filter. 
        /// Tags are query annotations that can provide contextual tracing information at different points in the query pipeline.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The same repository filter annotated with the given tag.</returns>
        public IRepositoryFilter<TEntity> TagWith(string tag);
    }
}