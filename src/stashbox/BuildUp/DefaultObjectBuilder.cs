using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : IObjectBuilder
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IContainerContext containerContext;

        public DefaultObjectBuilder(IContainerContext containerContext, IMetaInfoProvider metaInfoProvider,
            IContainerExtensionManager containerExtensionManager, InjectionParameter[] injectionParameters = null)
        {
            if (injectionParameters != null)
                this.injectionParameters = injectionParameters;

            this.containerExtensionManager = containerExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (!this.containerContext.Container.ContainerConfiguration.CircularDependencyTrackingEnabled)
                return this.GetExpressionInternal(resolutionInfo, resolveType);

            using (new CircularDependencyBarrier(resolutionInfo.CircularDependencyBarrier, this.metaInfoProvider.TypeTo))
                return this.GetExpressionInternal(resolutionInfo, resolveType);
        }

        public bool HandlesObjectDisposal => false;

        private Expression GetExpressionInternal(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            ResolutionConstructor constructor;
            if (!this.metaInfoProvider.TryChooseConstructor(out constructor, resolutionInfo, this.injectionParameters))
                throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
            return this.CreateExpression(constructor, resolutionInfo, resolveType);
        }

        private Expression CreateExpression(ResolutionConstructor constructor, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return ExpressionDelegateFactory.CreateExpression(this.containerExtensionManager, this.containerContext,
                    constructor, resolutionInfo, resolveType, this.injectionParameters,
                    this.metaInfoProvider.GetResolutionMembers(resolutionInfo, this.injectionParameters),
                    this.metaInfoProvider.GetResolutionMethods(resolutionInfo, this.injectionParameters));
        }

        public void CleanUp()
        {
        }
    }
}
