using Ronin.Common;
using Sendstorm.Infrastructure;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox
{
    public class ObjectExtender : IObjectExtender, IMessageReceiver<RegistrationAdded>, IMessageReceiver<RegistrationRemoved>
    {
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IMessagePublisher messagePublisher;
        private readonly HashSet<InjectionParameter> injectionParameters;
        private readonly DisposableReaderWriterLock readerWriterLock;

        private HashSet<ResolutionMethod> injectionMethods;
        private HashSet<ResolutionProperty> injectionProperties;

        private bool hasInjectionMethods;
        private bool hasInjectionProperties;

        public ObjectExtender(IMetaInfoProvider metaInfoProvider,
            IMessagePublisher messagePublisher, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            Shield.EnsureNotNull(metaInfoProvider);
            Shield.EnsureNotNull(messagePublisher);

            this.readerWriterLock = new DisposableReaderWriterLock();

            if (injectionParameters != null)
                this.injectionParameters = new HashSet<InjectionParameter>(injectionParameters);

            this.metaInfoProvider = metaInfoProvider;
            this.messagePublisher = messagePublisher;

            this.messagePublisher.Subscribe<RegistrationAdded>(this, addedEvent => this.metaInfoProvider.SensitivityList.Contains(addedEvent.RegistrationInfo.TypeFrom));
            this.messagePublisher.Subscribe<RegistrationRemoved>(this, removedEvent => this.metaInfoProvider.SensitivityList.Contains(removedEvent.RegistrationInfo.TypeFrom));
            this.CollectInjectionMembers();
        }

        public object ExtendObject(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            using (this.readerWriterLock.AquireReadLock())
            {
                if (this.hasInjectionProperties)
                {
                    foreach (var property in this.injectionProperties)
                    {
                        var value = containerContext.ResolutionStrategy.EvaluateResolutionTarget(containerContext, property.ResolutionTarget, resolutionInfo);
                        property.PropertySetter(instance, value);
                    }
                }

                if (this.hasInjectionMethods)
                {
                    foreach (var method in this.injectionMethods)
                    {
                        var parameters = method.Parameters.Select(parameter =>
                        containerContext.ResolutionStrategy.EvaluateResolutionTarget(containerContext, parameter, resolutionInfo));
                        method.MethodDelegate(instance, parameters.ToArray());
                    }
                }

                return instance;
            }
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
            using (this.readerWriterLock.AquireWriteLock())
            {
                this.injectionMethods = new HashSet<ResolutionMethod>(this.metaInfoProvider.GetResolutionMethods(resolutionInfo, this.injectionParameters));
                this.injectionProperties = new HashSet<ResolutionProperty>(this.metaInfoProvider.GetResolutionProperties(resolutionInfo, this.injectionParameters));

                this.hasInjectionMethods = this.injectionMethods.Any();
                this.hasInjectionProperties = this.injectionProperties.Any();
            }
        }
    }
}
