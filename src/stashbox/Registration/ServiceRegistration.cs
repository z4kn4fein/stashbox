using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration : IServiceRegistration
    {
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly IContainerContext containerContext;

        /// <inheritdoc />
        public Type ServiceType { get; }

        /// <inheritdoc />
        public Type ImplementationType { get; }

        /// <inheritdoc />
        public ILifetime LifetimeManager { get; }

        /// <inheritdoc />
        public IObjectBuilder ObjectBuilder { get; }

        /// <inheritdoc />
        public HashSet<Type> AttributeConditions { get; }

        /// <inheritdoc />
        public Type TargetTypeCondition { get; }

        /// <inheritdoc />
        public Func<TypeInformation, bool> ResolutionCondition { get; }

        internal ServiceRegistration(Type serviceType, Type implementationType, IContainerContext containerContext,
            ILifetime lifetimeManager, IObjectBuilder objectBuilder, IMetaInfoProvider metaInfoProvider, HashSet<Type> attributeConditions = null,
            Type targetTypeCondition = null, Func<TypeInformation, bool> resolutionCondition = null)
        {
            this.ImplementationType = implementationType;
            this.ServiceType = serviceType;
            this.containerContext = containerContext;
            this.LifetimeManager = lifetimeManager;
            this.ObjectBuilder = objectBuilder;
            this.AttributeConditions = attributeConditions;
            this.TargetTypeCondition = targetTypeCondition;
            this.ResolutionCondition = resolutionCondition;
            this.metaInfoProvider = metaInfoProvider;
            this.RegistrationNumber = containerContext.ReserveRegistrationNumber();
        }

        /// <inheritdoc />
        public int RegistrationNumber { get; }

        /// <inheritdoc />
        public bool IsUsableForCurrentContext(TypeInformation typeInfo) => this.TargetTypeCondition == null && this.ResolutionCondition == null && (this.AttributeConditions == null || !this.AttributeConditions.Any()) ||
                   this.TargetTypeCondition != null && typeInfo.ParentType != null && this.TargetTypeCondition == typeInfo.ParentType ||
                   this.AttributeConditions != null && typeInfo.CustomAttributes != null &&
                   this.AttributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any() ||
                   this.ResolutionCondition != null && this.ResolutionCondition(typeInfo);

        /// <inheritdoc />
        public bool HasCondition => this.TargetTypeCondition != null || this.ResolutionCondition != null ||
            this.AttributeConditions != null && this.AttributeConditions.Any();

        /// <inheritdoc />
        public bool ValidateGenericContraints(Type type) => !this.metaInfoProvider.HasGenericTypeConstraints ||
            this.metaInfoProvider.ValidateGenericContraints(type);
        
        /// <inheritdoc />
        public void CleanUp()
        {
            this.ObjectBuilder.CleanUp();
            this.LifetimeManager?.CleanUp();
        }

        /// <inheritdoc />
        public Expression GetExpression(ResolutionInfo resolutionInfo, Type resolveType)
        {
            var expr = this.LifetimeManager == null ? this.ObjectBuilder.GetExpression(resolutionInfo, resolveType) :
                this.LifetimeManager.GetExpression(this.containerContext, this.ObjectBuilder, resolutionInfo, resolveType);

            if (!this.containerContext.ContainerConfigurator.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                this.LifetimeManager != null && this.LifetimeManager.HandlesObjectDisposal ||
                this.ObjectBuilder.HandlesObjectDisposal ||
                !this.ImplementationType.GetTypeInfo().ImplementedInterfaces.Contains(Constants.DisposableType)) return expr;
            
            var method = Constants.AddDisposalMethod.MakeGenericMethod(this.ImplementationType);

            return Expression.Call(Constants.ScopeExpression, method, Expression.Convert(expr, this.ImplementationType));
        }
    }
}
