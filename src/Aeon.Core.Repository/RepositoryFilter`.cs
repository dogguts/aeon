using Aeon.Core.Repository.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Aeon.Core.Repository {
    /// <inheritdoc cref="IRepositoryFilter{TEntity}"/>
    public class RepositoryFilter<TEntity> : RepositoryInclude<TEntity>, IRepositoryFilter<TEntity> where TEntity : class {
        private readonly List<string> _tags = new();

        /// <inheritdoc/>
        public IEnumerable<string> Tags => _tags;
        /// <inheritdoc/>
        public RepositoryFilter(Expression<Func<TEntity, bool>> criteria, IRepositoryInclude<TEntity> includes = null) : base(includes) {
            Criteria = criteria;
        }

        /// <inheritdoc/>
        public Expression<Func<TEntity, bool>> Criteria { get; }

        /// <inheritdoc/>
        public IRepositoryFilter<TEntity> TagWith(string tag) {
            _tags.Add(tag);
            return this;
        }
    }
}

