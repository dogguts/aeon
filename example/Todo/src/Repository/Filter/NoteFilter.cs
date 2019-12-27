using System;
using System.Linq.Expressions;
using Aeon.Core.Repository;
using Aeon.Core.Repository.Infrastructure;
using Model = TodoApp.Model;

namespace TodoApp.Repository.Filter {
    public class NoteFilter : RepositoryFilter<Model.Note> {
        public NoteFilter(Expression<Func<Model.Note, bool>> criteria, IRepositoryInclude<Model.Note> includes = null) : base(criteria, includes) {
        }
        static NoteFilter() {
            var includes = new RepositoryInclude<Model.Note>();
            includes.Include((a) => a.Category);
            includes.Include((a) => a.NoteItems);
            DefaultIncludes = includes;
        }
        private static IRepositoryInclude<Model.Note> DefaultIncludes { get; }


        public static NoteFilter ById(long id) {
            return new NoteFilter(n => n.NoteId == id, DefaultIncludes);
        }
        public static NoteFilter All() {
            return new NoteFilter(n => true, DefaultIncludes);
        }
    }
}