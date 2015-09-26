using Ronin.Common;
using Sendstorm.Infrastructure;
using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Entity.Events;
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
        private readonly object syncObject = new object();
        private volatile Func<object[], object> constructorDelegate;
        private ResolutionConstructor constructor;
        private readonly HashSet<InjectionParameter> injectionParameters;

        public DefaultObjectBuilder(IMetaInfoProvider metaInfoProvider, IContainerExtensionManager containerExtensionManager, IMessagePublisher messagePublisher, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            Shield.EnsureNotNull(metaInfoProvider);
            Shield.EnsureNotNull(containerExtensionManager);
            Shield.EnsureNotNull(messagePublisher);

            if (injectionParameters != null)
                this.injectionParameters = new HashSet<InjectionParameter>(injectionParameters);

            this.containerExtensionManager = containerExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.messagePublisher = messagePublisher;
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
                        this.constructor.Constructor.Method,
                        this.constructor.Parameters.Select(parameter => parameter.TypeInformation),
                        this.metaInfoProvider.TypeTo);
                }
            }
        }

        public object BuildInstance(IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            Shield.EnsureNotNull(builderContext);
            Shield.EnsureNotNull(resolutionInfo);

            if (this.constructorDelegate == null)
            {
                lock (this.syncObject)
                {
                    if (this.constructorDelegate == null)
                    {
                        if (this.metaInfoProvider.TryChooseConstructor(out this.constructor, resolutionInfo.OverrideManager, this.injectionParameters))
                        {
                            this.containerExtensionManager.ExecutePreBuildExtensions(builderContext, resolutionInfo, this.injectionParameters);

                            this.constructorDelegate = ExpressionBuilder.BuildConstructorExpression(
                                this.constructor.Constructor.Method,
                                this.constructor.Parameters.Select(parameter => parameter.TypeInformation),
                                this.metaInfoProvider.TypeTo);

                            return this.ResolveType(builderContext, resolutionInfo);
                        }
                        else
                        {
                            throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                        }
                    }
                    else
                    {
                        this.containerExtensionManager.ExecutePreBuildExtensions(builderContext, resolutionInfo, this.injectionParameters);
                        return this.ResolveType(builderContext, resolutionInfo);
                    }
                }
            }
            else
            {
                this.containerExtensionManager.ExecutePreBuildExtensions(builderContext, resolutionInfo, this.injectionParameters);
                return this.ResolveType(builderContext, resolutionInfo);
            }
        }

        public void Receive(RegistrationAdded message)
        {
            this.CreateConstructorDelegate();
        }

        public void Receive(RegistrationRemoved message)
        {
            this.CreateConstructorDelegate();
        }

        private object ResolveType(IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            var parameters = this.EvaluateParameters(this.constructor.Parameters, resolutionInfo);
            return this.containerExtensionManager.ExecutePostBuildExtensions(this.constructorDelegate(parameters), builderContext, resolutionInfo, this.injectionParameters);
        }

        private object[] EvaluateParameters(IEnumerable<ResolutionTarget> parameters, ResolutionInfo info)
        {
            var resolutionParameters = parameters as ResolutionTarget[] ?? parameters.ToArray();
            var count = resolutionParameters.Count();
            var result = new object[count];
            for (var i = 0; i < count; i++)
            {
                var parameter = resolutionParameters.ElementAt(i);
                if (info.OverrideManager.ContainsValue(parameter.TypeInformation))
                    result[i] = info.OverrideManager.GetOverriddenValue(parameter.TypeInformation.Type, parameter.TypeInformation.DependencyName);
                else if (parameter.ResolutionTargetValue != null)
                    result[i] = parameter.ResolutionTargetValue;
                else
                    result[i] = parameter.Resolver.Resolve(info);
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
