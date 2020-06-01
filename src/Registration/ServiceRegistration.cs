using Stashbox.Configuration;
using Stashbox.Exceptions;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration
    {
        private static int globalRegistrationOrder;
        private readonly ContainerConfiguration containerConfiguration;

        /// <summary>
        /// The implementation type.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// The registration context.
        /// </summary>
        public RegistrationContext RegistrationContext { get; }

        /// <summary>
        /// The registration number.
        /// </summary>
        public int RegistrationId { get; private set; }

        /// <summary>
        /// The registration id.
        /// </summary>
        public object RegistrationName { get; }

        /// <summary>
        /// True if the registration is a decorator.
        /// </summary>
        public bool IsDecorator { get; }

        /// <summary>
        /// Represents the nature of the registration.
        /// </summary>
        public RegistrationType RegistrationType { get; }

        internal TypeInfo ImplementationTypeInfo { get; }

        internal bool IsResolvableByUnnamedRequest { get; }

        internal bool HasScopeName { get; }

        internal bool HasCondition { get; }

        internal ServiceRegistration(Type implementationType, RegistrationType registrationType, ContainerConfiguration containerConfiguration,
            RegistrationContext registrationContext, bool isDecorator)
        {
            this.containerConfiguration = containerConfiguration;
            this.ImplementationType = implementationType;
            this.ImplementationTypeInfo = implementationType.GetTypeInfo();
            this.RegistrationContext = registrationContext;
            this.IsDecorator = isDecorator;
            this.RegistrationType = registrationType;

            this.IsResolvableByUnnamedRequest = this.RegistrationContext.Name == null || containerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled;

            this.HasScopeName = this.RegistrationContext.NamedScopeRestrictionIdentifier != null;

            this.HasCondition = this.RegistrationContext.TargetTypeCondition != null || this.RegistrationContext.ResolutionCondition != null ||
                this.RegistrationContext.AttributeConditions != null && this.RegistrationContext.AttributeConditions.Any();

            this.RegistrationId = ReserveRegistrationOrder();
            this.RegistrationName = this.RegistrationContext.Name ??
                (containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.PreserveDuplications
                ? (object)this.RegistrationId
                : implementationType);
        }

        internal bool IsUsableForCurrentContext(TypeInformation typeInfo) =>
            this.HasParentTypeConditionAndMatch(typeInfo) ||
            this.HasAttributeConditionAndMatch(typeInfo) ||
            this.HasResolutionConditionAndMatch(typeInfo);

        internal bool CanInjectIntoNamedScope(IEnumerable<object> scopeNames) =>
            scopeNames.Last() == this.RegistrationContext.NamedScopeRestrictionIdentifier;

        internal ServiceRegistration Clone(Type implementationType, RegistrationType registrationType) =>
            new ServiceRegistration(implementationType, registrationType, this.containerConfiguration,
                this.RegistrationContext, this.IsDecorator);

        internal void Replaces(ServiceRegistration serviceRegistration)
        {
            if (this.containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.ThrowException)
                throw new ServiceAlreadyRegisteredException(this.ImplementationType);

            this.RegistrationId = serviceRegistration.RegistrationId;
        }

        private bool HasParentTypeConditionAndMatch(TypeInformation typeInfo) =>
            this.RegistrationContext.TargetTypeCondition != null && typeInfo.ParentType != null && this.RegistrationContext.TargetTypeCondition == typeInfo.ParentType;

        private bool HasAttributeConditionAndMatch(TypeInformation typeInfo) =>
            this.RegistrationContext.AttributeConditions != null && typeInfo.CustomAttributes != null &&
            this.RegistrationContext.AttributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any();

        private bool HasResolutionConditionAndMatch(TypeInformation typeInfo) =>
            this.RegistrationContext.ResolutionCondition != null && this.RegistrationContext.ResolutionCondition(typeInfo);

        private static int ReserveRegistrationOrder() =>
            Interlocked.Increment(ref globalRegistrationOrder);
    }
}
