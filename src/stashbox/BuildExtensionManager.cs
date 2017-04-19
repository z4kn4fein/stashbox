using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System.Linq;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    internal class BuildExtensionManager : IContainerExtensionManager
    {
        private readonly ConcurrentOrderedStore<IContainerExtension> repository;

        public bool HasPostBuildExtensions { get; private set; }

        public bool HasRegistrationExtensions { get; private set; }

        public BuildExtensionManager()
        {
            this.repository = new ConcurrentOrderedStore<IContainerExtension>();
        }

        public void AddExtension(IContainerExtension containerExtension)
        {
            if (containerExtension is IPostBuildExtension)
                this.HasPostBuildExtensions = true;
            else
                this.HasRegistrationExtensions = true;

            this.repository.Add(containerExtension);
        }

        public object ExecutePostBuildExtensions(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            Type resolveType, InjectionParameter[] injectionParameters = null) =>
            this.repository.OfType<IPostBuildExtension>().Aggregate(instance, (current, extension) => extension.PostBuild(current, containerContext, resolutionInfo, resolveType, injectionParameters));

        public void ExecuteOnRegistrationExtensions(IContainerContext containerContext, Type typeTo, Type typeFrom, InjectionParameter[] injectionParameters = null)
        {
            if (!this.HasRegistrationExtensions) return;

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
