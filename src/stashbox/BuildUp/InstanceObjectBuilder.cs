using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class InstanceObjectBuilder : ObjectBuilderBase
    {
        private volatile Expression expression;
        private readonly object syncObject = new object();

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;

                if (serviceRegistration.ShouldHandleDisposal && serviceRegistration.RegistrationContext.ExistingInstance is IDisposable disposable)
                    resolutionInfo.RootScope.AddDisposableTracking(disposable);

                if (serviceRegistration.RegistrationContext.Finalizer != null)
                {
                    var finalizerExpression = base.HandleFinalizer(Expression.Constant(serviceRegistration.RegistrationContext.ExistingInstance), serviceRegistration, resolutionInfo);
                    return this.expression = Expression.Constant(finalizerExpression.CompileDelegate(resolutionInfo.CurrentScopeParameter)(resolutionInfo.ResolutionScope));
                }

                return this.expression = Expression.Constant(serviceRegistration.RegistrationContext.ExistingInstance);
            }
        }

        public override IObjectBuilder Produce() => new InstanceObjectBuilder();

        public override bool HandlesObjectLifecycle => true;
    }
}