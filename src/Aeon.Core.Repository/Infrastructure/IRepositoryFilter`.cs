using System;
using System.Linq.Expressions;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// Defines the basic functionality of a repository filter.
    /// </summary>
    /// <typeparam name="TEntity">The Entity type for the Filter</typeparam>
    public interface IRepositoryFilter<TEntity> : IRepositoryInclude<TEntity> where TEntity : class {
        /// <summary>
        /// Criteria for the filter
        /// </summary>
        Expression<Func<TEntity, bool>> Criteria { get; }
    }
}