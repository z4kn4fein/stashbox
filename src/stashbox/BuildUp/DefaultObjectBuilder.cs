using Stashbox.BuildUp.Expressions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : ObjectBuilderBase
    {
        private readonly IExpressionBuilder expressionBuilder;

        public DefaultObjectBuilder(IExpressionBuilder expressionBuilder)
        {
            this.expressionBuilder = expressionBuilder;
        }

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (!containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependencyTrackingEnabled)
                return this.expressionBuilder.CreateExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

            using (new CircularDependencyBarrier(resolutionContext, serviceRegistration))
                return this.expressionBuilder.CreateExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
        }
    }
}
