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
        private readonly object syncObject = new object();

        private Ref<ResolutionMethod[]> injectionMethods;
        private Ref<ResolutionProperty[]> injectionProperties;

        private bool hasInjectionMethods;
        private bool hasInjectionProperties;

        public ObjectExtender(IMetaInfoProvider metaInfoProvider,
            IMessagePublisher messagePublisher, InjectionParameter[] injectionParameters = null)
        {
            Shield.EnsureNotNull(metaInfoProvider);
            Shield.EnsureNotNull(messagePublisher);

            this.injectionMethods = new Ref<ResolutionMethod[]>();
            this.injectionProperties = new Ref<ResolutionProperty[]>();

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
                var properties = this.injectionProperties.Value.CreateCopy();
                for (int i = 0; i < properties.Count; i++)
                {
                    var value = containerContext.ResolutionStrategy.EvaluateResolutionTarget(containerContext, properties[i].ResolutionTarget, resolutionInfo);
                    properties[i].PropertySetter(instance, value);
                }
            }

            if (this.hasInjectionMethods)
            {
                var methods = this.injectionMethods.Value.CreateCopy();
                for (int i = 0; i < methods.Count; i++)
                {
                    var parameters = methods[i].Parameters.Select(parameter =>
                    containerContext.ResolutionStrategy.EvaluateResolutionTarget(containerContext, parameter, resolutionInfo));
                    methods[i].MethodDelegate(instance, parameters.ToArray());
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

        private void CollectInjectionMembers(ResolutionInfo resolutionInfo = null)
        {
            lock (this.syncObject)
            {
                var injectionMethods = this.metaInfoProvider.GetResolutionMethods(resolutionInfo, this.injectionParameters).ToArray();
                if (!this.injectionMethods.TrySwapIfStillCurrent(this.injectionMethods.Value, injectionMethods))
                    this.injectionMethods.Swap(_ => injectionMethods);

                var injectionProperties = this.metaInfoProvider.GetResolutionProperties(resolutionInfo, this.injectionParameters).ToArray();
                if (!this.injectionProperties.TrySwapIfStillCurrent(this.injectionProperties.Value, injectionProperties))
                    this.injectionProperties.Swap(_ => injectionProperties);

                this.hasInjectionMethods = injectionMethods.Length > 0;
                this.hasInjectionProperties = injectionProperties.Length > 0;
            }
        }

        public void CleanUp()
        {
            this.messagePublisher.UnSubscribe<RegistrationAdded>(this);
            this.messagePublisher.UnSubscribe<RegistrationRemoved>(this);
        }
    }
}
