using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// Defines the basic functionality of a repository filter.
    /// </summary>
    /// <typeparam name="T">The Entity type for the Filter</typeparam>
    public interface IRepositoryFilter<T> : IRepositoryInclude<T> where T : class {
        /// <summary>
        /// Criteria for the filter
        /// </summary>
        Expression<Func<T, bool>> Criteria { get; }
    }
}