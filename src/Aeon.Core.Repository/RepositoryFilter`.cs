using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Aeon.Core.Repository.Infrastructure;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public class RepositoryFilter<T> : RepositoryInclude<T>, IRepositoryFilter<T> {
        public RepositoryFilter(Expression<Func<T, bool>> criteria, IRepositoryInclude<T> includes = null):base(includes) {
            Criteria = criteria;
         }

        public Expression<Func<T, bool>> Criteria { get; }

    }
}

