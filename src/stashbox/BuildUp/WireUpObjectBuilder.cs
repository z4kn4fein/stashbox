using Stashbox.BuildUp.Expressions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class WireUpObjectBuilder : ObjectBuilderBase
    {
        private volatile Expression expression;
        private readonly object syncObject = new object();
        private readonly IExpressionBuilder expressionBuilder;

        public WireUpObjectBuilder(IExpressionBuilder expressionBuilder)
        {
            this.expressionBuilder = expressionBuilder;
        }

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;

                var expr = this.expressionBuilder.CreateFillExpression(containerContext, serviceRegistration, Expression.Constant(serviceRegistration.RegistrationContext.ExistingInstance),
                    resolutionContext, serviceRegistration.ImplementationType);
                var factory = expr.CompileDelegate(resolutionContext);

                var instance = factory(resolutionContext.ResolutionScope);

                if (serviceRegistration.ShouldHandleDisposal && instance is IDisposable disposable)
                    resolutionContext.RootScope.AddDisposableTracking(disposable);

                if (serviceRegistration.RegistrationContext.Finalizer != null)
                {
                    var finalizerExpression = base.HandleFinalizer(Expression.Constant(instance), serviceRegistration,
                        Expression.Property(resolutionContext.CurrentScopeParameter, Constants.RootScopeProperty));
                    return this.expression = Expression.Constant(finalizerExpression.CompileDelegate(resolutionContext)(resolutionContext.ResolutionScope));
                }

                return this.expression = Expression.Constant(instance);
            }
        }

        public override IObjectBuilder Produce() => new WireUpObjectBuilder(this.expressionBuilder);

        public override bool HandlesObjectLifecycle => true;
    }
}