using Stashbox.BuildUp;
using Stashbox.Registration;
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
            this.ScopeName = scopeName;
        }

        /// <summary>
        /// The name of the scoped lifetime.
        /// </summary>
        public object ScopeName { get; }

        /// <inheritdoc />
        public override ILifetime Create() => new NamedScopeLifetime(this.ScopeName);

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            var factory = base.GetFactoryExpression(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
            if (factory == null)
                return null;

            var genericMethod = GetScopeValueMethod.MakeGenericMethod(resolveType);
            return genericMethod.InvokeMethod(resolutionContext.CurrentScopeParameter, factory,
                base.ScopeId.AsConstant(), base.Sync.AsConstant(), this.ScopeName.AsConstant());
        }

        private static TValue GetScopedValue<TValue>(IResolutionScope currentScope, Func<IResolutionScope, object> factory, int scopeId, object sync, object scopeName)
        {
            var scope = currentScope;
            while (scope != null && scope.Name != scopeName)
                scope = scope.ParentScope;

            if (scope == null)
                throw new InvalidOperationException($"The scope '{scopeName}' not found.");

            return (TValue)scope.GetOrAddScopedItem(scopeId, sync, factory);

        }
    }
}
