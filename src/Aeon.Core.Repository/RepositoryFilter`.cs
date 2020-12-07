using Aeon.Core.Repository.Infrastructure;
using System;
using System.Linq.Expressions;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public class RepositoryFilter<TEntity> : RepositoryInclude<TEntity>, IRepositoryFilter<TEntity> where TEntity : class {
        public RepositoryFilter(Expression<Func<TEntity, bool>> criteria, IRepositoryInclude<TEntity> includes = null) : base(includes) {
            Criteria = criteria;
        }

        public Expression<Func<TEntity, bool>> Criteria { get; }

    }
}

