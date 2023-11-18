using System.Linq.Expressions;
using Stashbox.Registration;
using Stashbox.Resolution;

namespace Stashbox.Lifetime;

internal class AutoLifetime : LifetimeDescriptor
{
    private LifetimeDescriptor selectedLifetime;

    internal override bool StoreResultInLocalVariable => this.selectedLifetime.StoreResultInLocalVariable;

    protected internal override int LifeSpan => this.selectedLifetime.LifeSpan;

    public AutoLifetime(LifetimeDescriptor boundaryLifetime)
    {
        this.selectedLifetime = boundaryLifetime;
    }

    private protected override Expression? BuildLifetimeAppliedExpression(ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation)
    {
        var tracker = new ResolutionContext.AutoLifetimeTracker();
        var context = resolutionContext.BeginAutoLifetimeTrackingContext(tracker);
        var expression = GetExpressionForRegistration(serviceRegistration, context, typeInformation);
        this.selectedLifetime = tracker.HighestRankingLifetime.LifeSpan <= this.selectedLifetime.LifeSpan
            ? tracker.HighestRankingLifetime
            : this.selectedLifetime;

        var func = expression?.CompileDelegate(context, context.CurrentContainerContext.ContainerConfiguration);
        if (func == null) return null;

        var final = Expression.Invoke(func.AsConstant(), context.CurrentScopeParameter, context.RequestContextParameter);
        
        return this.selectedLifetime.ApplyLifetimeToExpression(final, serviceRegistration, resolutionContext, typeInformation);
    }

    internal override Expression? ApplyLifetimeToExpression(Expression? expression,
        ServiceRegistration serviceRegistration,
        ResolutionContext resolutionContext, TypeInformation typeInformation) =>
        this.selectedLifetime.ApplyLifetimeToExpression(expression, serviceRegistration, resolutionContext, typeInformation);
}