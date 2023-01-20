using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Utils;

namespace Stashbox.Lifetime;

/// <summary>
/// Represents a named scope lifetime.
/// </summary>
public class NamedScopeLifetime : FactoryLifetimeDescriptor
{
    private static readonly MethodInfo GetScopeValueMethod = TypeCache<NamedScopeLifetime>.Type.GetMethod(nameof(GetScopedValue), BindingFlags.Static | BindingFlags.NonPublic)!;

    /// <summary>
    /// The name of the scope where this lifetime activates.
    /// </summary>
    public readonly object ScopeName;

    /// <inheritdoc />
    protected override int LifeSpan => 10;

    /// <summary>
    /// Constructs a <see cref="NamedScopeLifetime"/>.
    /// </summary>
    /// <param name="scopeName"></param>
    public NamedScopeLifetime(object scopeName)
    {
        this.ScopeName = scopeName;
    }

    /// <inheritdoc />
    protected override Expression ApplyLifetime(Func<IResolutionScope, IRequestContext, object> factory,
        ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType) =>
        GetScopeValueMethod.CallStaticMethod(resolutionContext.CurrentScopeParameter,
            resolutionContext.RequestContextParameter,
            factory.AsConstant(),
            serviceRegistration.ImplementationType.AsConstant(),
            serviceRegistration.RegistrationId.AsConstant(),
            this.ScopeName.AsConstant()).ConvertTo(resolveType);

    private static object GetScopedValue(IResolutionScope currentScope, IRequestContext requestContext,
        Func<IResolutionScope, IRequestContext, object> factory, Type serviceType, int scopeId, object scopeName)
    {
        var scope = currentScope;
        while (scope != null && !scopeName.Equals(scope.Name))
            scope = scope.ParentScope;

        if (scope == null)
            throw new ResolutionFailedException(serviceType, message: $"The scope '{scopeName}' was not found to resolve {serviceType.FullName} with named scope lifetime.");

        return scope.GetOrAddScopedObject(scopeId, factory, requestContext, serviceType);
    }
}