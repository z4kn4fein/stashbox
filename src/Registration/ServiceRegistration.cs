using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration
    {
        internal static readonly ServiceRegistration Empty = new();

        private static int GlobalRegistrationId;

        private static int GlobalRegistrationOrder;

        /// <summary>
        /// The registration order indicator.
        /// </summary>
        public int RegistrationOrder { get; private set; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        public readonly Type ImplementationType;

        /// <summary>
        /// The registration id.
        /// </summary>
        public readonly int RegistrationId;

        /// <summary>
        /// True if the registration is a decorator.
        /// </summary>
        public readonly bool IsDecorator;

        /// <summary>
        /// Determines whether the service's resolution should be handled by a dynamic <see cref="IDependencyResolver.Resolve(Type)"/> call on the current <see cref="IDependencyResolver"/> instead of a pre-built instantiation expression.
        /// </summary>
        public readonly bool IsResolutionCallRequired;

        /// <summary>
        /// Name of the registration.
        /// </summary>
        public readonly object? Name;

        /// <summary>
        /// The selected constructor if any was set.
        /// </summary>
        public readonly ConstructorInfo? SelectedConstructor;

        /// <summary>
        /// The arguments of the selected constructor if any was set.
        /// </summary>
        public readonly object[]? ConstructorArguments;

        /// <summary>
        /// Lifetime of the registration.
        /// </summary>
        public readonly LifetimeDescriptor Lifetime;

        /// <summary>
        /// Dependency names or types that are bound to named registrations.
        /// </summary>
        public readonly Dictionary<object, object?>? DependencyBindings;
        /// <summary>
        /// The cleanup delegate.
        /// </summary>
        public readonly Action<object>? Finalizer;

        /// <summary>
        /// The initializer delegate.
        /// </summary>
        public readonly Delegate? Initializer;

        /// <summary>
        /// The async initializer delegate.
        /// </summary>
        public readonly Func<object, IDependencyResolver, CancellationToken, Task>? AsyncInitializer;

        /// <summary>
        /// The auto member injection rule for the registration.
        /// </summary>
        public readonly Rules.AutoMemberInjectionRules AutoMemberInjectionRule;

        /// <summary>
        /// True if auto member injection is enabled on this instance.
        /// </summary>
        public readonly bool AutoMemberInjectionEnabled;

        /// <summary>
        /// True if the lifetime of the service is owned externally.
        /// </summary>
        public readonly bool IsLifetimeExternallyOwned;

        /// <summary>
        /// The name of the scope this registration defines.
        /// </summary>
        public readonly object? DefinedScopeName;

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        public readonly Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>>? ConstructorSelectionRule;

        /// <summary>
        /// A filter delegate used to determine which members should be auto injected and which are not.
        /// </summary>
        public readonly Func<MemberInfo, bool>? AutoMemberInjectionFilter;

        /// <summary>
        /// The additional metadata.
        /// </summary>
        public readonly object? Metadata;

        internal readonly bool ReplaceExistingRegistration;
        internal readonly bool ReplaceExistingRegistrationOnlyIfExists;
        internal readonly ExpandableArray<Type>? AdditionalServiceTypes;
        internal readonly ExpandableArray<KeyValuePair<string, object?>>? InjectionParameters;
        internal readonly bool HasScopeName;
        internal readonly object? NamedScopeRestrictionIdentifier;
        internal readonly bool HasCondition;
        internal readonly object RegistrationDiscriminator;

        private readonly ExpandableArray<Type>? targetTypeConditions;
        private readonly ExpandableArray<Func<TypeInformation, bool>>? resolutionConditions;
        private readonly ExpandableArray<Type>? attributeConditions;

        internal bool IsUsableForCurrentContext(TypeInformation typeInfo) =>
            this.HasParentTypeConditionAndMatch(typeInfo) ||
            this.HasAttributeConditionAndMatch(typeInfo) ||
            this.HasResolutionConditionAndMatch(typeInfo);

        internal void Replaces(ServiceRegistration serviceRegistration) =>
            this.RegistrationOrder = serviceRegistration.RegistrationOrder;

        private bool HasParentTypeConditionAndMatch(TypeInformation typeInfo) =>
            this.targetTypeConditions != null &&
            typeInfo.ParentType != null &&
            this.targetTypeConditions.Contains(typeInfo.ParentType);

        private bool HasAttributeConditionAndMatch(TypeInformation typeInfo) =>
            this.attributeConditions != null &&
            typeInfo.CustomAttributes != null &&
            this.attributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any();

        private bool HasResolutionConditionAndMatch(TypeInformation typeInfo)
        {
            if (this.resolutionConditions == null)
                return false;

            var length = this.resolutionConditions.Length;
            for (var i = 0; i < length; i++)
            {
                if (this.resolutionConditions[i](typeInfo))
                    return true;
            }

            return false;
        }

        private static int ReserveRegistrationId() =>
            Interlocked.Increment(ref GlobalRegistrationId);

        private static int ReserveRegistrationOrder() =>
            Interlocked.Increment(ref GlobalRegistrationOrder);

        private ServiceRegistration()
        {
            this.RegistrationDiscriminator = new object();
            this.Lifetime = Lifetimes.Transient;
            this.ImplementationType = this.GetType();
            this.ConstructorSelectionRule = Rules.ConstructorSelection.PreferMostParameters;
        }

        internal ServiceRegistration(Type implementationType, RegistrationContext registrationContext,
            ContainerConfiguration containerConfiguration, bool isDecorator)
            : this(implementationType,
                containerConfiguration,
                isDecorator,
                registrationContext.Name,
                registrationContext.Lifetime,
                registrationContext.SelectedConstructor,
                registrationContext.ConstructorArguments,
                registrationContext.DependencyBindings,
                registrationContext.Finalizer,
                registrationContext.Initializer,
                registrationContext.AsyncInitializer,
                registrationContext.AutoMemberInjectionRule,
                registrationContext.AutoMemberInjectionEnabled,
                registrationContext.IsLifetimeExternallyOwned,
                registrationContext.DefinedScopeName,
                registrationContext.ConstructorSelectionRule,
                registrationContext.AutoMemberInjectionFilter,
                registrationContext.Metadata,
                registrationContext.TargetTypeConditions,
                registrationContext.ResolutionConditions,
                registrationContext.AttributeConditions,
                registrationContext.ReplaceExistingRegistration,
                registrationContext.ReplaceExistingRegistrationOnlyIfExists,
                registrationContext.IsResolutionCallRequired,
                registrationContext.AdditionalServiceTypes,
                registrationContext.InjectionParameters)
        { }

        internal ServiceRegistration(Type implementationType, object? name,
            ContainerConfiguration containerConfiguration, ServiceRegistration baseRegistration)
            : this(implementationType,
                containerConfiguration,
                baseRegistration.IsDecorator,
                name,
                baseRegistration.Lifetime,
                baseRegistration.SelectedConstructor,
                baseRegistration.ConstructorArguments,
                baseRegistration.DependencyBindings,
                baseRegistration.Finalizer,
                baseRegistration.Initializer,
                baseRegistration.AsyncInitializer,
                baseRegistration.AutoMemberInjectionRule,
                baseRegistration.AutoMemberInjectionEnabled,
                baseRegistration.IsLifetimeExternallyOwned,
                baseRegistration.DefinedScopeName,
                baseRegistration.ConstructorSelectionRule,
                baseRegistration.AutoMemberInjectionFilter,
                baseRegistration.Metadata,
                baseRegistration.targetTypeConditions,
                baseRegistration.resolutionConditions,
                baseRegistration.attributeConditions,
                baseRegistration.ReplaceExistingRegistration,
                baseRegistration.ReplaceExistingRegistrationOnlyIfExists,
                baseRegistration.IsResolutionCallRequired,
                baseRegistration.AdditionalServiceTypes,
                baseRegistration.InjectionParameters)
        { }

        internal ServiceRegistration(
            Type implementationType,
            ContainerConfiguration containerConfiguration,
            bool isDecorator,
            object? name,
            LifetimeDescriptor? lifetime,
            ConstructorInfo? selectedConstructor = null,
            object[]? constructorArguments = null,
            Dictionary<object, object?>? dependencyBindings = null,
            Action<object>? finalizer = null,
            Delegate? initializer = null,
            Func<object, IDependencyResolver, CancellationToken, Task>? asyncInitializer = null,
            Rules.AutoMemberInjectionRules autoMemberInjectionRule = default,
            bool autoMemberInjectionEnabled = false,
            bool isLifetimeExternallyOwned = false,
            object? definedScopeName = null,
            Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>>? constructorSelectionRule = null,
            Func<MemberInfo, bool>? autoMemberInjectionFilter = null,
            object? metadata = null,
            ExpandableArray<Type>? targetTypeConditions = null,
            ExpandableArray<Func<TypeInformation, bool>>? resolutionConditions = null,
            ExpandableArray<Type>? attributeConditions = null,
            bool replaceExistingRegistration = false,
            bool replaceExistingRegistrationOnlyIfExists = false,
            bool isResolutionCallRequired = false,
            ExpandableArray<Type>? additionalServiceTypes = null,
            ExpandableArray<KeyValuePair<string, object?>>? injectionParameters = null)
        {
            this.ImplementationType = implementationType;
            this.IsDecorator = isDecorator;
            this.Name = name;
            this.SelectedConstructor = selectedConstructor;
            this.ConstructorArguments = constructorArguments;
            this.Lifetime = lifetime ?? containerConfiguration.DefaultLifetime;
            this.DependencyBindings = dependencyBindings;
            this.Finalizer = finalizer;
            this.Initializer = initializer;
            this.AsyncInitializer = asyncInitializer;
            this.AutoMemberInjectionRule = autoMemberInjectionRule;
            this.AutoMemberInjectionEnabled = autoMemberInjectionEnabled;
            this.IsLifetimeExternallyOwned = isLifetimeExternallyOwned;
            this.DefinedScopeName = definedScopeName;
            this.ConstructorSelectionRule = constructorSelectionRule;
            this.AutoMemberInjectionFilter = autoMemberInjectionFilter;
            this.Metadata = metadata;
            this.targetTypeConditions = targetTypeConditions;
            this.resolutionConditions = resolutionConditions;
            this.attributeConditions = attributeConditions;
            this.ReplaceExistingRegistration = replaceExistingRegistration;
            this.ReplaceExistingRegistrationOnlyIfExists = replaceExistingRegistrationOnlyIfExists;
            this.IsResolutionCallRequired = isResolutionCallRequired;
            this.AdditionalServiceTypes = additionalServiceTypes;
            this.InjectionParameters = injectionParameters;

            if (lifetime is NamedScopeLifetime namedScopeLifetime)
            {
                this.HasScopeName = true;
                this.NamedScopeRestrictionIdentifier = namedScopeLifetime.ScopeName;
            }

            this.HasCondition = targetTypeConditions != null ||
                                resolutionConditions != null ||
                                attributeConditions != null;

            this.RegistrationId = ReserveRegistrationId();
            this.RegistrationOrder = ReserveRegistrationOrder();
            this.RegistrationDiscriminator = containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.PreserveDuplications
                ? this.RegistrationId
                : name ?? implementationType;
        }
    }
}
