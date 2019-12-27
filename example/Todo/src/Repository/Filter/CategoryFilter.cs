using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Model = TodoApp.Model;

namespace TodoApp.Repository.Filter {
    public class CategoryFilter : RepositoryFilter<Model.Category> {
        public CategoryFilter(Expression<Func<Model.Category, bool>> criteria, IRepositoryInclude<Model.Category> includes = null) : base(criteria, includes) {
        }


        public static (RepositoryFilter<Model.Category> filter, RepositorySort<Model.Category> sort) AllAlphabetical(bool includeDeleted = false) {
            var filter = new CategoryFilter(c => !c.Deleted || includeDeleted);
            var sort = new RepositorySort<Model.Category>((ListSortDirection.Ascending, c => c.Title));
            return (filter, sort);
        }
    }
}