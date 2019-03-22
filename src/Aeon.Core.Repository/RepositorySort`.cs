using System.Collections.Generic;
using System.ComponentModel;
using Aeon.Core.Repository.Infrastructure;
using System.Linq;
using System;
using System.Linq.Expressions;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public class RepositorySort<T> : IRepositorySort<T> {
        private IList<(ListSortDirection Direction, Expression<Func<T, object>>)> _sorts;

        public RepositorySort(params (ListSortDirection Direction, Expression<Func<T, object>> KeySelector)[] sorts) {
            _sorts = sorts.ToList();
        }
        public RepositorySort(IList<(ListSortDirection Direction, Expression<Func<T, object>> KeySelector)> sorts) {
            _sorts = sorts;
        }

        public IList<(ListSortDirection Direction, Expression<Func<T, object>> KeySelector)> Sorts {
            get { return _sorts; }
            set { _sorts = value ?? new List<(ListSortDirection Direction, Expression<Func<T, object>> KeySelector)>(); }
        }
    }
}