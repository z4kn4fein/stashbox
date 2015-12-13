using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq.Expressions;

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
        private readonly IContainerContext containerContext;

        public BuildUpObjectBuilder(object instance, IContainerContext containerContext, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender)
        {
            Shield.EnsureNotNull(() => containerContext);
            Shield.EnsureNotNull(() => instance);
            Shield.EnsureNotNull(() => containerExtensionManager);
            Shield.EnsureNotNull(() => objectExtender);

            this.instance = instance;
            this.instanceType = instance.GetType();
            this.containerExtensionManager = containerExtensionManager;
            this.objectExtender = objectExtender;
            this.containerContext = containerContext;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo)
        {
            Shield.EnsureNotNull(() => resolutionInfo);

            if (this.builtInstance != null) return this.builtInstance;
            lock (this.syncObject)
            {
                if (this.builtInstance != null) return this.builtInstance;
                this.builtInstance = this.objectExtender.ExtendObject(this.instance, this.containerContext, resolutionInfo);
                this.builtInstance = this.containerExtensionManager.ExecutePostBuildExtensions(this.builtInstance, this.instanceType, this.containerContext, resolutionInfo);
            }

            return this.builtInstance;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            return Expression.Constant(this.BuildInstance(resolutionInfo));
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

            this.objectExtender.CleanUp();
        }
    }
}