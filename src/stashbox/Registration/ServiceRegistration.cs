using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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
        public bool ValidateGenericContraints(TypeInformation typeInformation) => !this.metaInfoProvider.HasGenericTypeConstraints ||
            this.metaInfoProvider.ValidateGenericContraints(typeInformation);
        
        /// <inheritdoc />
        public void CleanUp()
        {
            this.ObjectBuilder.CleanUp();
            this.LifetimeManager.CleanUp();
        }

        /// <inheritdoc />
        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            var expr = this.LifetimeManager.GetExpression(this.containerContext, this.ObjectBuilder, resolutionInfo, resolveType);
            if (!this.containerContext.ContainerConfigurator.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                !this.LifetimeManager.IsTransient || this.ObjectBuilder.HandlesObjectDisposal) return expr;

            var call = Expression.Call(Expression.Constant(this), "AddTransientObjectTracking", null, expr);
            return Expression.Convert(call, resolveType.Type);
        }

        private object AddTransientObjectTracking(object instance)
        {
            if (instance is IDisposable)
                this.containerContext.TrackedTransientObjects.Add(instance);
            return instance;
        }
    }
}
