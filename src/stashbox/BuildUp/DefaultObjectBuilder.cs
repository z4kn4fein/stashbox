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
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly object syncObject = new object();
        private readonly object resolutionMemberSyncObject = new object();
        private readonly object resolutionMethodSyncObject = new object();
        private volatile ResolutionMember[] resolutionMembers;
        private volatile ResolutionMethod[] resolutionMethods;
        private ResolutionConstructor resolutionConstructor;
        private Func<ResolutionInfo, object> createDelegate;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IContainerContext containerContext;
        private bool isConstructorDirty;
        private bool isMembersDirty;
        private bool isMethodDirty;

        public DefaultObjectBuilder(IContainerContext containerContext, IMetaInfoProvider metaInfoProvider,
            IContainerExtensionManager containerExtensionManager, InjectionParameter[] injectionParameters = null)
        {
            if (injectionParameters != null)
                this.injectionParameters = injectionParameters;

            this.containerExtensionManager = containerExtensionManager;
            this.metaInfoProvider = metaInfoProvider;
            this.containerContext = containerContext;
        }

        public object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (!this.containerContext.ContainerConfiguration.CircularDependencyTrackingEnabled)
                return this.BuildInstanceInternal(resolutionInfo, resolveType);

            using (new CircularDependencyBarrier(resolutionInfo.CircularDependencyBarrier, this.metaInfoProvider.TypeTo))
                return this.BuildInstanceInternal(resolutionInfo, resolveType);
        }

        private object BuildInstanceInternal(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (this.createDelegate != null && !this.isConstructorDirty) return this.createDelegate(resolutionInfo);
            lock (this.syncObject)
            {
                if (this.createDelegate != null && !this.isConstructorDirty) return this.createDelegate(resolutionInfo);
                ResolutionConstructor rConstructor;
                if (!this.metaInfoProvider.TryChooseConstructor(out rConstructor, resolutionInfo,
                    this.injectionParameters))
                    throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);

                var parameter = Expression.Parameter(typeof(ResolutionInfo));
                var expr = this.CreateExpression(rConstructor, resolutionInfo, resolveType, parameter);
                this.createDelegate = Expression.Lambda<Func<ResolutionInfo, object>>(expr, parameter).Compile();

                this.resolutionConstructor = rConstructor;
                this.isConstructorDirty = false;

                return this.createDelegate(resolutionInfo);
            }

        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            if (!this.containerContext.ContainerConfiguration.CircularDependencyTrackingEnabled)
                return this.GetExpressionInternal(resolutionInfo, resolutionInfoExpression, resolveType);

            using (new CircularDependencyBarrier(resolutionInfo.CircularDependencyBarrier, this.metaInfoProvider.TypeTo))
                return this.GetExpressionInternal(resolutionInfo, resolutionInfoExpression, resolveType);
        }

        private Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            if (this.resolutionConstructor != null && !this.isConstructorDirty) return this.CreateExpression(this.resolutionConstructor, resolutionInfo, resolveType, resolutionInfoExpression);
            {
                lock (this.syncObject)
                {
                    if (this.resolutionConstructor != null && !this.isConstructorDirty) return this.CreateExpression(this.resolutionConstructor, resolutionInfo, resolveType, resolutionInfoExpression);
                    {
                        ResolutionConstructor constructor;
                        if (!this.metaInfoProvider.TryChooseConstructor(out constructor, injectionParameters: this.injectionParameters))
                            throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                        this.resolutionConstructor = constructor;
                        this.isConstructorDirty = false;
                        return this.CreateExpression(constructor, resolutionInfo, resolveType, resolutionInfoExpression);
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

        private Expression CreateExpression(ResolutionConstructor constructor, ResolutionInfo resolutionInfo, TypeInformation resolveType, Expression resolutionInfoExpression)
        {
            return ExpressionDelegateFactory.CreateExpression(this.containerExtensionManager, this.containerContext,
                    constructor, resolutionInfo, resolutionInfoExpression, resolveType, this.injectionParameters, this.GetResolutionMembers(resolutionInfo), this.GetResolutionMethods(resolutionInfo));
        }

        private ResolutionMember[] GetResolutionMembers(ResolutionInfo resolutionInfo)
        {
            if (!this.metaInfoProvider.HasInjectionMembers) return null;

            if (this.resolutionMembers != null && !this.isMembersDirty) return this.resolutionMembers;
            lock (this.resolutionMemberSyncObject)
            {
                if (this.resolutionMembers != null && !this.isMembersDirty) return this.resolutionMembers;
                this.isMembersDirty = false;
                return this.resolutionMembers = this.metaInfoProvider.GetResolutionMembers(resolutionInfo, this.injectionParameters).ToArray();
            }
        }

        private ResolutionMethod[] GetResolutionMethods(ResolutionInfo resolutionInfo)
        {
            if (!this.metaInfoProvider.HasInjectionMethod) return null;

            if (this.resolutionMethods != null && !this.isMethodDirty) return this.resolutionMethods;
            lock (this.resolutionMethodSyncObject)
            {
                if (this.resolutionMethods != null && !this.isMethodDirty) return this.resolutionMethods;
                this.isMethodDirty = false;
                return this.resolutionMethods = this.metaInfoProvider.GetResolutionMethods(resolutionInfo, this.injectionParameters).ToArray();
            }
        }

        public void CleanUp()
        {
            this.createDelegate = null;
        }
    }
}
