using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// Defines the basic functionality to include navigation properties.
    /// </summary>
    /// <typeparam name="T">The type of entity containing the navigation properties</typeparam>
    public interface IRepositoryInclude<T> where T : class {
        /// <summary>
        /// The dot-separated strings representing the paths to include.
        /// </summary>
        /// <value></value>
        IEnumerable<string> IncludePaths { get; }
        /// <summary>
        ///  Paths to include for type.
        /// </summary>
        IReadOnlyList<IRepositoryIncludable<T>> Includes { get; }
    }
}