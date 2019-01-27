using System.Collections.Generic;
using System.ComponentModel;

namespace Aeon.Core.Repository.Infrastructure {
        /// <summary>
    /// Defines the basic functionality of a repository sort.
    /// </summary>
    /// <typeparam name="T">The Entity type for the Sort</typeparam>
    public interface IRepositorySort<T> {
        /// <summary>
        /// Sort specifications
        /// </summary>
        /// <value></value>
        IList<(ListSortDirection Direction, string Member)> Sorts { get; set; }
    }
}