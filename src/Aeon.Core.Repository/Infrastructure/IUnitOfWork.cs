using Microsoft.EntityFrameworkCore;

namespace Aeon.Core.Repository.Infrastructure {
    /// <summary>
    /// Defines the basic functionality of a unit of work
    /// </summary>
    public interface IUnitOfWork {
        /// <summary>
        /// Commits the unit of work
        /// </summary>
        void Commit();
    }

    /// <summary>
    /// Defines the basic functionality of a unit of work
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext {

    }
}