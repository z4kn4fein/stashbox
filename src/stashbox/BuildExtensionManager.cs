using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Collections.Generic;

namespace Stashbox
{
    internal class BuildExtensionManager : IBuildExtensionManager
    {
        private readonly HashSet<BuildExtension> buildExtensions;
        private readonly DisposableReaderWriterLock readerWriterLock;

        public BuildExtensionManager()
        {
            this.buildExtensions = new HashSet<BuildExtension>();
            this.readerWriterLock = new DisposableReaderWriterLock();
        }

        public void AddExtension(BuildExtension buildExtension)
        {
            using (this.readerWriterLock.AquireWriteLock())
            {
                this.buildExtensions.Add(buildExtension);
            }
        }

        public object ExecutePostBuildExtensions(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            using (this.readerWriterLock.AquireReadLock())
            {
                object result = instance;
                foreach (var extension in this.buildExtensions)
                {
                    result = extension.PostBuild(instance, builderContext, resolutionInfo);
                }

                return result;
            }
        }

        public void ExecutePreBuildExtensions(IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            using (this.readerWriterLock.AquireReadLock())
            {
                foreach (var extension in this.buildExtensions)
                {
                    extension.PreBuild(builderContext, resolutionInfo);
                }
            }
        }

        public void ExecuteOnRegistrationExtensions(IBuilderContext builderContext, RegistrationInfo registrationInfo)
        {
            using (this.readerWriterLock.AquireReadLock())
            {
                foreach (var extension in this.buildExtensions)
                {
                    extension.OnRegistration(builderContext, registrationInfo);
                }
            }
        }
    }
}
