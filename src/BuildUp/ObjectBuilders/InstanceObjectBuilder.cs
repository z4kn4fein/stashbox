using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.ObjectBuilders
{
    internal class InstanceObjectBuilder : ObjectBuilderBase
    {
        public override bool ProducesLifetimeManageableOutput => false;

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType) =>
            serviceRegistration.RegistrationContext.ExistingInstance.AsConstant();
    }
}