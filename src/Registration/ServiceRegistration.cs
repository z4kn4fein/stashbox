using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using System;
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

        /// <summary>
        /// The implementation type.
        /// </summary>
        public Type ImplementationType { get; }

        /// <summary>
        /// The registration context.
        /// </summary>
        public RegistrationContext RegistrationContext { get; }

        /// <summary>
        /// The registration id.
        /// </summary>
        public int RegistrationId { get; private set; }

        /// <summary>
        /// The object used to synchronized operations.
        /// </summary>
        public object SynchronizationObject { get; }

        /// <summary>
        /// True if the registration is a decorator.
        /// </summary>
        public bool IsDecorator { get; }

        /// <summary>
        /// Represents the nature of the registration.
        /// </summary>
        public RegistrationType RegistrationType { get; }

        internal readonly TypeInfo ImplementationTypeInfo;

        internal readonly bool IsResolvableByUnnamedRequest;

        internal readonly bool HasScopeName;

        internal readonly object NamedScopeRestrictionIdentifier;

        internal readonly bool HasCondition;

        internal readonly object RegistrationDiscriminator;

        private protected readonly ContainerConfiguration Configuration;

        internal ServiceRegistration(Type implementationType, RegistrationType registrationType, ContainerConfiguration containerConfiguration,
            RegistrationContext registrationContext, bool isDecorator)
        {
            this.Configuration = containerConfiguration;
            this.ImplementationType = implementationType;
            this.ImplementationTypeInfo = implementationType.GetTypeInfo();
            this.RegistrationContext = registrationContext;
            this.IsDecorator = isDecorator;
            this.RegistrationType = registrationType;
            this.SynchronizationObject = new object();

            this.IsResolvableByUnnamedRequest = this.RegistrationContext.Name == null || containerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled;

            if (this.RegistrationContext.Lifetime is NamedScopeLifetime lifetime)
            {
                this.HasScopeName = true;
                this.NamedScopeRestrictionIdentifier = lifetime.ScopeName;
            }

            this.HasCondition = this.RegistrationContext.TargetTypeCondition != null || this.RegistrationContext.ResolutionCondition != null ||
                this.RegistrationContext.AttributeConditions != null && this.RegistrationContext.AttributeConditions.Any();

            this.RegistrationId = ReserveRegistrationOrder();
            this.RegistrationDiscriminator = containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.PreserveDuplications
                    ? this.RegistrationId
                    : this.RegistrationContext.Name ?? implementationType;
        }

        internal bool IsUsableForCurrentContext(TypeInformation typeInfo) =>
            this.HasParentTypeConditionAndMatch(typeInfo) ||
            this.HasAttributeConditionAndMatch(typeInfo) ||
            this.HasResolutionConditionAndMatch(typeInfo);

        internal void Replaces(ServiceRegistration serviceRegistration) =>
            this.RegistrationId = serviceRegistration.RegistrationId;

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
