using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aeon.Core.Repository.Infrastructure;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    public class RepositoryInclude<T> : IRepositoryInclude<T>, IRepositoryIncludable<T> where T : class {
        private List<IRepositoryIncludable<T>> _includes = new List<IRepositoryIncludable<T>>();

        public RepositoryInclude() { }
        public RepositoryInclude(IRepositoryInclude<T> repositoryInclude) {
            if (repositoryInclude != null) {
                _includes = new List<IRepositoryIncludable<T>>(repositoryInclude.Includes);
            }
        }

        internal IRepositoryIncludable<T> AddInclude(IRepositoryIncludable<T> inc) {
            _includes.Add(inc);
            return inc;
        }

        public IEnumerable<string> IncludePaths => _includes.Select(i => i.ToString());
        public IReadOnlyList<IRepositoryIncludable<T>> Includes => _includes;

        public String Name { get; set; }

        public IRepositoryIncludable<T> Previous => throw new NotImplementedException();
    }

    public interface IRepositoryIncludable<out T> where T : class {
        String Name { get; set; }
        IRepositoryIncludable<T> Previous { get; }
    }

    public interface IRepositoryIncludable<out T, out P> : IRepositoryIncludable<T> where T : class {
    }


    public interface IRepositoryIncludeExpression<T, out P> : IRepositoryIncludable<T, P> where T : class {
        IRepositoryIncludable<T> Next { get; set; }
    }

    public class RepositoryIncludeExpression<T, P> : IRepositoryIncludeExpression<T, P> where T : class {
        public IRepositoryIncludable<T> Previous { get; private set; }

        public RepositoryIncludeExpression(IRepositoryIncludable<T> previous) {
            Previous = previous;
            Name = Previous.Name;

            //Next = _queryable;
        }

        public String Name { get; set; }
        //public Expression Expression { get; set; }

        public override string ToString() {
            return (Name + "." + Next?.ToString()).TrimEnd('.');
        }

        public IRepositoryIncludable<T> Next { get; set; }
    }

    public static class RepositoryIncludeExtensions {
        public static IRepositoryIncludable<TEntity, TProperty> Include<TEntity, TProperty>(this IRepositoryIncludable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class {
            Console.WriteLine((navigationPropertyPath.Body as MemberExpression)?.Member?.Name);

            // find root (in case of Include after ThenInclude)
            while (!(source is RepositoryInclude<TEntity>)) {
                source = source.Previous;
            }

            var sourceO = (RepositoryInclude<TEntity>)source;

            var rootInc = new RepositoryIncludeExpression<TEntity, TProperty>(source) { Name = (navigationPropertyPath.Body as MemberExpression)?.Member?.Name ?? "!!" };
            sourceO.AddInclude(rootInc);

            return rootInc;
        }

        public static IRepositoryIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IRepositoryIncludable<TEntity, TPreviousProperty> source, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath) where TEntity : class {
            var sourceInt = (IRepositoryIncludeExpression<TEntity, TPreviousProperty>)source;

            var inc = new RepositoryIncludeExpression<TEntity, TProperty>(sourceInt) { Name = (navigationPropertyPath.Body as MemberExpression)?.Member?.Name ?? "!!" }; //, navigationPropertyPath.Body);
            sourceInt.Next = inc;
            return inc;
        }

        public static IRepositoryIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IRepositoryIncludable<TEntity, IEnumerable<TPreviousProperty>> source, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath) where TEntity : class {
            var sourceInt = (IRepositoryIncludeExpression<TEntity, IEnumerable<TPreviousProperty>>)source;

            var inc = new RepositoryIncludeExpression<TEntity, TProperty>(source) { Name = (navigationPropertyPath.Body as MemberExpression)?.Member?.Name ?? "!!" }; //, navigationPropertyPath.Body);
            sourceInt.Next = inc;
            return inc;
        }
    }



}

