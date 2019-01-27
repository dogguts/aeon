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
}