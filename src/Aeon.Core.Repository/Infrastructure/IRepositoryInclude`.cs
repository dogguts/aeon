using System.Collections.Generic;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// Defines the basic functionality to include navigation properties.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity containing the navigation properties</typeparam>
    public interface IRepositoryInclude<TEntity> where TEntity : class {
        /// <summary>
        /// The dot-separated strings representing the paths to include.
        /// </summary>
        /// <value></value>
        IEnumerable<string> IncludePaths { get; }
        /// <summary>
        ///  Paths to include for type.
        /// </summary>
        IReadOnlyList<IRepositoryIncludable<TEntity>> Includes { get; }
    }
}