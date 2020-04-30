using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a named scope lifetime.
    /// </summary>
    public class NamedScopeLifetime : LifetimeDescriptor
    {
        private static readonly MethodInfo GetScopeValueMethod = typeof(NamedScopeLifetime).GetSingleMethod(nameof(GetScopedValue), true);

        /// <inheritdoc />
        protected override Expression GetLifetimeAppliedExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType)
        {
            var factory = base.GetFactoryDelegate(containerContext, serviceRegistration, resolutionContext, resolveType);
            if (factory == null)
                return null;

            var genericMethod = GetScopeValueMethod.MakeGenericMethod(resolveType);
            return genericMethod.CallStaticMethod(resolutionContext.CurrentScopeParameter, factory.AsConstant(),
                serviceRegistration.RegistrationId.AsConstant(), serviceRegistration.RegistrationName.AsConstant(Constants.ObjectType),
                serviceRegistration.RegistrationContext.NamedScopeRestrictionIdentifier.AsConstant());
        }

        private static TValue GetScopedValue<TValue>(IResolutionScope currentScope, Func<IResolutionScope, object> factory,
            int scopeId, object sync, object scopeName)
        {
            var scope = currentScope;
            while (scope != null && scope.Name != scopeName)
                scope = scope.ParentScope;

            if (scope == null)
                throw new InvalidOperationException($"The scope '{scopeName}' not found.");

            return (TValue)scope.GetOrAddScopedObject(scopeId, sync, factory);

        }
    }
}
