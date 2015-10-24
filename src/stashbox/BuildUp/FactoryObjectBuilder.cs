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
        private readonly IObjectExtender objectExtender;

        public FactoryObjectBuilder(Func<object> factory, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            Shield.EnsureNotNull(factory);
            Shield.EnsureNotNull(containerExtensionManager);
            Shield.EnsureNotNull(objectExtender);
            this.containerExtensionManager = containerExtensionManager;
            this.singleFactory = factory;
            this.objectExtender = objectExtender;
        }

        public FactoryObjectBuilder(Func<object, object> oneParamsFactory, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            Shield.EnsureNotNull(oneParamsFactory);
            Shield.EnsureNotNull(containerExtensionManager);
            this.containerExtensionManager = containerExtensionManager;
            this.oneParamsFactory = oneParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object> twoParamsFactory, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            Shield.EnsureNotNull(twoParamsFactory);
            Shield.EnsureNotNull(containerExtensionManager);
            this.containerExtensionManager = containerExtensionManager;
            this.twoParamsFactory = twoParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object, object> threeParamsFactory, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            Shield.EnsureNotNull(threeParamsFactory);
            Shield.EnsureNotNull(containerExtensionManager);
            this.containerExtensionManager = containerExtensionManager;
            this.threeParamsFactory = threeParamsFactory;
        }

        public object BuildInstance(IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
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

            var builtInstance = this.objectExtender.ExtendObject(instance, containerContext, resolutionInfo);
            return this.containerExtensionManager.ExecutePostBuildExtensions(builtInstance, builtInstance?.GetType(), containerContext, resolutionInfo);
        }

        public void CleanUp()
        {
        }
    }
}