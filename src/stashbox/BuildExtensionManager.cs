using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace Stashbox
{
    internal class BuildExtensionManager : IContainerExtensionManager
    {
        private readonly Ref<ImmutableHashSet<IPostBuildExtension>> postbuildExtensions;
        private readonly Ref<ImmutableHashSet<IRegistrationExtension>> registrationExtensions;
        private readonly DisposableReaderWriterLock readerWriterLock;

        public BuildExtensionManager()
        {
            this.postbuildExtensions = new Ref<ImmutableHashSet<IPostBuildExtension>>(ImmutableHashSet<IPostBuildExtension>.Empty);
            this.registrationExtensions = new Ref<ImmutableHashSet<IRegistrationExtension>>(ImmutableHashSet<IRegistrationExtension>.Empty);
            this.readerWriterLock = new DisposableReaderWriterLock(LockRecursionPolicy.SupportsRecursion);
        }

        public void AddExtension(IContainerExtension containerExtension)
        {
            //using (this.readerWriterLock.AquireWriteLock())
            //{
                var postBuildExtension = containerExtension as IPostBuildExtension;
                if (postBuildExtension != null)
                {
                    var newRepo = this.postbuildExtensions.Value.Add(postBuildExtension);
                    if (!this.postbuildExtensions.TrySwapIfStillCurrent(this.postbuildExtensions.Value, newRepo))
                        this.postbuildExtensions.Swap(_ => newRepo);
                }

                var registrationExtension = containerExtension as IRegistrationExtension;
                if (registrationExtension != null)
                {
                    var newRepo = this.registrationExtensions.Value.Add(registrationExtension);
                    if (!this.registrationExtensions.TrySwapIfStillCurrent(this.registrationExtensions.Value, newRepo))
                        this.registrationExtensions.Swap(_ => newRepo);
                }
            //}
        }

        public object ExecutePostBuildExtensions(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            //using (this.readerWriterLock.AquireReadLock())
            //{
                var result = instance;
                foreach (var extension in this.postbuildExtensions.Value)
                {
                    result = extension.PostBuild(instance, targetType, containerContext, resolutionInfo, injectionParameters);
                }

                return result;
            //}
        }

        public void ExecuteOnRegistrationExtensions(IContainerContext containerContext, RegistrationInfo registrationInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            //using (this.readerWriterLock.AquireReadLock())
            //{
                foreach (var extension in this.registrationExtensions.Value)
                {
                    extension.OnRegistration(containerContext, registrationInfo, injectionParameters);
                }
            //}
        }
    }
}
