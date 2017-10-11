using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a named scope lifetime.
    /// </summary>
    public class NamedScopeLifetime : ScopedLifetimeBase
    {
        private static readonly MethodInfo GetScopeValueMethod = typeof(NamedScopeLifetime).GetSingleMethod(nameof(GetScopedValue), true);

        /// <summary>
        /// Constructs a <see cref="NamedScopeLifetime"/>.
        /// </summary>
        /// <param name="scopeName">The scope name.</param>
        public NamedScopeLifetime(object scopeName)
        {
            ScopeName = scopeName;
        }

        /// <summary>
        /// The name of the scoped lifetime.
        /// </summary>
        public object ScopeName { get; }

        /// <inheritdoc />
        public override ILifetime Create() => new NamedScopeLifetime(ScopeName);

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            var factory = base.GetFactoryDelegate(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
            if (factory == null)
                return null;

            var genericMethod = GetScopeValueMethod.MakeGenericMethod(resolveType);
            return Expression.Call(genericMethod, resolutionContext.CurrentScopeParameter, Expression.Constant(factory),
                Expression.Constant(base.ScopeId), Expression.Constant(this.ScopeName), Expression.Constant(resolveType));
        }

        private static TValue GetScopedValue<TValue>(IResolutionScope currentScope, Func<IResolutionScope, object> factory, object scopeId, object scopeName, Type resolveType)
        {
            var scope = currentScope;
            while (scope != null && scope.Name != scopeName)
                scope = scope.ParentScope;

            if (scope == null)
                throw new NamedScopeNotFoundException(resolveType, scopeName);

            return (TValue)scope.GetOrAddScopedItem(scopeId, factory);

        }
    }
}
