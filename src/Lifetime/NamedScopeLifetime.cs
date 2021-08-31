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
    public class NamedScopeLifetime : FactoryLifetimeDescriptor
    {
        private static readonly MethodInfo GetScopeValueMethod = typeof(NamedScopeLifetime).GetMethod(nameof(GetScopedValue), BindingFlags.Static | BindingFlags.NonPublic);

        /// <summary>
        /// The name of the scope where this lifetime activates.
        /// </summary>
        public readonly object ScopeName;

        /// <inheritdoc />
        protected override int LifeSpan { get; } = 10;

        /// <summary>
        /// Constructs a <see cref="NamedScopeLifetime"/>.
        /// </summary>
        /// <param name="scopeName"></param>
        public NamedScopeLifetime(object scopeName)
        {
            this.ScopeName = scopeName;
        }

        /// <inheritdoc />
        protected override Expression ApplyLifetime(Func<IResolutionScope, object> factory,
            ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType) =>
            GetScopeValueMethod.CallStaticMethod(
                resolutionContext.CurrentScopeParameter,
                    factory.AsConstant(),
                    resolveType.AsConstant(),
                    serviceRegistration.RegistrationId.AsConstant(),
                    this.ScopeName.AsConstant()).ConvertTo(resolveType);

        private static object GetScopedValue(IResolutionScope currentScope, Func<IResolutionScope, object> factory, Type requestedType,
            int scopeId, object scopeName)
        {
            var scope = currentScope;
            while (scope != null && scope.Name != scopeName)
                scope = scope.ParentScope;

            if (scope == null)
                throw new InvalidOperationException($"The scope '{scopeName}' not found.");

            return scope.GetOrAddScopedObject(scopeId, factory, requestedType);
        }
    }
}
