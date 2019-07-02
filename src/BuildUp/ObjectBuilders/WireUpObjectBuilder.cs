using Stashbox.BuildUp.Expressions;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.ObjectBuilders
{
    internal class WireUpObjectBuilder : ObjectBuilderBase
    {
        private readonly IExpressionBuilder expressionBuilder;

        public WireUpObjectBuilder(IExpressionBuilder expressionBuilder)
        {
            this.expressionBuilder = expressionBuilder;
        }

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType) =>
            this.expressionBuilder.CreateFillExpression(containerContext, serviceRegistration,
                serviceRegistration.RegistrationContext.ExistingInstance.AsConstant(),
                resolutionContext, serviceRegistration.ImplementationType);
    }
}