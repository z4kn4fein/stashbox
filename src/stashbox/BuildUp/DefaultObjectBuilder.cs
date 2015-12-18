using Ronin.Common;
using Sendstorm;
using Sendstorm.Infrastructure;
using Stashbox.BuildUp.DelegateFactory;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Entity.Resolution;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : IObjectBuilder, IMessageReceiver<RegistrationAdded>
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IMessagePublisher messagePublisher;
        private readonly object syncObject = new object();
        private volatile CreateInstance constructorDelegate;
        private ImmutableArray<ResolutionMember> resolutionMembers;
        private ImmutableArray<ResolutionMethod> resolutionMethods;
        private ResolutionConstructor resolutionConstructor;
        private Func<object> createDelegate;
        private bool hasInjectionMethods;
        private readonly Type instanceType;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IContainerContext containerContext;

        private SpinLock spinLock;

        public DefaultObjectBuilder(IContainerContext containerContext, IMetaInfoProvider metaInfoProvider, IContainerExtensionManager containerExtensionManager,
            IMessagePublisher messagePublisher, InjectionParameter[] injectionParameters = null)
        {
            if (injectionParameters != null)
                this.injectionParameters = injectionParameters;

            this.spinLock = new SpinLock();
            this.instanceType = metaInfoProvider.TypeTo;
            this.containerExtensionManager = containerExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
            this.messagePublisher = messagePublisher;
            this.CollectInjectionMembers();
            this.CreateConstructorDelegate();
            this.messagePublisher.Subscribe<RegistrationAdded>(this, addedEvent => this.metaInfoProvider.SensitivityList.Contains(addedEvent.RegistrationInfo.TypeFrom), ExecutionTarget.BackgroundThread);
        }

        private void CreateConstructorDelegate()
        {
            ResolutionConstructor constructor;
            if (!this.metaInfoProvider.TryChooseConstructor(out constructor,
                    injectionParameters: this.injectionParameters)) return;
            this.constructorDelegate = ExpressionDelegateFactory.CreateConstructorExpression(this.containerContext, constructor, this.resolutionMembers.CreateCopy());
            this.resolutionConstructor = constructor;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo)
        {
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
                            this.constructorDelegate = ExpressionDelegateFactory.CreateConstructorExpression(this.containerContext, constructor, this.resolutionMembers.CreateCopy());
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

        private object ResolveType(IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            var instance = this.constructorDelegate(resolutionInfo);

            if (!this.hasInjectionMethods)
                return this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.instanceType,
                    containerContext, resolutionInfo, this.injectionParameters);

            var lockTaken = false;
            try
            {
                this.spinLock.Enter(ref lockTaken);
                var methods = this.resolutionMethods.CreateCopy();
                if (lockTaken) this.spinLock.Exit();

                var count = methods.Count;
                for (var i = 0; i < count; i++)
                {
                    methods[i].MethodDelegate(resolutionInfo, instance);
                }
            }
            finally
            {
                if (this.spinLock.IsHeldByCurrentThread)
                    this.spinLock.Exit();
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
            return ExpressionDelegateFactory.CreateExpression(this.containerContext, constructor, resolutionInfo, this.resolutionMembers.CreateCopy());
        }

        private void CollectInjectionMembers()
        {
            var members = this.metaInfoProvider.GetResolutionMembers(this.injectionParameters).ToArray();
            var methods = this.metaInfoProvider.GetResolutionMethods(this.injectionParameters).ToArray();

            var lockTaken = false;
            try
            {
                this.spinLock.Enter(ref lockTaken);
                this.resolutionMembers = new ImmutableArray<ResolutionMember>(members);
                this.resolutionMethods = new ImmutableArray<ResolutionMethod>(methods);
                this.hasInjectionMethods = methods.Length > 0;

            }
            finally
            {
                if (lockTaken)
                    this.spinLock.Exit();
            }
        }

        public void CleanUp()
        {
            this.messagePublisher.UnSubscribe<RegistrationAdded>(this);
            this.constructorDelegate = null;
        }
    }
}
