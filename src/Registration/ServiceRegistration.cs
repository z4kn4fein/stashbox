using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using Stashbox.Utils.Data;

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
                containerConfiguration.RegistrationBehavior,
                isDecorator,
                registrationContext.Name,
                registrationContext.SelectedConstructor,
                registrationContext.ConstructorArguments,
                registrationContext.Lifetime ?? containerConfiguration.DefaultLifetime,
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
                registrationContext.AdditionalServiceTypes,
                registrationContext.InjectionParameters)
        { }

        internal ServiceRegistration(Type implementationType, object? name,
            ContainerConfiguration containerConfiguration, ServiceRegistration baseRegistration)
            : this(implementationType,
                containerConfiguration.RegistrationBehavior,
                baseRegistration.IsDecorator,
                name,
                baseRegistration.SelectedConstructor,
                baseRegistration.ConstructorArguments,
                baseRegistration.Lifetime,
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
                baseRegistration.AdditionalServiceTypes,
                baseRegistration.InjectionParameters)
        { }

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

        private ServiceRegistration(
            Type implementationType,
            Rules.RegistrationBehavior currentRegistrationBehavior,
            bool isDecorator,
            object? name,
            ConstructorInfo? selectedConstructor,
            object[]? constructorArguments,
            LifetimeDescriptor lifetime,
            Dictionary<object, object?>? dependencyBindings,
            Action<object>? finalizer,
            Delegate? initializer,
            Func<object, IDependencyResolver, CancellationToken, Task>? asyncInitializer,
            Rules.AutoMemberInjectionRules autoMemberInjectionRule,
            bool autoMemberInjectionEnabled,
            bool isLifetimeExternallyOwned,
            object? definedScopeName,
            Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>>? constructorSelectionRule,
            Func<MemberInfo, bool>? autoMemberInjectionFilter,
            object? metadata,
            ExpandableArray<Type>? targetTypeConditions,
            ExpandableArray<Func<TypeInformation, bool>>? resolutionConditions,
            ExpandableArray<Type>? attributeConditions,
            bool replaceExistingRegistration,
            bool replaceExistingRegistrationOnlyIfExists,
            ExpandableArray<Type>? additionalServiceTypes,
            ExpandableArray<KeyValuePair<string, object?>>? injectionParameters)
        {
            this.ImplementationType = implementationType;
            this.IsDecorator = isDecorator;
            this.Name = name;
            this.SelectedConstructor = selectedConstructor;
            this.ConstructorArguments = constructorArguments;
            this.Lifetime = lifetime;
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
            this.RegistrationDiscriminator = currentRegistrationBehavior == Rules.RegistrationBehavior.PreserveDuplications
                ? this.RegistrationId
                : name ?? implementationType;
        }
    }
}
