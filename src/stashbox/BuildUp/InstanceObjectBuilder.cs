using Stashbox.Entity;
using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp
{
    internal class InstanceObjectBuilder : ObjectBuilderBase
    {
        private volatile Expression expression;
        private readonly object syncObject = new object();

        public InstanceObjectBuilder(IContainerContext containerContext)
            : base(containerContext)
        {
        }

        protected override Expression GetExpressionInternal(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;

                if (serviceRegistration.ShouldHandleDisposal && serviceRegistration.RegistrationContext.ExistingInstance is IDisposable disposable)
                    resolutionInfo.RootScope.AddDisposableTracking(disposable);

                return this.expression = Expression.Constant(serviceRegistration.RegistrationContext.ExistingInstance);
            }
        }

        public override bool HandlesObjectDisposal => true;
    }
}