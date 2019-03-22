using System;
using System.Linq.Expressions;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Chinook.Repository.Model;

namespace Chinook.Repository.Filter {
    public class GenreFilter : RepositoryFilter<Chinook.Repository.Model.Genre> {
        public GenreFilter(Expression<Func<Chinook.Repository.Model.Genre, bool>> criteria, IRepositoryInclude<Chinook.Repository.Model.Genre> includes = null) : base(criteria, includes) {
        }

        /// <summary>
        /// Filter: Get Genre by name
        /// </summary>
        public static GenreFilter ByName(string name) {
            return new GenreFilter(g => g.Name == name);
        }
    }
}