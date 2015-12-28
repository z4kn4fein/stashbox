using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using System.Linq;

namespace Stashbox
{
    public class ObjectExtender : IObjectExtender
    {
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly InjectionParameter[] injectionParameters;
        private volatile ResolutionMethod[] injectionMethods;
        private volatile ResolutionMember[] injectionMembers;
        private readonly object resolutionMemberSyncObject = new object();
        private readonly object resolutionMethodSyncObject = new object();
        private bool isMembersDirty;
        private bool isMethodDirty;

        public ObjectExtender(IMetaInfoProvider metaInfoProvider, InjectionParameter[] injectionParameters = null)
        {
            if (injectionParameters != null)
                this.injectionParameters = injectionParameters;

            this.metaInfoProvider = metaInfoProvider;
        }

        public object FillResolutionMembers(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            if (!this.metaInfoProvider.HasInjectionMembers) return instance;

            var members = this.GetResolutionMembers();

            var count = members.Length;
            for (var i = 0; i < count; i++)
            {
                var value = containerContext.ResolutionStrategy.EvaluateResolutionTarget(members[i].ResolutionTarget, resolutionInfo);
                members[i].MemberSetter(instance, value);
            }

            return instance;
        }

        public object FillResolutionMethods(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            if (!this.metaInfoProvider.HasInjectionMethod) return instance;
            {
                var methods = this.GetResolutionMethods();

                var count = methods.Length;
                for (var i = 0; i < count; i++)
                {
                    methods[i].MethodDelegate(resolutionInfo, instance);
                }
            }

            return instance;
        }

        public ResolutionMember[] GetResolutionMembers()
        {
            if (!this.metaInfoProvider.HasInjectionMembers) return null;

            if (this.injectionMembers != null && !this.isMembersDirty) return this.injectionMembers;
            lock (this.resolutionMemberSyncObject)
            {
                if (this.injectionMembers != null && !this.isMembersDirty) return this.injectionMembers;
                return this.injectionMembers = this.metaInfoProvider.GetResolutionMembers(this.injectionParameters).ToArray();
            }
        }

        public void ServiceUpdated(RegistrationInfo registrationInfo)
        {
            if (!this.metaInfoProvider.SensitivityList.Contains(registrationInfo.TypeFrom)) return;
            this.isMembersDirty = true;
            this.isMethodDirty = true;
        }

        public void CleanUp()
        {
        }

        private ResolutionMethod[] GetResolutionMethods()
        {
            if (!this.metaInfoProvider.HasInjectionMethod) return null;

            if (this.injectionMethods != null && !this.isMethodDirty) return this.injectionMethods;
            lock (this.resolutionMethodSyncObject)
            {
                if (this.injectionMethods != null && !this.isMethodDirty) return this.injectionMethods;
                return this.injectionMethods = this.metaInfoProvider.GetResolutionMethods(this.injectionParameters).ToArray();
            }
        }

        public void Receive(ServiceUpdated message)
        {
            this.isMembersDirty = true;
            this.isMethodDirty = true;
        }
    }
}
