using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime;

/// <summary>
/// Represents a lifetime descriptor which applies to expressions.
/// </summary>
public abstract class ExpressionLifetimeDescriptor : LifetimeDescriptor
{
    private protected override Expression? BuildLifetimeAppliedExpression(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var expression = GetExpressionForRegistration(serviceRegistration, resolutionContext, typeInformation);
        return expression == null ? null : this.ApplyLifetime(expression, serviceRegistration, resolutionContext, typeInformation.Type);
    }

    /// <summary>
    /// Derived types are using this method to apply their lifetime to the instance creation.
    /// </summary>
    /// <param name="expression">The expression the lifetime should apply to.</param>
    /// <param name="serviceRegistration">The service registration.</param>
    /// <param name="resolutionContext">The info about the actual resolution.</param>
    /// <param name="resolveType">The type of the resolved service.</param>
    /// <returns>The lifetime managed expression.</returns>
    protected abstract Expression ApplyLifetime(Expression expression, ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, Type resolveType);
}