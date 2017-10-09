using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
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

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;

                var expr = this.expressionBuilder.CreateFillExpression(containerContext, serviceRegistration, Expression.Constant(serviceRegistration.RegistrationContext.ExistingInstance),
                    resolutionInfo, serviceRegistration.ImplementationType);
                var factory = expr.CompileDelegate(resolutionInfo.CurrentScopeParameter);

                var instance = factory(resolutionInfo.ResolutionScope);

                if (serviceRegistration.ShouldHandleDisposal && instance is IDisposable disposable)
                    resolutionInfo.RootScope.AddDisposableTracking(disposable);

                if (serviceRegistration.RegistrationContext.Finalizer != null)
                {
                    var finalizerExpression = base.HandleFinalizer(Expression.Constant(instance), serviceRegistration, resolutionInfo);
                    return this.expression = Expression.Constant(finalizerExpression.CompileDelegate(resolutionInfo.CurrentScopeParameter)(resolutionInfo.ResolutionScope));
                }

                return this.expression = Expression.Constant(instance);
            }
        }

        public override IObjectBuilder Produce() => new WireUpObjectBuilder(this.expressionBuilder);

        public override bool HandlesObjectLifecycle => true;
    }
}