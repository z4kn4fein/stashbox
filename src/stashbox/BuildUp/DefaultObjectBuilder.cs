using System;
using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : ObjectBuilderBase
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IContainerContext containerContext;
        private readonly IExpressionBuilder expressionBuilder;

        public DefaultObjectBuilder(IContainerContext containerContext, IMetaInfoProvider metaInfoProvider,
            IContainerExtensionManager containerExtensionManager, IExpressionBuilder expressionBuilder,
            InjectionParameter[] injectionParameters, bool isDecorator, bool shouldHandleDisposal)
            : base(containerContext, isDecorator, shouldHandleDisposal)
        {
            this.injectionParameters = injectionParameters;
            this.containerExtensionManager = containerExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
            this.expressionBuilder = expressionBuilder;
        }

        protected override Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (!this.containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependencyTrackingEnabled)
                return this.CreateExpression(resolutionInfo, resolveType);

            using (new CircularDependencyBarrier(resolutionInfo, this.metaInfoProvider.TypeTo))
                return this.CreateExpression(resolutionInfo, resolveType);
        }

        private Expression CreateExpression(ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (!this.metaInfoProvider.TryChooseConstructor(out ResolutionConstructor constructor,
                resolutionInfo))
                return null;

            return this.expressionBuilder.CreateExpression(this.containerExtensionManager, this.containerContext,
                    constructor, resolutionInfo, resolveType, this.injectionParameters,
                    this.metaInfoProvider.GetResolutionMembers(resolutionInfo),
                    this.metaInfoProvider.GetResolutionMethods(resolutionInfo));
        }
    }
}
