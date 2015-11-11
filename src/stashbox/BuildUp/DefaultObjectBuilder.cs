using Ronin.Common;
using Sendstorm.Infrastructure;
using Stashbox.BuildUp.DelegateFactory;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Entity.Resolution;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : IObjectBuilder, IMessageReceiver<RegistrationAdded>, IMessageReceiver<RegistrationRemoved>
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IMessagePublisher messagePublisher;
        private readonly IObjectExtender objectExtender;
        private readonly object syncObject = new object();
        private volatile CreateInstance constructorDelegate;
        //private ResolutionConstructor constructor;
        private volatile Func<ResolutionInfo, object> constructorFunc;
        private readonly Type instanceType;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IContainerContext containerContext;

        public DefaultObjectBuilder(IContainerContext containerContext, IMetaInfoProvider metaInfoProvider, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender,
            IMessagePublisher messagePublisher, InjectionParameter[] injectionParameters = null)
        {
            Shield.EnsureNotNull(metaInfoProvider);
            Shield.EnsureNotNull(containerContext);
            Shield.EnsureNotNull(containerExtensionManager);
            Shield.EnsureNotNull(messagePublisher);
            Shield.EnsureNotNull(objectExtender);

            if (injectionParameters != null)
                this.injectionParameters = injectionParameters;

            this.instanceType = metaInfoProvider.TypeTo;
            this.containerExtensionManager = containerExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
            this.messagePublisher = messagePublisher;
            this.objectExtender = objectExtender;
            this.CreateConstructorDelegate();
            this.messagePublisher.Subscribe<RegistrationAdded>(this, addedEvent => this.metaInfoProvider.SensitivityList.Contains(addedEvent.RegistrationInfo.TypeFrom));
            this.messagePublisher.Subscribe<RegistrationRemoved>(this, removedEvent => this.metaInfoProvider.SensitivityList.Contains(removedEvent.RegistrationInfo.TypeFrom));
        }

        private void CreateConstructorDelegate()
        {
            ResolutionConstructor constructor;
            if (this.metaInfoProvider.TryChooseConstructor(out constructor, injectionParameters: this.injectionParameters))
            {
                //this.constructorDelegate = ExpressionDelegateFactory.BuildConstructorExpression(
                //    constructor.Constructor,
                //    constructor.Parameters.Select(parameter => parameter.TypeInformation.Type),
                //    this.metaInfoProvider.TypeTo);
                //this.constructorFunc = this.CreateExpression(this)
                this.constructorFunc = ExpressionDelegateFactory.CreateConstructorExpression(this.containerContext, constructor);
            }
        }

        public object BuildInstance(ResolutionInfo resolutionInfo)
        {
            Shield.EnsureNotNull(containerContext);
            Shield.EnsureNotNull(resolutionInfo);

            if (this.constructorFunc == null)
            {
                ResolutionConstructor constructor;
                if (this.metaInfoProvider.TryChooseConstructor(out constructor, resolutionInfo, this.injectionParameters))
                {
                    //this.constructorDelegate = ExpressionDelegateFactory.BuildConstructorExpression(
                    //    constructor.Constructor,
                    //    constructor.Parameters.Select(parameter => parameter.TypeInformation.Type),
                    //    this.metaInfoProvider.TypeTo);
                    this.constructorFunc = ExpressionDelegateFactory.CreateConstructorExpression(this.containerContext, constructor);

                    return this.ResolveType(containerContext, resolutionInfo);
                }
                throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
            }
            return this.ResolveType(containerContext, resolutionInfo);
        }

        public void Receive(RegistrationAdded message)
        {
            this.CreateConstructorDelegate();
        }

        public void Receive(RegistrationRemoved message)
        {
            this.CreateConstructorDelegate();
        }

        private object ResolveType(IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            //var parameters = this.EvaluateParameters(containerContext, this.constructor.Parameters, resolutionInfo);
            var instance = this.objectExtender.ExtendObject(this.constructorFunc(resolutionInfo), containerContext, resolutionInfo);
            return this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.instanceType, containerContext, resolutionInfo, this.injectionParameters);
        }

        private object[] EvaluateParameters(IContainerContext containerContext, ResolutionTarget[] parameters, ResolutionInfo info)
        {
            var result = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                result[i] = containerContext.ResolutionStrategy.EvaluateResolutionTarget(containerContext, parameter, info);
            }
            return result;
        }
        
        public void CleanUp()
        {
            this.objectExtender.CleanUp();
            this.messagePublisher.UnSubscribe<RegistrationAdded>(this);
            this.messagePublisher.UnSubscribe<RegistrationRemoved>(this);
            this.constructorDelegate = null;
        }
    }
}
