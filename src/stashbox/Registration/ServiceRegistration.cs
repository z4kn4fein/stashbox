using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.Utils;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration : IServiceRegistration
    {
        /// <inheritdoc />
        public IMetaInfoProvider MetaInfoProvider { get; }

        /// <inheritdoc />
        public Type ServiceType { get; }

        /// <inheritdoc />
        public Type ImplementationType { get; }

        /// <inheritdoc />
        public ILifetime LifetimeManager { get; }

        /// <inheritdoc />
        public IObjectBuilder ObjectBuilder { get; }

        /// <inheritdoc />
        public RegistrationContextData RegistrationContext { get; }

        /// <inheritdoc />
        public bool IsDecorator { get; }

        /// <inheritdoc />
        public bool ShouldHandleDisposal { get; }

        /// <inheritdoc />
        public int RegistrationNumber { get; }

        internal ServiceRegistration(Type serviceType, Type implementationType, int registrationNumber,
            ILifetime lifetimeManager, IObjectBuilder objectBuilder, IMetaInfoProvider metaInfoProvider, 
            RegistrationContextData registrationContextData, bool isDecorator, bool shouldHandleDisposal)
        {
            this.ImplementationType = implementationType;
            this.ServiceType = serviceType;
            this.LifetimeManager = lifetimeManager;
            this.ObjectBuilder = objectBuilder;
            this.MetaInfoProvider = metaInfoProvider;
            this.RegistrationNumber = registrationNumber;
            this.RegistrationContext = registrationContextData;
            this.IsDecorator = isDecorator;
            this.ShouldHandleDisposal = shouldHandleDisposal;

            this.RegistrationContext.Name = NameGenerator.GetRegistrationName(serviceType, implementationType, this.RegistrationContext.Name);
        }

        /// <inheritdoc />
        public bool IsUsableForCurrentContext(TypeInformation typeInfo) => this.RegistrationContext.TargetTypeCondition == null && this.RegistrationContext.ResolutionCondition == null && (this.RegistrationContext.AttributeConditions == null || !this.RegistrationContext.AttributeConditions.Any()) ||
                   this.RegistrationContext.TargetTypeCondition != null && typeInfo.ParentType != null && this.RegistrationContext.TargetTypeCondition == typeInfo.ParentType ||
                   this.RegistrationContext.AttributeConditions != null && typeInfo.CustomAttributes != null &&
                   this.RegistrationContext.AttributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any() ||
                   this.RegistrationContext.ResolutionCondition != null && this.RegistrationContext.ResolutionCondition(typeInfo);

        /// <inheritdoc />
        public bool HasCondition => this.RegistrationContext.TargetTypeCondition != null || this.RegistrationContext.ResolutionCondition != null ||
            this.RegistrationContext.AttributeConditions != null && this.RegistrationContext.AttributeConditions.Any();

        /// <inheritdoc />
        public bool ValidateGenericContraints(Type type) => !this.MetaInfoProvider.HasGenericTypeConstraints ||
            this.MetaInfoProvider.ValidateGenericContraints(type);
        
        /// <inheritdoc />
        public Expression GetExpression(ResolutionInfo resolutionInfo, Type resolveType)
        {
            var expr = this.LifetimeManager == null || this.ServiceType.IsOpenGenericType() ?
                this.ObjectBuilder.GetExpression(this, resolutionInfo, resolveType) :
                this.LifetimeManager.GetExpression(this, this.ObjectBuilder, resolutionInfo, resolveType);

            return expr;
        }
    }
}
