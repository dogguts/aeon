using Aeon.Core.Repository.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Aeon.Core.Repository {
    /// <inheritdoc/>
    public class RepositoryFilter<TEntity> : RepositoryInclude<TEntity>, IRepositoryFilter<TEntity> where TEntity : class {
        /// <inheritdoc/>
        public RepositoryFilter(Expression<Func<TEntity, bool>> criteria, IRepositoryInclude<TEntity> includes = null) : base(includes) {
            Criteria = criteria;
        }

        /// <inheritdoc/>
        public Expression<Func<TEntity, bool>> Criteria { get; }

    }
}

