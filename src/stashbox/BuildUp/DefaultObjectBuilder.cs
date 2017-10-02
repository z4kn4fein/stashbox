using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
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

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (!containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependencyTrackingEnabled)
                return this.expressionBuilder.CreateExpression(containerContext, serviceRegistration, resolutionInfo, resolveType);

            using (new CircularDependencyBarrier(resolutionInfo, serviceRegistration))
                return this.expressionBuilder.CreateExpression(containerContext, serviceRegistration, resolutionInfo, resolveType);
        }
    }
}
