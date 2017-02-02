using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.BuildUp.Expressions;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : IObjectBuilder
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IMetaInfoProvider metaInfoProvider;
        private volatile ResolutionMember[] resolutionMembers;
        private volatile ResolutionMethod[] resolutionMethods;
        private Expression expression;
        private Func<ResolutionInfo, object> createDelegate;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IContainerContext containerContext;
        private bool isDirty;
        
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
            if (this.createDelegate != null && !this.isDirty) return this.createDelegate(resolutionInfo);
            var expr = this.GetExpressionInternal(resolutionInfo, resolveType);
            this.createDelegate = Expression.Lambda<Func<ResolutionInfo, object>>(expr).Compile();

            return this.createDelegate(resolutionInfo);
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (!this.containerContext.ContainerConfiguration.CircularDependencyTrackingEnabled)
                return this.GetExpressionInternal(resolutionInfo, resolveType);

            using (new CircularDependencyBarrier(resolutionInfo.CircularDependencyBarrier, this.metaInfoProvider.TypeTo))
                return this.GetExpressionInternal(resolutionInfo, resolveType);
        }

        private Expression GetExpressionInternal(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (this.expression != null && !this.isDirty) return this.expression;
            {
                ResolutionConstructor constructor;
                if (!this.metaInfoProvider.TryChooseConstructor(out constructor, this.injectionParameters))
                    throw new ResolutionFailedException(this.metaInfoProvider.TypeTo.FullName);
                this.isDirty = false;
                this.expression = this.CreateExpression(constructor, resolutionInfo, resolveType);
                return this.expression;
            }
        }

        public void ServiceUpdated(RegistrationInfo registrationInfo)
        {
            if (!this.metaInfoProvider.SensitivityList.Contains(registrationInfo.TypeFrom)) return;
            this.isDirty = true;
        }

        private Expression CreateExpression(ResolutionConstructor constructor, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return ExpressionDelegateFactory.CreateExpression(this.containerExtensionManager, this.containerContext,
                    constructor, resolutionInfo, resolveType, this.injectionParameters, this.GetResolutionMembers(resolutionInfo), this.GetResolutionMethods(resolutionInfo));
        }

        private ResolutionMember[] GetResolutionMembers(ResolutionInfo resolutionInfo)
        {
            if (!this.metaInfoProvider.HasInjectionMembers) return null;

            if (this.resolutionMembers != null && !this.isDirty) return this.resolutionMembers;
            return this.resolutionMembers = this.metaInfoProvider.GetResolutionMembers(this.injectionParameters).ToArray();
        }

        private ResolutionMethod[] GetResolutionMethods(ResolutionInfo resolutionInfo)
        {
            if (!this.metaInfoProvider.HasInjectionMethod) return null;

            if (this.resolutionMethods != null && !this.isDirty) return this.resolutionMethods;
            return this.resolutionMethods = this.metaInfoProvider.GetResolutionMethods(this.injectionParameters).ToArray();
        }

        public void CleanUp()
        {
            this.createDelegate = null;
        }
    }
}
