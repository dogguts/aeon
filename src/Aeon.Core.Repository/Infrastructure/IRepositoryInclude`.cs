using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aeon.Core.Repository.Infrastructure {

    /// <summary>
    /// Defines the basic functionality to include navigation properties.
    /// </summary>
    /// <typeparam name="T">The type of entity containing the navigation properties</typeparam>
    public interface IRepositoryInclude<T> {
        /// <summary>
        ///  Paths to include for type.
        /// </summary>
        IEnumerable<string> IncludePaths { get; }

        /// <summary>
        /// represents the paths to include.
        /// </summary>
        IReadOnlyList<RepositoryIncludeRoot<T>> Includes { get; }

        // /// <summary>
        // /// Expressions representing the paths to include.
        // /// </summary>
        // List<Expression<Func<T, object>>> Includes { get; }

        // /// <summary>
        // /// The dot-separated strings representing the paths to include.
        // /// </summary>
        // /// <value></value>
        // List<string> IncludeStrings { get; }
    }
}