using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Stashbox.Utils;

namespace Stashbox
{
    internal class BuildExtensionManager : IContainerExtensionManager
    {
        private readonly HashSet<IPostBuildExtension> postbuildExtensions;
        private readonly HashSet<IRegistrationExtension> registrationExtensions;
        private readonly DisposableReaderWriterLock readerWriterLock;

        public bool HasPostBuildExtensions => this.hasPostBuildExtensions;

        private bool hasPostBuildExtensions;
        private bool hasRegistrationExtensions;

        public BuildExtensionManager()
        {
            this.postbuildExtensions = new HashSet<IPostBuildExtension>();
            this.registrationExtensions = new HashSet<IRegistrationExtension>();
            this.readerWriterLock = new DisposableReaderWriterLock(LockRecursionPolicy.SupportsRecursion);
        }

        public void AddExtension(IContainerExtension containerExtension)
        {
            using (this.readerWriterLock.AcquireWriteLock())
            {
                var postBuildExtension = containerExtension as IPostBuildExtension;
                if (postBuildExtension != null)
                {
                    this.postbuildExtensions.Add(postBuildExtension);
                    this.hasPostBuildExtensions = true;
                }

                var registrationExtension = containerExtension as IRegistrationExtension;
                if (registrationExtension == null) return;
                this.registrationExtensions.Add(registrationExtension);
                this.hasRegistrationExtensions = true;
            }
        }

        public object ExecutePostBuildExtensions(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo, 
            TypeInformation resolveType, InjectionParameter[] injectionParameters = null)
        {
            if (!this.hasPostBuildExtensions) return instance;
            using (this.readerWriterLock.AcquireReadLock())
            {
                var result = instance;
                foreach (var extension in this.postbuildExtensions)
                    result = extension.PostBuild(instance, targetType, containerContext, resolutionInfo, resolveType, injectionParameters);

                return result;
            }
        }

        public void ExecuteOnRegistrationExtensions(IContainerContext containerContext, RegistrationInfo registrationInfo, InjectionParameter[] injectionParameters = null)
        {
            if (!this.hasRegistrationExtensions) return;
            using (this.readerWriterLock.AcquireReadLock())
            {
                foreach (var extension in this.registrationExtensions)
                    extension.OnRegistration(containerContext, registrationInfo, injectionParameters);
            }
        }

        public IContainerExtensionManager CreateCopy()
        {
            var extensionManager = new BuildExtensionManager();
            using (this.readerWriterLock.AcquireReadLock())
                foreach (var extension in this.postbuildExtensions.OfType<IContainerExtension>().Concat(this.registrationExtensions))
                    extensionManager.AddExtension(extension.CreateCopy());

            return extensionManager;
        }

        public void ReinitalizeExtensions(IContainerContext containerContext)
        {
            using (this.readerWriterLock.AcquireReadLock())
                foreach (var extension in this.postbuildExtensions.OfType<IContainerExtension>().Concat(this.registrationExtensions))
                    extension.Initialize(containerContext);
        }


        public void CleanUp()
        {
            using (this.readerWriterLock.AcquireWriteLock())
                foreach (var extension in this.postbuildExtensions.OfType<IContainerExtension>().Concat(this.registrationExtensions))
                    extension.CleanUp();
        }
    }
}
