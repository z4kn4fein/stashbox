using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class InstanceObjectBuilder : ObjectBuilderBase
    {
        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType) =>
            serviceRegistration.RegistrationContext.ExistingInstance.AsConstant();
    }
}