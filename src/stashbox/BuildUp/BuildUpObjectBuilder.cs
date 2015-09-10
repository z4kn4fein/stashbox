using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;

namespace Stashbox.BuildUp
{
    internal class BuildUpObjectBuilder : IObjectBuilder
    {
        private readonly object instance;
        private volatile object builtInstance;
        private readonly object syncObject = new object();
        private readonly IContainerExtensionManager containerExtensionManager;

        public BuildUpObjectBuilder(object instance, IContainerExtensionManager containerExtensionManager)
        {
            Shield.EnsureNotNull(instance);
            Shield.EnsureNotNull(containerExtensionManager);

            this.instance = instance;
            this.containerExtensionManager = containerExtensionManager;
        }

        public object BuildInstance(IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            Shield.EnsureNotNull(builderContext);
            Shield.EnsureNotNull(resolutionInfo);

            if (this.builtInstance != null) return this.builtInstance;
            lock (this.syncObject)
            {
                if (this.builtInstance != null) return this.builtInstance;
                this.containerExtensionManager.ExecutePreBuildExtensions(builderContext, resolutionInfo);
                this.builtInstance = this.containerExtensionManager.ExecutePostBuildExtensions(this.instance, builderContext, resolutionInfo);
            }

            return this.builtInstance;
        }

        public void CleanUp()
        {
            if (this.builtInstance == null) return;
            lock (this.syncObject)
            {
                if (this.builtInstance == null) return;
                var disposable = this.builtInstance as IDisposable;
                disposable?.Dispose();
                this.builtInstance = null;
            }
        }
    }
}