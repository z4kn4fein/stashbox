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
        private readonly IContainerContext containerContext;

        private FactoryObjectBuilder(IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            Shield.EnsureNotNull(containerExtensionManager);
            Shield.EnsureNotNull(objectExtender);
            Shield.EnsureNotNull(containerContext);
            this.containerExtensionManager = containerExtensionManager;
            this.objectExtender = objectExtender;
            this.containerContext = containerContext;
        }

        public FactoryObjectBuilder(Func<object> factory, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            :this(containerContext, containerExtensionManager, objectExtender)
        {
            this.singleFactory = factory;
        }

        public FactoryObjectBuilder(Func<object, object> oneParamsFactory, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            : this(containerContext, containerExtensionManager, objectExtender)
        {
            this.oneParamsFactory = oneParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object> twoParamsFactory, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            : this(containerContext, containerExtensionManager, objectExtender)
        {
            this.twoParamsFactory = twoParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object, object> threeParamsFactory, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
            : this(containerContext, containerExtensionManager, objectExtender)
        {
            this.threeParamsFactory = threeParamsFactory;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo)
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
            this.objectExtender.CleanUp();
        }
    }
}