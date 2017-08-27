using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.MetaInfo;
using Stashbox.Utils;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration : IServiceRegistration
    {
        private static readonly ConcurrentTree<Type, MetaInformation> MetaRepository = new ConcurrentTree<Type, MetaInformation>();

        private readonly IObjectBuilder objectBuilder;

        /// <inheritdoc />
        public MetaInformation MetaInformation { get; }

        /// <inheritdoc />
        public Type ServiceType { get; }

        /// <inheritdoc />
        public Type ImplementationType { get; }

        /// <inheritdoc />
        public RegistrationContextData RegistrationContext { get; }

        /// <inheritdoc />
        public bool IsDecorator { get; }

        /// <inheritdoc />
        public bool ShouldHandleDisposal { get; }

        /// <inheritdoc />
        public int RegistrationNumber { get; }

        internal ServiceRegistration(Type serviceType, Type implementationType, IContainerContext containerContext,
             IObjectBuilder objectBuilder, RegistrationContextData registrationContextData,
             bool isDecorator, bool shouldHandleDisposal)
        {
            this.objectBuilder = objectBuilder;
            this.ImplementationType = implementationType;
            this.ServiceType = serviceType;
            this.MetaInformation = GetOrCreateMetaInfo(implementationType);
            this.RegistrationNumber = containerContext.ReserveRegistrationNumber();
            this.RegistrationContext = registrationContextData;
            this.IsDecorator = isDecorator;
            this.ShouldHandleDisposal = shouldHandleDisposal;

            if (this.RegistrationContext.Name == null)
                this.RegistrationContext.Name = containerContext.ContainerConfigurator.ContainerConfiguration.SetUniqueRegistrationNames ? (object)this.RegistrationNumber : implementationType;
        }

        /// <inheritdoc />
        public bool IsUsableForCurrentContext(TypeInformation typeInfo) =>
            this.RegistrationContext.TargetTypeCondition == null && this.RegistrationContext.ResolutionCondition == null && (this.RegistrationContext.AttributeConditions == null || !this.RegistrationContext.AttributeConditions.Any()) ||
            this.RegistrationContext.TargetTypeCondition != null && typeInfo.ParentType != null && this.RegistrationContext.TargetTypeCondition == typeInfo.ParentType ||
            this.RegistrationContext.AttributeConditions != null && typeInfo.CustomAttributes != null &&
            this.RegistrationContext.AttributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any() ||
            this.RegistrationContext.ResolutionCondition != null && this.RegistrationContext.ResolutionCondition(typeInfo);

        /// <inheritdoc />
        public bool HasCondition => this.RegistrationContext.TargetTypeCondition != null || this.RegistrationContext.ResolutionCondition != null ||
            this.RegistrationContext.AttributeConditions != null && this.RegistrationContext.AttributeConditions.Count > 0;

        /// <inheritdoc />
        public bool ValidateGenericContraints(Type type) =>
            this.MetaInformation.GenericTypeConstraints.Count == 0 || this.MetaInformation.ValidateGenericContraints(type);

        /// <inheritdoc />
        public Expression GetExpression(ResolutionInfo resolutionInfo, Type resolveType) =>
            this.RegistrationContext.Lifetime == null || this.ServiceType.IsOpenGenericType() ?
                this.objectBuilder.GetExpression(this, resolutionInfo, resolveType) :
                this.RegistrationContext.Lifetime.GetExpression(this, this.objectBuilder, resolutionInfo, resolveType);

        private static MetaInformation GetOrCreateMetaInfo(Type typeTo)
        {
            var found = MetaRepository.GetOrDefault(typeTo);
            if (found != null) return found;

            var meta = new MetaInformation(typeTo);
            MetaRepository.AddOrUpdate(typeTo, meta);
            return meta;
        }
    }
}
