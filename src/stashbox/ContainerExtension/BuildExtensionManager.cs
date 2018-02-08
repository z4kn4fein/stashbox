using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq;

namespace Stashbox.ContainerExtension
{
    internal class BuildExtensionManager : IContainerExtensionManager
    {
        private ArrayStore<IContainerExtension> repository;

        public bool HasPostBuildExtensions { get; private set; }

        public bool HasRegistrationExtensions { get; private set; }

        public BuildExtensionManager()
        {
            this.repository = ArrayStore<IContainerExtension>.Empty;
        }

        public void AddExtension(IContainerExtension containerExtension)
        {
            if (containerExtension is IPostBuildExtension)
                this.HasPostBuildExtensions = true;

            if (containerExtension is IRegistrationExtension)
                this.HasRegistrationExtensions = true;

            Swap.SwapValue(ref this.repository, repo => repo.Add(containerExtension));
        }

        public object ExecutePostBuildExtensions(object instance, IContainerContext containerContext, ResolutionContext resolutionContext,
            IServiceRegistration serviceRegistration, Type requestedType) =>
            this.repository.OfType<IPostBuildExtension>().Aggregate(instance, (current, extension) => extension.PostBuild(current, containerContext, resolutionContext, serviceRegistration, requestedType));

        public void ExecuteOnRegistrationExtensions(IContainerContext containerContext, IServiceRegistration serviceRegistration)
        {
            if (!this.HasRegistrationExtensions) return;

            foreach (var extension in this.repository.OfType<IRegistrationExtension>())
                extension.OnRegistration(containerContext, serviceRegistration);
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
