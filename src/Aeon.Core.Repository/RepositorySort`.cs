using System.Collections.Generic;
using System.ComponentModel;
using Aeon.Core.Repository.Infrastructure;
using System.Linq;
using System;
using System.Linq.Expressions;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public class RepositorySort<TEntity> : IRepositorySort<TEntity> {
        private IList<(ListSortDirection Direction, Expression<Func<TEntity, object>>)> _sorts;

        public RepositorySort(params (ListSortDirection Direction, Expression<Func<TEntity, object>> KeySelector)[] sorts) {
            _sorts = sorts.ToList();
        }
        public RepositorySort(IList<(ListSortDirection Direction, Expression<Func<TEntity, object>> KeySelector)> sorts) {
            _sorts = sorts;
        }

        public IList<(ListSortDirection Direction, Expression<Func<TEntity, object>> KeySelector)> Sorts {
            get { return _sorts; }
            set { _sorts = value ?? new List<(ListSortDirection Direction, Expression<Func<TEntity, object>> KeySelector)>(); }
        }
    }
}