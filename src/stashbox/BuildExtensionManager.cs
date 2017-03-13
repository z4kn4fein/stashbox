using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System.Linq;
using System.Threading;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    internal class BuildExtensionManager : IContainerExtensionManager
    {
        private readonly ConcurrentTree<IContainerExtension> repository;
        private int extensionCounter;

        public bool HasPostBuildExtensions { get; private set; }

        public BuildExtensionManager()
        {
            this.repository = new ConcurrentTree<IContainerExtension>();
        }

        public void AddExtension(IContainerExtension containerExtension)
        {
            if (containerExtension is IPostBuildExtension)
                this.HasPostBuildExtensions = true;

            this.repository.AddOrUpdate(Interlocked.Increment(ref extensionCounter), containerExtension);
        }

        public object ExecutePostBuildExtensions(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            Type resolveType, InjectionParameter[] injectionParameters = null) =>
            this.repository.OfType<IPostBuildExtension>().Aggregate(instance, (current, extension) => extension.PostBuild(current, containerContext, resolutionInfo, resolveType, injectionParameters));

        public void ExecuteOnRegistrationExtensions(IContainerContext containerContext, Type typeTo, Type typeFrom, InjectionParameter[] injectionParameters = null)
        {
            foreach (var extension in this.repository.OfType<IRegistrationExtension>())
                extension.OnRegistration(containerContext, typeTo, typeFrom, injectionParameters);
        }

        public IContainerExtensionManager CreateCopy()
        {
            var extensionManager = new BuildExtensionManager();
            foreach (var extension in this.repository)
                extensionManager.AddExtension(extension.CreateCopy());

            return extensionManager;
        }

        public void ReinitalizeExtensions(IContainerContext containerContext)
        {
            foreach (var extension in this.repository)
                extension.Initialize(containerContext);
        }

        public void CleanUp()
        {
            foreach (var extension in this.repository)
                extension.CleanUp();
        }
    }
}
