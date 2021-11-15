using Aeon.Core.Repository.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Aeon.Core.Repository {
    /// <inheritdoc/>
    public class RepositoryInclude<TEntity> : IRepositoryInclude<TEntity>, IRepositoryIncludable<TEntity> where TEntity : class {
        private readonly List<IRepositoryIncludable<TEntity>> _includes = new();

        /// <inheritdoc/>
        public RepositoryInclude() { }

        /// <inheritdoc/>
        public RepositoryInclude(IRepositoryInclude<TEntity> repositoryInclude) {
            if (repositoryInclude != null) {
                _includes = new List<IRepositoryIncludable<TEntity>>(repositoryInclude.Includes);
            }
        }

        internal IRepositoryIncludable<TEntity> AddInclude(IRepositoryIncludable<TEntity> inc) {
            _includes.Add(inc);
            return inc;
        }

        /// <inheritdoc/>
        public IEnumerable<string> IncludePaths => _includes.Select(i => i.ToString());

        /// <inheritdoc/>
        public IReadOnlyList<IRepositoryIncludable<TEntity>> Includes => _includes;

        /// <inheritdoc/>
        public string Name { get; set; }

        //TODO: internal?
        /// <inheritdoc/>
        public IRepositoryIncludable<TEntity> Previous => throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public interface IRepositoryIncludable<out T> where T : class {
        /// <inheritdoc/>
        string Name { get; set; }
        /// <inheritdoc/>
        IRepositoryIncludable<T> Previous { get; }
    }

    /// <inheritdoc/>
    public interface IRepositoryIncludable<out T, out P> : IRepositoryIncludable<T> where T : class {
    }

    /// <inheritdoc/>
    public interface IRepositoryIncludeExpression<T, out P> : IRepositoryIncludable<T, P> where T : class {
        /// <inheritdoc/>
        IRepositoryIncludable<T> Next { get; set; }
    }

    /// <inheritdoc/>
    public class RepositoryIncludeExpression<T, P> : IRepositoryIncludeExpression<T, P> where T : class {
        /// <inheritdoc/>
        public IRepositoryIncludable<T> Previous { get; private set; }
        /// <inheritdoc/>
        public RepositoryIncludeExpression(IRepositoryIncludable<T> previous) {
            Previous = previous;
            Name = Previous.Name;
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public override string ToString() {
            return (Name + "." + Next?.ToString()).TrimEnd('.');
        }

        /// <inheritdoc/>
        public IRepositoryIncludable<T> Next { get; set; }
    }

    /// <inheritdoc/>
    public static class RepositoryIncludeExtensions {
        /// <inheritdoc/>
        public static IRepositoryIncludable<TEntity, TProperty> Include<TEntity, TProperty>(this IRepositoryIncludable<TEntity> source, Expression<Func<TEntity, TProperty>> navigationPropertyPath) where TEntity : class {
            // find root (in case of Include after ThenInclude)
            while (source is not RepositoryInclude<TEntity>) {
                source = source.Previous;
            }

            var sourceO = (RepositoryInclude<TEntity>)source;

            var rootInc = new RepositoryIncludeExpression<TEntity, TProperty>(source) { Name = (navigationPropertyPath.Body as MemberExpression)?.Member?.Name ?? "!!" };
            sourceO.AddInclude(rootInc);

            return rootInc;
        }
        /// <inheritdoc/>
        public static IRepositoryIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IRepositoryIncludable<TEntity, TPreviousProperty> source, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath) where TEntity : class {
            var sourceInt = (IRepositoryIncludeExpression<TEntity, TPreviousProperty>)source;

            var inc = new RepositoryIncludeExpression<TEntity, TProperty>(sourceInt) { Name = (navigationPropertyPath.Body as MemberExpression)?.Member?.Name ?? "!!" }; //, navigationPropertyPath.Body);
            sourceInt.Next = inc;
            return inc;
        }

        /// <inheritdoc/>
        public static IRepositoryIncludable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IRepositoryIncludable<TEntity, IEnumerable<TPreviousProperty>> source, Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath) where TEntity : class {
            var sourceInt = (IRepositoryIncludeExpression<TEntity, IEnumerable<TPreviousProperty>>)source;

            var inc = new RepositoryIncludeExpression<TEntity, TProperty>(source) { Name = (navigationPropertyPath.Body as MemberExpression)?.Member?.Name ?? "!!" }; //, navigationPropertyPath.Body);
            sourceInt.Next = inc;
            return inc;
        }
    }



}

