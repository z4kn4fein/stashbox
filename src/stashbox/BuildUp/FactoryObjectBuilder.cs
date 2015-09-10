using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq;

namespace Stashbox.BuildUp
{
    internal class FactoryObjectBuilder : IObjectBuilder
    {
        private readonly Func<object> singleFactory;
        private readonly Func<object, object> oneParamsFactory;
        private readonly Func<object, object, object> twoParamsFactory;
        private readonly Func<object, object, object, object> threeParamsFactory;
        private readonly IContainerExtensionManager containerExtensionManager;

        public FactoryObjectBuilder(Func<object> factory, IContainerExtensionManager containerExtensionManager)
        {
            Shield.EnsureNotNull(factory);
            Shield.EnsureNotNull(containerExtensionManager);
            this.containerExtensionManager = containerExtensionManager;
            this.singleFactory = factory;
        }

        public FactoryObjectBuilder(Func<object, object> oneParamsFactory, IContainerExtensionManager containerExtensionManager)
        {
            Shield.EnsureNotNull(oneParamsFactory);
            Shield.EnsureNotNull(containerExtensionManager);
            this.containerExtensionManager = containerExtensionManager;
            this.oneParamsFactory = oneParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object> twoParamsFactory, IContainerExtensionManager containerExtensionManager)
        {
            Shield.EnsureNotNull(twoParamsFactory);
            Shield.EnsureNotNull(containerExtensionManager);
            this.containerExtensionManager = containerExtensionManager;
            this.twoParamsFactory = twoParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object, object> threeParamsFactory, IContainerExtensionManager containerExtensionManager)
        {
            Shield.EnsureNotNull(threeParamsFactory);
            Shield.EnsureNotNull(containerExtensionManager);
            this.containerExtensionManager = containerExtensionManager;
            this.threeParamsFactory = threeParamsFactory;
        }

        public object BuildInstance(IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            this.containerExtensionManager.ExecutePreBuildExtensions(builderContext, resolutionInfo);

            object instance = null;

            if (this.singleFactory != null)
                instance = this.singleFactory.Invoke();

            if (this.oneParamsFactory != null)
                instance = this.oneParamsFactory.Invoke(resolutionInfo.FactoryParams);

            if (this.twoParamsFactory != null)
                instance = this.twoParamsFactory.Invoke(resolutionInfo.FactoryParams.ElementAt(0), resolutionInfo.FactoryParams.ElementAt(1));

            if (this.threeParamsFactory != null)
                instance = this.threeParamsFactory.Invoke(
                    resolutionInfo.FactoryParams.ElementAt(0),
                    resolutionInfo.FactoryParams.ElementAt(1),
                    resolutionInfo.FactoryParams.ElementAt(2));

            return this.containerExtensionManager.ExecutePostBuildExtensions(instance, builderContext, resolutionInfo);
        }

        public void CleanUp()
        {
        }
    }
}