using Stashbox.BuildUp.DelegateFactory;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : IObjectBuilder
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly string registrationName;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly object syncObject = new object();
        private readonly object resolutionMemberSyncObject = new object();
        private readonly object resolutionMethodSyncObject = new object();
        private volatile CreateInstance constructorDelegate;
        private volatile ResolutionMember[] resolutionMembers;
        private volatile ResolutionMethod[] resolutionMethods;
        private ResolutionConstructor resolutionConstructor;
        private Func<ResolutionInfo, object> createDelegate;
        private readonly Type instanceType;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IContainerContext containerContext;

        public DefaultObjectBuilder(IContainerContext containerContext, IMetaInfoProvider metaInfoProvider,
            IContainerExtensionManager containerExtensionManager, string registrationName,
            InjectionParameter[] injectionParameters = null)
        {
            if (injectionParameters != null)
                this.injectionParameters = injectionParameters;

            this.instanceType = metaInfoProvider.TypeTo;
            this.containerExtensionManager = containerExtensionManager;
            this.registrationName = registrationName;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (this.metaInfoProvider.HasInjectionMethod || this.containerExtensionManager.HasPostBuildExtensions)
            {
                if (this.constructorDelegate != null) return this.ResolveType(containerContext, resolutionInfo);

                lock (this.syncObject)
                {
                    if (this.constructorDelegate != null) return this.ResolveType(containerContext, resolutionInfo);

                    ResolutionConstructor constructor;
                    if (!this.metaInfoProvider.TryChooseConstructor(out constructor, resolutionInfo,
                            this.injectionParameters))
                        throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                    this.constructorDelegate = ExpressionDelegateFactory.CreateConstructorExpression(this.containerContext, constructor, this.GetResolutionMembers());
                    this.resolutionConstructor = constructor;
                    return this.ResolveType(containerContext, resolutionInfo);

                }

            }

            if (this.createDelegate != null) return this.createDelegate(resolutionInfo);
            this.createDelegate = this.containerContext.DelegateRepository.GetOrAdd(this.registrationName, () =>
            {
                ResolutionConstructor constructor;
                if (!this.metaInfoProvider.TryChooseConstructor(out constructor, resolutionInfo,
                    this.injectionParameters))
                    throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);

                var parameter = Expression.Parameter(typeof(ResolutionInfo));
                this.createDelegate = Expression.Lambda<Func<ResolutionInfo, object>>(this.GetExpressionInternal(constructor, resolutionInfo, parameter), parameter).Compile();
                this.resolutionConstructor = constructor;
                return this.createDelegate;
            });
            return this.createDelegate(resolutionInfo);
        }

        private object ResolveType(IContainerContext containerContext, ResolutionInfo resolutionInfo)
        {
            var instance = this.constructorDelegate(resolutionInfo);

            if (!this.metaInfoProvider.HasInjectionMethod)
                return this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.instanceType,
                    containerContext, resolutionInfo, this.injectionParameters);


            var methods = this.GetResolutionMethods();

            var count = methods.Length;
            for (var i = 0; i < count; i++)
            {
                methods[i].MethodDelegate(resolutionInfo, instance);
            }

            return this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.instanceType, containerContext, resolutionInfo, this.injectionParameters);
        }

        private ResolutionMember[] GetResolutionMembers()
        {
            if (!this.metaInfoProvider.HasInjectionMembers) return null;

            if (this.resolutionMembers != null) return this.resolutionMembers;
            lock (this.resolutionMemberSyncObject)
            {
                if (this.resolutionMembers != null) return this.resolutionMembers;
                return this.resolutionMembers = this.metaInfoProvider.GetResolutionMembers(this.injectionParameters).ToArray();
            }
        }

        private ResolutionMethod[] GetResolutionMethods()
        {
            if (!this.metaInfoProvider.HasInjectionMethod) return null;

            if (this.resolutionMethods != null) return this.resolutionMethods;
            lock (this.resolutionMethodSyncObject)
            {
                if (this.resolutionMethods != null) return this.resolutionMethods;
                return this.resolutionMethods = this.metaInfoProvider.GetResolutionMethods(this.injectionParameters).ToArray();
            }
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            if (this.resolutionConstructor != null) return this.GetExpressionInternal(this.resolutionConstructor, resolutionInfo, resolutionInfoExpression);
            {
                lock (this.syncObject)
                {
                    if (this.resolutionConstructor != null) return this.GetExpressionInternal(this.resolutionConstructor, resolutionInfo, resolutionInfoExpression);
                    {
                        ResolutionConstructor constructor;
                        if (!this.metaInfoProvider.TryChooseConstructor(out constructor, injectionParameters: this.injectionParameters))
                            throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                        this.resolutionConstructor = constructor;
                        return this.GetExpressionInternal(constructor, resolutionInfo, resolutionInfoExpression);
                    }
                }
            }
        }

        private Expression GetExpressionInternal(ResolutionConstructor constructor, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            return ExpressionDelegateFactory.CreateExpression(this.containerContext, constructor, resolutionInfo, resolutionInfoExpression, this.GetResolutionMembers());
        }

        public void CleanUp()
        {
            this.constructorDelegate = null;
        }
    }
}
