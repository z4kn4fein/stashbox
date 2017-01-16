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
        private bool isConstructorDirty;
        private bool isMembersDirty;
        private bool isMethodDirty;

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
                if (this.constructorDelegate != null && !this.isConstructorDirty) return this.ResolveType(resolutionInfo, resolveType);

                lock (this.syncObject)
                {
                    if (this.constructorDelegate != null && !this.isConstructorDirty) return this.ResolveType(resolutionInfo, resolveType);

                    ResolutionConstructor constructor;
                    if (!this.metaInfoProvider.TryChooseConstructor(out constructor, resolutionInfo,
                            this.injectionParameters))
                        throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                    this.constructorDelegate = ExpressionDelegateFactory.CreateConstructorExpression(this.containerContext, constructor, this.GetResolutionMembers());
                    this.resolutionConstructor = constructor;
                    this.isConstructorDirty = false;
                    return this.ResolveType(resolutionInfo, resolveType);
                }

            }

            if (this.createDelegate != null && !this.isConstructorDirty) return this.createDelegate(resolutionInfo);
            this.createDelegate = this.containerContext.DelegateRepository.GetOrAdd(this.registrationName, () =>
            {
                ResolutionConstructor constructor;
                if (!this.metaInfoProvider.TryChooseConstructor(out constructor, resolutionInfo,
                    this.injectionParameters))
                    throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);

                var parameter = Expression.Parameter(typeof(ResolutionInfo));
                this.createDelegate = Expression.Lambda<Func<ResolutionInfo, object>>(this.GetExpressionInternal(constructor, resolutionInfo, parameter), parameter).Compile();
                this.resolutionConstructor = constructor;
                this.isConstructorDirty = false;
                return this.createDelegate;
            }, this.isConstructorDirty);

            return this.createDelegate(resolutionInfo);
        }

        private object ResolveType(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            var instance = this.constructorDelegate(resolutionInfo);

            if (!this.metaInfoProvider.HasInjectionMethod)
                return this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.instanceType,
                    this.containerContext, resolutionInfo, resolveType, this.injectionParameters);


            var methods = this.GetResolutionMethods();

            var count = methods.Length;
            for (var i = 0; i < count; i++)
            {
                methods[i].MethodDelegate(resolutionInfo, instance);
            }

            return this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.instanceType, this.containerContext, resolutionInfo, resolveType, this.injectionParameters);
        }

        private ResolutionMember[] GetResolutionMembers()
        {
            if (!this.metaInfoProvider.HasInjectionMembers) return null;

            if (this.resolutionMembers != null && !this.isMembersDirty) return this.resolutionMembers;
            lock (this.resolutionMemberSyncObject)
            {
                if (this.resolutionMembers != null && !this.isMembersDirty) return this.resolutionMembers;
                this.isMembersDirty = false;
                return this.resolutionMembers = this.metaInfoProvider.GetResolutionMembers(this.injectionParameters).ToArray();
            }
        }

        private ResolutionMethod[] GetResolutionMethods()
        {
            if (!this.metaInfoProvider.HasInjectionMethod) return null;

            if (this.resolutionMethods != null && !this.isMethodDirty) return this.resolutionMethods;
            lock (this.resolutionMethodSyncObject)
            {
                if (this.resolutionMethods != null && !this.isMethodDirty) return this.resolutionMethods;
                this.isMethodDirty = false;
                return this.resolutionMethods = this.metaInfoProvider.GetResolutionMethods(this.injectionParameters).ToArray();
            }
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            if (this.resolutionConstructor != null && !this.isConstructorDirty) return this.GetExpressionInternal(this.resolutionConstructor, resolutionInfo, resolutionInfoExpression);
            {
                lock (this.syncObject)
                {
                    if (this.resolutionConstructor != null && !this.isConstructorDirty) return this.GetExpressionInternal(this.resolutionConstructor, resolutionInfo, resolutionInfoExpression);
                    {
                        ResolutionConstructor constructor;
                        if (!this.metaInfoProvider.TryChooseConstructor(out constructor, injectionParameters: this.injectionParameters))
                            throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                        this.resolutionConstructor = constructor;
                        this.isConstructorDirty = false;
                        return this.GetExpressionInternal(constructor, resolutionInfo, resolutionInfoExpression);
                    }
                }
            }
        }

        public void ServiceUpdated(RegistrationInfo registrationInfo)
        {
            if (!this.metaInfoProvider.SensitivityList.Contains(registrationInfo.TypeFrom)) return;
            this.isConstructorDirty = true;
            this.isMembersDirty = true;
            this.isMethodDirty = true;
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
