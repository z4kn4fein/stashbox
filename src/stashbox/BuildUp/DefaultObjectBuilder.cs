using Ronin.Common;
using Sendstorm.Infrastructure;
using Stashbox.BuildUp.DelegateFactory;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Entity.Resolution;
using Stashbox.Exceptions;
using Stashbox.Extensions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : IObjectBuilder, IMessageReceiver<RegistrationAdded>, IMessageReceiver<RegistrationRemoved>
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IMessagePublisher messagePublisher;
        private readonly object syncObject = new object();
        private volatile CreateInstance constructorDelegate;
        private ResolutionProperty[] resolutionProperties;
        private ResolutionMethod[] resolutionMethods;
        private bool hasInjectionMethods;
        private readonly Type instanceType;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IContainerContext containerContext;

        public DefaultObjectBuilder(IContainerContext containerContext, IMetaInfoProvider metaInfoProvider, IContainerExtensionManager containerExtensionManager,
            IMessagePublisher messagePublisher, InjectionParameter[] injectionParameters = null)
        {
            Shield.EnsureNotNull(metaInfoProvider);
            Shield.EnsureNotNull(containerContext);
            Shield.EnsureNotNull(containerExtensionManager);
            Shield.EnsureNotNull(messagePublisher);

            if (injectionParameters != null)
                this.injectionParameters = injectionParameters;

            this.instanceType = metaInfoProvider.TypeTo;
            this.containerExtensionManager = containerExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
            this.messagePublisher = messagePublisher;
            this.CollectInjectionMembers();
            this.CreateConstructorDelegate();
            this.messagePublisher.Subscribe<RegistrationAdded>(this, addedEvent => this.metaInfoProvider.SensitivityList.Contains(addedEvent.RegistrationInfo.TypeFrom));
            this.messagePublisher.Subscribe<RegistrationRemoved>(this, removedEvent => this.metaInfoProvider.SensitivityList.Contains(removedEvent.RegistrationInfo.TypeFrom));
        }

        private void CreateConstructorDelegate()
        {
            ResolutionConstructor constructor;
            if (this.metaInfoProvider.TryChooseConstructor(out constructor, injectionParameters: this.injectionParameters))
            {
                this.constructorDelegate = ExpressionDelegateFactory.CreateConstructorExpression(this.containerContext, constructor, this.resolutionProperties);
            }
        }

        public object BuildInstance(ResolutionInfo resolutionInfo)
        {
            Shield.EnsureNotNull(containerContext);
            Shield.EnsureNotNull(resolutionInfo);

            if (this.constructorDelegate == null)
            {
                ResolutionConstructor constructor;
                if (this.metaInfoProvider.TryChooseConstructor(out constructor, resolutionInfo, this.injectionParameters))
                {
                    this.constructorDelegate = ExpressionDelegateFactory.CreateConstructorExpression(this.containerContext, constructor, this.resolutionProperties);

                    return this.ResolveType(containerContext, resolutionInfo);
                }
                throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
            }
            return this.ResolveType(containerContext, resolutionInfo);
        }

        public void Receive(RegistrationAdded message)
        {
            this.CollectInjectionMembers();
            if (this.constructorDelegate == null)
                this.CreateConstructorDelegate();
        }

        public void Receive(RegistrationRemoved message)
        {
            this.CollectInjectionMembers();
            this.CreateConstructorDelegate();
        }

        private object ResolveType(IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            var instance = this.constructorDelegate(resolutionInfo);

            if (this.hasInjectionMethods)
            {
                var methods = this.resolutionMethods.CreateCopy();
                var count = methods.Count;
                for (int i = 0; i < count; i++)
                {
                    methods[i].MethodDelegate(resolutionInfo, instance);
                }
            }

            return this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.instanceType, containerContext, resolutionInfo, this.injectionParameters);
        }

        private void CollectInjectionMembers()
        {
            this.resolutionProperties = this.metaInfoProvider.GetResolutionProperties(this.injectionParameters).ToArray();
            this.resolutionMethods = this.metaInfoProvider.GetResolutionMethods(this.injectionParameters).ToArray();
            this.hasInjectionMethods = this.resolutionMethods.Length > 0;
        }

        public void CleanUp()
        {
            this.messagePublisher.UnSubscribe<RegistrationAdded>(this);
            this.messagePublisher.UnSubscribe<RegistrationRemoved>(this);
            this.constructorDelegate = null;
        }
    }
}
