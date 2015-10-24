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
        private readonly Type instanceType;
        private volatile object builtInstance;
        private readonly object syncObject = new object();
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IObjectExtender objectExtender;

        public BuildUpObjectBuilder(object instance, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            Shield.EnsureNotNull(instance);
            Shield.EnsureNotNull(containerExtensionManager);
            Shield.EnsureNotNull(objectExtender);

            this.instance = instance;
            this.instanceType = instance.GetType();
            this.containerExtensionManager = containerExtensionManager;
            this.objectExtender = objectExtender;
        }

        public object BuildInstance(IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            Shield.EnsureNotNull(containerContext);
            Shield.EnsureNotNull(resolutionInfo);

            if (this.builtInstance != null) return this.builtInstance;
            lock (this.syncObject)
            {
                if (this.builtInstance != null) return this.builtInstance;
                this.builtInstance = this.objectExtender.ExtendObject(this.instance, containerContext, resolutionInfo);
                this.builtInstance = this.containerExtensionManager.ExecutePostBuildExtensions(this.builtInstance, this.instanceType, containerContext, resolutionInfo);
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