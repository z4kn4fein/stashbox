using Ronin.Common;
using Sendstorm.Infrastructure;
using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Entity.Resolution;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : IObjectBuilder, IMessageReceiver<RegistrationAdded>, IMessageReceiver<RegistrationRemoved>
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IMessagePublisher messagePublisher;
        private readonly IObjectExtender objectExtender;
        private readonly object syncObject = new object();
        private volatile Func<object[], object> constructorDelegate;
        private ResolutionConstructor constructor;
        private readonly Type instanceType;
        private readonly HashSet<InjectionParameter> injectionParameters;

        public DefaultObjectBuilder(IMetaInfoProvider metaInfoProvider, IContainerExtensionManager containerExtensionManager, IObjectExtender objectExtender,
            IMessagePublisher messagePublisher, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            Shield.EnsureNotNull(metaInfoProvider);
            Shield.EnsureNotNull(containerExtensionManager);
            Shield.EnsureNotNull(messagePublisher);
            Shield.EnsureNotNull(objectExtender);

            if (injectionParameters != null)
                this.injectionParameters = new HashSet<InjectionParameter>(injectionParameters);

            this.instanceType = metaInfoProvider.TypeTo;
            this.containerExtensionManager = containerExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.messagePublisher = messagePublisher;
            this.objectExtender = objectExtender;
            this.CreateConstructorDelegate();
            this.messagePublisher.Subscribe<RegistrationAdded>(this, addedEvent => this.metaInfoProvider.SensitivityList.Contains(addedEvent.RegistrationInfo.TypeFrom));
            this.messagePublisher.Subscribe<RegistrationRemoved>(this, removedEvent => this.metaInfoProvider.SensitivityList.Contains(removedEvent.RegistrationInfo.TypeFrom));
        }

        private void CreateConstructorDelegate()
        {
            if (this.constructorDelegate != null) return;
            lock (this.syncObject)
            {
                if (this.constructorDelegate != null) return;
                if (this.metaInfoProvider.TryChooseConstructor(out this.constructor, injectionParameters: this.injectionParameters))
                {
                    this.constructorDelegate = ExpressionBuilder.BuildConstructorExpression(
                        this.constructor.Constructor,
                        this.constructor.Parameters.Select(parameter => parameter.TypeInformation),
                        this.metaInfoProvider.TypeTo);
                }
            }
        }

        public object BuildInstance(IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            Shield.EnsureNotNull(containerContext);
            Shield.EnsureNotNull(resolutionInfo);

            if (this.constructorDelegate == null)
            {
                lock (this.syncObject)
                {
                    if (this.constructorDelegate == null)
                    {
                        if (this.metaInfoProvider.TryChooseConstructor(out this.constructor, resolutionInfo, this.injectionParameters))
                        {
                            this.constructorDelegate = ExpressionBuilder.BuildConstructorExpression(
                                this.constructor.Constructor,
                                this.constructor.Parameters.Select(parameter => parameter.TypeInformation),
                                this.metaInfoProvider.TypeTo);

                            return this.ResolveType(containerContext, resolutionInfo);
                        }
                        throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                    }
                    return this.ResolveType(containerContext, resolutionInfo);
                }
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
            var parameters = this.EvaluateParameters(containerContext, this.constructor.Parameters, resolutionInfo);
            var instance = this.objectExtender.ExtendObject(this.constructorDelegate(parameters), containerContext, resolutionInfo);
            return this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.instanceType, containerContext, resolutionInfo, this.injectionParameters);
        }

        private object[] EvaluateParameters(IContainerContext containerContext, IEnumerable<ResolutionTarget> parameters, ResolutionInfo info)
        {
            var resolutionParameters = parameters as ResolutionTarget[] ?? parameters.ToArray();
            var count = resolutionParameters.Count();
            var result = new object[count];
            for (var i = 0; i < count; i++)
            {
                var parameter = resolutionParameters.ElementAt(i);
                result[i] = containerContext.ResolutionStrategy.EvaluateResolutionTarget(containerContext, parameter, info);
            }
            return result;
        }

        public void CleanUp()
        {
            this.messagePublisher.UnSubscribe<RegistrationAdded>(this);
            this.messagePublisher.UnSubscribe<RegistrationRemoved>(this);
            this.constructorDelegate = null;
        }
    }
}
