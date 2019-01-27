using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Aeon.Core.Repository.Infrastructure;

#pragma warning disable 1591 //docs are in interface specifications

namespace Aeon.Core.Repository {
    /// <summary>
    /// Represents the navigation properties to include.
    /// </summary>
    /// <typeparam name="T">The type of entity containing the navigation properties</typeparam>
    public class RepositoryInclude<T> : IRepositoryInclude<T> {
        public RepositoryInclude() { }
        public RepositoryInclude(IRepositoryInclude<T> repositoryInclude) {
            if (repositoryInclude != null) {
                _includes = new List<RepositoryIncludeRoot<T>>(repositoryInclude.Includes);
            }
        }
        internal List<RepositoryIncludeRoot<T>> _includes { get; private set; } = new List<RepositoryIncludeRoot<T>>();
        public IEnumerable<string> IncludePaths => _includes.Select(i => i.ToString());

        public IReadOnlyList<RepositoryIncludeRoot<T>> Includes => _includes;

        // public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        // public List<string> IncludeStrings { get; } = new List<string>();
    }

    /// <summary>
    /// Creates a new first level include
    /// </summary>
    public static class RepositoryIncludeExtensions {
        public static RepositoryIncludeExpression<P> Include<T, P>(this RepositoryInclude<T> repository, Expression<Func<T, P>> expr) {
            var root = new RepositoryIncludeRoot<T>();
            repository._includes.Add(root);
            return root.Include<P>(expr);
        }
        public static RepositoryIncludeExpression<P> Include<T, P>(this RepositoryInclude<T> repository, Expression<Func<T, ICollection<P>>> expr) {
            var root = new RepositoryIncludeRoot<T>();
            repository._includes.Add(root);
            return root.Include<P>(expr);
        }
    }

    /// <summary>
    /// First level include definition
    /// </summary>
    /// <typeparam name="T">The type of entity containing the navigation properties</typeparam>
    public class RepositoryIncludeRoot<T> : RepositoryIncludeExpression {
        public RepositoryIncludeExpression<P> Include<P>(Expression<Func<T, P>> expr) {
            return Set(expr);
        }
        public RepositoryIncludeExpression<P> Include<P>(Expression<Func<T, ICollection<P>>> expr) {
            return Set(expr);
        }
    }

    public abstract class RepositoryIncludeExpression {
        protected string Name;
        protected RepositoryIncludeExpression Child;

        public override string ToString() {
            return (Name + "." + Child?.ToString()).TrimEnd('.');
        }

        private RepositoryIncludeExpression<P> Set<P>(Expression body) {
            Name = (body as MemberExpression)?.Member?.Name;
            var nextInclude = new RepositoryIncludeExpression<P>();
            Child = nextInclude;
            return nextInclude;
        }

        protected RepositoryIncludeExpression<P> Set<T, P>(Expression<Func<T, P>> expr) {
            return Set<P>(expr.Body);
        }
        protected RepositoryIncludeExpression<P> Set<T, P>(Expression<Func<T, ICollection<P>>> expr) {
            return Set<P>(expr.Body);
        }
    }

    /// <summary>
    /// Subsequent include definition
    /// </summary>
    /// <typeparam name="T">The type of entity containing the navigation properties</typeparam>
    public class RepositoryIncludeExpression<T> : RepositoryIncludeExpression {

        public RepositoryIncludeExpression<P> ThenInclude<P>(Expression<Func<T, P>> expr) {
            return Set(expr);
        }
        public RepositoryIncludeExpression<P> ThenInclude<P>(Expression<Func<T, ICollection<P>>> expr) {
            return Set(expr);
        }
    }

}

