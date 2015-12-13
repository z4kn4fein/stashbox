using Ronin.Common;
using Sendstorm.Infrastructure;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Entity.Resolution;
using Stashbox.Extensions;
using Stashbox.Infrastructure;
using System.Linq;

namespace Stashbox
{
    public class ObjectExtender : IObjectExtender, IMessageReceiver<RegistrationAdded>, IMessageReceiver<RegistrationRemoved>
    {
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IMessagePublisher messagePublisher;
        private readonly InjectionParameter[] injectionParameters;

        private ResolutionMethod[] injectionMethods;
        private ResolutionProperty[] injectionProperties;

        private bool hasInjectionMethods;
        private bool hasInjectionProperties;

        public ObjectExtender(IMetaInfoProvider metaInfoProvider,
            IMessagePublisher messagePublisher, InjectionParameter[] injectionParameters = null)
        {
            Shield.EnsureNotNull(() => metaInfoProvider);
            Shield.EnsureNotNull(() => messagePublisher);

            if (injectionParameters != null)
                this.injectionParameters = injectionParameters;

            this.metaInfoProvider = metaInfoProvider;
            this.messagePublisher = messagePublisher;

            this.messagePublisher.Subscribe<RegistrationAdded>(this, addedEvent => this.metaInfoProvider.SensitivityList.Contains(addedEvent.RegistrationInfo.TypeFrom));
            this.messagePublisher.Subscribe<RegistrationRemoved>(this, removedEvent => this.metaInfoProvider.SensitivityList.Contains(removedEvent.RegistrationInfo.TypeFrom));
            this.CollectInjectionMembers();
        }

        public object ExtendObject(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            if (this.hasInjectionProperties)
            {
                var properties = this.injectionProperties.CreateCopy();
                var count = properties.Count;
                for (var i = 0; i < count; i++)
                {
                    var value = containerContext.ResolutionStrategy.EvaluateResolutionTarget(containerContext, properties[i].ResolutionTarget, resolutionInfo);
                    properties[i].PropertySetter(instance, value);
                }
            }

            if (!this.hasInjectionMethods) return instance;
            {
                var methods = this.injectionMethods.CreateCopy();
                var count = methods.Count;
                for (var i = 0; i < count; i++)
                {
                    methods[i].MethodDelegate(resolutionInfo, instance);
                }
            }

            return instance;
        }

        public void Receive(RegistrationRemoved message)
        {
            this.CollectInjectionMembers();
        }

        public void Receive(RegistrationAdded message)
        {
            this.CollectInjectionMembers();
        }

        private void CollectInjectionMembers()
        {
            this.injectionMethods = this.metaInfoProvider.GetResolutionMethods(this.injectionParameters).ToArray();
            this.injectionProperties = this.metaInfoProvider.GetResolutionProperties(this.injectionParameters).ToArray();

            this.hasInjectionMethods = this.injectionMethods.Length > 0;
            this.hasInjectionProperties = this.injectionProperties.Length > 0;
        }

        public void CleanUp()
        {
            this.messagePublisher.UnSubscribe<RegistrationAdded>(this);
            this.messagePublisher.UnSubscribe<RegistrationRemoved>(this);
        }
    }
}
