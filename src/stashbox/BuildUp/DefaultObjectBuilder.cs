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
using System.Linq.Expressions;

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
        private ResolutionConstructor resolutionConstructor;
        private Func<object> createDelegate;
        private bool hasInjectionMethods;
        private readonly Type instanceType;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IContainerContext containerContext;

        public DefaultObjectBuilder(IContainerContext containerContext, IMetaInfoProvider metaInfoProvider, IContainerExtensionManager containerExtensionManager,
            IMessagePublisher messagePublisher, InjectionParameter[] injectionParameters = null)
        {
            Shield.EnsureNotNull(() => metaInfoProvider);
            Shield.EnsureNotNull(() => containerContext);
            Shield.EnsureNotNull(() => containerExtensionManager);
            Shield.EnsureNotNull(() => messagePublisher);

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
            if (!this.metaInfoProvider.TryChooseConstructor(out constructor,
                    injectionParameters: this.injectionParameters)) return;
            this.constructorDelegate = ExpressionDelegateFactory.CreateConstructorExpression(this.containerContext, constructor, this.resolutionProperties);
            this.resolutionConstructor = constructor;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo)
        {
            Shield.EnsureNotNull(() => resolutionInfo);

            if (resolutionInfo.OverrideManager != null || this.hasInjectionMethods || this.containerExtensionManager.HasPostBuildExtensions)
            {
                if (this.constructorDelegate != null) return this.ResolveType(containerContext, resolutionInfo);
                {
                    lock (this.syncObject)
                    {
                        if (this.constructorDelegate != null) return this.ResolveType(containerContext, resolutionInfo);
                        {
                            ResolutionConstructor constructor;
                            if (!this.metaInfoProvider.TryChooseConstructor(out constructor, resolutionInfo,
                                    this.injectionParameters))
                                throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                            this.constructorDelegate = ExpressionDelegateFactory.CreateConstructorExpression(this.containerContext, constructor, this.resolutionProperties);
                            this.resolutionConstructor = constructor;
                            return this.ResolveType(containerContext, resolutionInfo);
                        }
                    }
                }
            }
            else
            {
                if (this.createDelegate != null) return this.createDelegate();
                {
                    lock (this.syncObject)
                    {
                        if (this.createDelegate != null) return this.createDelegate();
                        {
                            ResolutionConstructor constructor;
                            if (!this.metaInfoProvider.TryChooseConstructor(out constructor, resolutionInfo,
                                    this.injectionParameters))
                                throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                            this.createDelegate = Expression.Lambda<Func<object>>(this.GetExpressionInternal(constructor, resolutionInfo)).Compile();
                            this.resolutionConstructor = constructor;
                            return this.createDelegate();
                        }
                    }
                }
            }
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

            if (!this.hasInjectionMethods)
                return this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.instanceType,
                    containerContext, resolutionInfo, this.injectionParameters);
            var methods = this.resolutionMethods.CreateCopy();
            var count = methods.Count;
            for (var i = 0; i < count; i++)
            {
                methods[i].MethodDelegate(resolutionInfo, instance);
            }

            return this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.instanceType, containerContext, resolutionInfo, this.injectionParameters);
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            if (this.resolutionConstructor != null) return this.GetExpressionInternal(this.resolutionConstructor, resolutionInfo);
            {
                lock (this.syncObject)
                {
                    if (this.resolutionConstructor != null) return this.GetExpressionInternal(this.resolutionConstructor, resolutionInfo);
                    {
                        ResolutionConstructor constructor;
                        if (!this.metaInfoProvider.TryChooseConstructor(out constructor, resolutionInfo,
                                this.injectionParameters))
                            throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                        this.resolutionConstructor = constructor;
                        return this.GetExpressionInternal(constructor, resolutionInfo);
                    }
                }
            }
        }

        private Expression GetExpressionInternal(ResolutionConstructor constructor, ResolutionInfo resolutionInfo)
        {
            return ExpressionDelegateFactory.CreateExpression(this.containerContext, constructor, resolutionInfo, this.resolutionProperties);
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
