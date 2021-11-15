using Aeon.Core.Repository.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace Aeon.Core.Repository {
    /// <inheritdoc/>
    public class RepositorySort<TEntity> : IRepositorySort<TEntity> {
        private IList<(ListSortDirection Direction, Expression<Func<TEntity, object>>)> _sorts;

        /// <inheritdoc/>
        public RepositorySort(params (ListSortDirection Direction, Expression<Func<TEntity, object>> KeySelector)[] sorts) {
            _sorts = sorts.ToList();
        }

        /// <inheritdoc/>
        public RepositorySort(IList<(ListSortDirection Direction, Expression<Func<TEntity, object>> KeySelector)> sorts) {
            _sorts = sorts;
        }

        /// <inheritdoc/>
        public IList<(ListSortDirection Direction, Expression<Func<TEntity, object>> KeySelector)> Sorts {
            get { return _sorts; }
            set { _sorts = value ?? new List<(ListSortDirection Direction, Expression<Func<TEntity, object>> KeySelector)>(); }
        }
    }
}