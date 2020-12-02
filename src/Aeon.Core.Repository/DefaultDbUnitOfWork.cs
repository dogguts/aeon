using Aeon.Core.Repository.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aeon.Core.Repository {
    /// <summary>
    /// A simple IUnitOfWork implemenation, calls SaveChanges() on a TContext instance
    /// </summary>
    /// <typeparam name="TContext">the DbContext type SaveChanges should be invoked for</typeparam>
    public class DefaultDbUnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext {
        private readonly TContext _dbContext;

        /// <summary>
        /// Creates a new DefaultDbUnitOfWork for dbContext
        /// </summary>
        /// <param name="dbContext">The DbContext this DefaultDbUnitOfWork is handling</param>
        public DefaultDbUnitOfWork(TContext dbContext) {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Save the changes from the TContext instance to the database
        /// </summary>
        public void Commit() {
            _dbContext.SaveChanges();
        }
    }
}