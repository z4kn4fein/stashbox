using Ronin.Common;
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
        private readonly DisposableReaderWriterLock readerWriterLock;

        public BuildExtensionManager()
        {
            this.postbuildExtensions = new HashSet<IPostBuildExtension>();
            this.registrationExtensions = new HashSet<IRegistrationExtension>();
            this.readerWriterLock = new DisposableReaderWriterLock(LockRecursionPolicy.SupportsRecursion);
        }

        public void AddExtension(IContainerExtension containerExtension)
        {
            using (this.readerWriterLock.AquireWriteLock())
            {
                var postBuildExtension = containerExtension as IPostBuildExtension;
                if (postBuildExtension != null)
                    this.postbuildExtensions.Add(postBuildExtension);

                var registrationExtension = containerExtension as IRegistrationExtension;
                if (registrationExtension != null)
                    this.registrationExtensions.Add(registrationExtension);
            }
        }

        public object ExecutePostBuildExtensions(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            using (this.readerWriterLock.AquireReadLock())
            {
                var result = instance;
                foreach (var extension in this.postbuildExtensions)
                {
                    result = extension.PostBuild(instance, targetType, containerContext, resolutionInfo, injectionParameters);
                }

                return result;
            }
        }

        public void ExecuteOnRegistrationExtensions(IContainerContext containerContext, RegistrationInfo registrationInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            using (this.readerWriterLock.AquireReadLock())
            {
                foreach (var extension in this.registrationExtensions)
                {
                    extension.OnRegistration(containerContext, registrationInfo, injectionParameters);
                }
            }
        }
    }
}
