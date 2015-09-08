using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
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
        private readonly Func<object, object, object, object, object> fourParamsFactory;
        private readonly IBuildExtensionManager buildExtensionManager;

        public FactoryObjectBuilder(Func<object> factory, IBuildExtensionManager buildExtensionManager)
        {
            Shield.EnsureNotNull(factory);
            Shield.EnsureNotNull(buildExtensionManager);
            this.buildExtensionManager = buildExtensionManager;
            this.singleFactory = factory;
        }

        public FactoryObjectBuilder(Func<object, object> oneParamsFactory, IBuildExtensionManager buildExtensionManager)
        {
            Shield.EnsureNotNull(oneParamsFactory);
            Shield.EnsureNotNull(buildExtensionManager);
            this.buildExtensionManager = buildExtensionManager;
            this.oneParamsFactory = oneParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object> twoParamsFactory, IBuildExtensionManager buildExtensionManager)
        {
            Shield.EnsureNotNull(twoParamsFactory);
            Shield.EnsureNotNull(buildExtensionManager);
            this.buildExtensionManager = buildExtensionManager;
            this.twoParamsFactory = twoParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object, object> threeParamsFactory, IBuildExtensionManager buildExtensionManager)
        {
            Shield.EnsureNotNull(threeParamsFactory);
            Shield.EnsureNotNull(buildExtensionManager);
            this.buildExtensionManager = buildExtensionManager;
            this.threeParamsFactory = threeParamsFactory;
        }

        public FactoryObjectBuilder(Func<object, object, object, object, object> fourParamsFactory, IBuildExtensionManager buildExtensionManager)
        {
            Shield.EnsureNotNull(fourParamsFactory);
            Shield.EnsureNotNull(buildExtensionManager);
            this.buildExtensionManager = buildExtensionManager;
            this.fourParamsFactory = fourParamsFactory;
        }

        public object BuildInstance(IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            this.buildExtensionManager.ExecutePreBuildExtensions(builderContext, resolutionInfo);

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

            if (this.fourParamsFactory != null)
                instance = this.fourParamsFactory.Invoke(
                    resolutionInfo.FactoryParams.ElementAt(0),
                    resolutionInfo.FactoryParams.ElementAt(1),
                    resolutionInfo.FactoryParams.ElementAt(2),
                    resolutionInfo.FactoryParams.ElementAt(3));

            return this.buildExtensionManager.ExecutePostBuildExtensions(instance, builderContext, resolutionInfo);
        }

        public void CleanUp()
        {
        }
    }
}