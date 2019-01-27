using System.Collections.Generic;
using System.ComponentModel;
using Aeon.Core.Repository.Infrastructure;
using System.Linq;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public class RepositorySort<T> : IRepositorySort<T> {
        private IList<(ListSortDirection Direction, string Member)> _sorts;

        public RepositorySort(IList<(ListSortDirection Direction, string Member)> sorts) {
            Sorts = sorts;
        }
        public RepositorySort(params (ListSortDirection Direction, string Member)[] sorts) {
            Sorts = sorts.ToList();
        }

        public IList<(ListSortDirection Direction, string Member)> Sorts {
            get { return _sorts; }
            set { _sorts = value ?? new List<(ListSortDirection Direction, string Member)>(); }
        }
    }
}