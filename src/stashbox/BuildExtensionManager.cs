using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Stashbox
{
    internal class BuildExtensionManager : IContainerExtensionManager
    {
        private readonly HashSet<IPostBuildExtension> postbuildExtensions;
        private readonly HashSet<IRegistrationExtension> registrationExtensions;
        private readonly ReaderWriterLockSlim readerWriterLock;

        private bool hasPostBuildExtensions;
        private bool hasRegistrationExtensions;

        public BuildExtensionManager()
        {
            this.postbuildExtensions = new HashSet<IPostBuildExtension>();
            this.registrationExtensions = new HashSet<IRegistrationExtension>();
            this.readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public void AddExtension(IContainerExtension containerExtension)
        {
            try
            {
                this.readerWriterLock.EnterWriteLock();
                var postBuildExtension = containerExtension as IPostBuildExtension;
                if (postBuildExtension != null)
                {
                    this.postbuildExtensions.Add(postBuildExtension);
                    this.hasPostBuildExtensions = true;
                }

                var registrationExtension = containerExtension as IRegistrationExtension;
                if (registrationExtension != null)
                {
                    this.registrationExtensions.Add(registrationExtension);
                    this.hasRegistrationExtensions = true;
                }
            }
            finally
            {
                this.readerWriterLock.ExitWriteLock();
            }
        }

        public object ExecutePostBuildExtensions(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            if (!this.hasPostBuildExtensions) return instance;
            try
            {
                this.readerWriterLock.EnterReadLock();
                var result = instance;
                foreach (var extension in this.postbuildExtensions)
                {
                    result = extension.PostBuild(instance, targetType, containerContext, resolutionInfo, injectionParameters);
                }

                return result;
            }
            finally
            {
                this.readerWriterLock.ExitReadLock();
            }
        }

        public void ExecuteOnRegistrationExtensions(IContainerContext containerContext, RegistrationInfo registrationInfo, InjectionParameter[] injectionParameters = null)
        {
            if (!this.hasRegistrationExtensions) return;
            try
            {
                this.readerWriterLock.EnterReadLock();
                foreach (var extension in this.registrationExtensions)
                {
                    extension.OnRegistration(containerContext, registrationInfo, injectionParameters);
                }
            }
            finally
            {
                this.readerWriterLock.ExitReadLock();
            }
        }
    }
}
