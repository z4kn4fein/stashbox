using Ronin.Common;
using Sendstorm.Infrastructure;
using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Entity.Events;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : IObjectBuilder, IMessageReceiver<RegistrationAdded>, IMessageReceiver<RegistrationRemoved>
    {
        private readonly IBuildExtensionManager buildExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IMessagePublisher messagePublisher;
        private readonly object syncObject = new object();
        private volatile Func<object[], object> constructorDelegate;
        private ResolutionConstructor constructor;

        public DefaultObjectBuilder(IMetaInfoProvider metaInfoProvider, IBuildExtensionManager buildExtensionManager, IMessagePublisher messagePublisher)
        {
            Shield.EnsureNotNull(metaInfoProvider);
            Shield.EnsureNotNull(buildExtensionManager);
            Shield.EnsureNotNull(messagePublisher);

            this.buildExtensionManager = buildExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.messagePublisher = messagePublisher;
            this.CreateConstructorDelegate();
            this.messagePublisher.Subscribe<RegistrationAdded>(this);
            this.messagePublisher.Subscribe<RegistrationRemoved>(this);
        }

        private void CreateConstructorDelegate()
        {
            if (this.constructorDelegate != null) return;
            lock (this.syncObject)
            {
                if (this.constructorDelegate != null) return;
                if (this.metaInfoProvider.TryChooseConstructor(out this.constructor))
                {
                    this.constructorDelegate = ExpressionBuilder.BuildConstructorExpression(
                        this.constructor.Constructor.Method,
                        this.constructor.Parameters.Select(parameter => parameter.ParameterInfo),
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
                        if (this.metaInfoProvider.TryChooseConstructor(out this.constructor, resolutionInfo.OverrideManager))
                        {
                            this.buildExtensionManager.ExecutePreBuildExtensions(builderContext, resolutionInfo);

                            this.constructorDelegate = ExpressionBuilder.BuildConstructorExpression(
                                this.constructor.Constructor.Method,
                                this.constructor.Parameters.Select(parameter => parameter.ParameterInfo),
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
                        this.buildExtensionManager.ExecutePreBuildExtensions(builderContext, resolutionInfo);
                        return this.ResolveType(builderContext, resolutionInfo);
                    }
                }
            }
            else
            {
                this.buildExtensionManager.ExecutePreBuildExtensions(builderContext, resolutionInfo);
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
            return this.buildExtensionManager.ExecutePostBuildExtensions(this.constructorDelegate(parameters), builderContext, resolutionInfo);
        }

        private object[] EvaluateParameters(IEnumerable<ResolutionParameter> parameters, ResolutionInfo info)
        {
            var resolutionParameters = parameters as ResolutionParameter[] ?? parameters.ToArray();
            var count = resolutionParameters.Count();
            var result = new object[count];
            for (var i = 0; i < count; i++)
            {
                var parameter = resolutionParameters.ElementAt(i);
                if (info.OverrideManager.ContainsValue(parameter.ParameterInfo))
                    result[i] = info.OverrideManager.GetOverriddenValue(parameter.ParameterInfo.Type, parameter.ParameterInfo.DependencyName);
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
