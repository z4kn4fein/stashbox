using System;
using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using System.Linq.Expressions;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : ObjectBuilderBase
    {
        private readonly IContainerContext containerContext;
        private readonly IExpressionBuilder expressionBuilder;

        public DefaultObjectBuilder(IContainerContext containerContext, IExpressionBuilder expressionBuilder)
            : base(containerContext)
        {
            this.containerContext = containerContext;
            this.expressionBuilder = expressionBuilder;
        }

        protected override Expression GetExpressionInternal(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (!this.containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependencyTrackingEnabled)
                return this.CreateExpression(serviceRegistration, resolutionInfo, resolveType);

            using (new CircularDependencyBarrier(resolutionInfo, serviceRegistration.ImplementationType))
                return this.CreateExpression(serviceRegistration, resolutionInfo, resolveType);
        }

        private Expression CreateExpression(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (!serviceRegistration.MetaInfoProvider.TryChooseConstructor(out ResolutionConstructor constructor, resolutionInfo))
                return null;

            return this.expressionBuilder.CreateExpression(serviceRegistration, constructor, resolutionInfo, resolveType);
        }
    }
}
