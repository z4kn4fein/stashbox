using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    [DebuggerDisplay("Name = {Name}, Lifetime = {Lifetime.Name}", Name = "{ImplementationType}")]
    public class ServiceRegistration
    {
        private static int GlobalRegistrationId = int.MinValue;

        private static int GlobalRegistrationOrder = int.MinValue;

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
        public object? Name { get; internal set; }

        /// <summary>
        /// Lifetime of the registration.
        /// </summary>
        public LifetimeDescriptor Lifetime { get; internal set; }

        /// <summary>
        /// The registration order indicator.
        /// </summary>
        public int RegistrationOrder { get; private set; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        public Type ImplementationType { get; internal set; }

        /// <summary>
        /// Advanced registration options.
        /// </summary>
        public IReadOnlyDictionary<RegistrationOption, object?>? RegistrationOptions => this.Options;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal Dictionary<RegistrationOption, object?>? Options;

        internal ServiceRegistration(Type implementationType, object? name,
            LifetimeDescriptor lifetimeDescriptor, bool isDecorator, Dictionary<RegistrationOption, object?>? options = null, 
            int? registrationId = null, int? order = null)
        {
            this.ImplementationType = implementationType;
            this.IsDecorator = isDecorator;
            this.Name = name;
            this.Lifetime = lifetimeDescriptor;
            this.Options = options;
            this.RegistrationId = registrationId ?? ReserveRegistrationId();
            this.RegistrationOrder = order ?? ReserveRegistrationOrder();
        }

        internal void Replaces(ServiceRegistration serviceRegistration) =>
            this.RegistrationOrder = serviceRegistration.RegistrationOrder;

        internal bool IsFactory() => Options.GetOrDefault(RegistrationOption.RegistrationTypeOptions) is FactoryOptions;

        internal bool IsInstance() => Options.GetOrDefault(RegistrationOption.RegistrationTypeOptions) is InstanceOptions;

        internal bool IsUsableForCurrentContext(TypeInformation typeInfo, ConditionOptions conditionOptions) =>
            HasParentTypeConditionAndMatch(typeInfo, conditionOptions) ||
            HasAttributeConditionAndMatch(typeInfo, conditionOptions) ||
            HasResolutionConditionAndMatch(typeInfo, conditionOptions);

        private bool HasParentTypeConditionAndMatch(TypeInformation typeInfo, ConditionOptions conditionOptions) =>
            conditionOptions.TargetTypeConditions != null &&
            typeInfo.ParentType != null &&
            conditionOptions.TargetTypeConditions.Contains(typeInfo.ParentType);

        private bool HasAttributeConditionAndMatch(TypeInformation typeInfo, ConditionOptions conditionOptions) =>
            conditionOptions.AttributeConditions != null &&
            typeInfo.CustomAttributes != null &&
            conditionOptions.AttributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any();

        private bool HasResolutionConditionAndMatch(TypeInformation typeInfo, ConditionOptions conditionOptions)
        {
            if (conditionOptions.ResolutionConditions == null)
                return false;
            var length = conditionOptions.ResolutionConditions.Length;
            for (var i = 0; i < length; i++)
            {
                if (conditionOptions.ResolutionConditions[i](typeInfo))
                    return true;
            }
            return false;
        }

        private static int ReserveRegistrationId() =>
            Interlocked.Increment(ref GlobalRegistrationId);

        private static int ReserveRegistrationOrder() =>
            Interlocked.Increment(ref GlobalRegistrationOrder);

    }

    /// <summary>
    /// 
    /// </summary>
    public enum RegistrationOption
    {
        /// <summary>
        /// Determines whether the service's resolution should be handled by a dynamic <see cref="IDependencyResolver.Resolve(Type)"/> call on the current <see cref="IDependencyResolver"/> instead of a pre-built instantiation expression.
        /// </summary>
        IsResolutionCallRequired,

        /// <summary>
        /// Constructor related registration options.
        /// </summary>
        ConstructorOptions,

        /// <summary>
        /// Auto member injection related registration options.
        /// </summary>
        AutoMemberOptions,

        /// <summary>
        /// Dependency names or types that are bound to named registrations.
        /// </summary>
        DependencyBindings,

        /// <summary>
        /// The cleanup delegate.
        /// </summary>
        Finalizer,

        /// <summary>
        /// The initializer delegate.
        /// </summary>
        Initializer,

        /// <summary>
        /// The async initializer delegate.
        /// </summary>
        AsyncInitializer,

        /// <summary>
        /// True if the lifetime of the service is owned externally.
        /// </summary>
        IsLifetimeExternallyOwned,

        /// <summary>
        /// The name of the scope this registration defines.
        /// </summary>
        DefinedScopeName,

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        ConstructorSelectionRule,

        /// <summary>
        /// The additional metadata.
        /// </summary>
        Metadata,

        /// <summary>
        /// Indicates whther this registration should replace an existing registration.
        /// </summary>
        ReplaceExistingRegistration,

        /// <summary>
        /// Indicates whther this registration should replace a registration only when it's exist.
        /// </summary>
        ReplaceExistingRegistrationOnlyIfExists,

        /// <summary>
        /// Additional service types to map.
        /// </summary>
        AdditionalServiceTypes,

        /// <summary>
        /// Injection parameters.
        /// </summary>
        InjectionParameters,

        /// <summary>
        /// Condition related registration options.
        /// </summary>
        ConditionOptions,

        /// <summary>
        /// Options related to instance or factory registrations.
        /// </summary>
        RegistrationTypeOptions,
    }

    /// <summary>
    /// Represents the factory registration options.
    /// </summary>
    public class FactoryOptions
    {
        /// <summary>
        /// Container factory of the registration.
        /// </summary>
        public readonly Delegate Factory;

        /// <summary>
        /// Parameters to inject for the factory registration.
        /// </summary>
        public readonly Type[] FactoryParameters;

        /// <summary>
        /// Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.
        /// </summary>
        public readonly bool IsFactoryDelegateACompiledLambda;

        internal FactoryOptions(Delegate factory, Type[] factoryParameters, bool isCompiledLambda)
        {
            this.Factory = factory;
            this.FactoryParameters = factoryParameters;
            this.IsFactoryDelegateACompiledLambda = isCompiledLambda;
        }
    }

    /// <summary>
    /// Represents the instance registration options.
    /// </summary>
    public class InstanceOptions
    {
        /// <summary>
        /// If true, the existing instance will be wired into the container, it will perform member and method injection on it.
        /// </summary>
        public readonly bool IsWireUp;

        /// <summary>
        /// The already stored instance which was provided by instance or wired up registration.
        /// </summary>
        public readonly object ExistingInstance;

        internal InstanceOptions(object existingInstance, bool isWireUp)
        {
            this.IsWireUp = isWireUp;
            this.ExistingInstance = existingInstance;
        }
    }

    /// <summary>
    /// Represents the auto member injection related registration options.
    /// </summary>
    public class AutoMemberOptions
    {
        /// <summary>
        /// The auto member injection rule for the registration.
        /// </summary>
        public readonly Rules.AutoMemberInjectionRules AutoMemberInjectionRule;

        /// <summary>
        /// A filter delegate used to determine which members should be auto injected and which are not.
        /// </summary>
        public readonly Func<MemberInfo, bool>? AutoMemberInjectionFilter;

        internal AutoMemberOptions(Rules.AutoMemberInjectionRules autoMemberInjectionRule, Func<MemberInfo, bool>? autoMemberInjectionFilter)
        {
            this.AutoMemberInjectionRule = autoMemberInjectionRule;
            this.AutoMemberInjectionFilter = autoMemberInjectionFilter;
        }
    }

    /// <summary>
    /// Represents the constructor related registration options.
    /// </summary>
    public class ConstructorOptions
    {
        /// <summary>
        /// The selected constructor if any was set.
        /// </summary>
        public readonly ConstructorInfo SelectedConstructor;

        /// <summary>
        /// The arguments of the selected constructor if any was set.
        /// </summary>
        public readonly object[]? ConstructorArguments;

        internal ConstructorOptions(ConstructorInfo selectedConstructor, object[]? constructorArguments)
        {
            this.SelectedConstructor = selectedConstructor;
            this.ConstructorArguments = constructorArguments;
        }
    }

    internal class ConditionOptions
    {
        internal ExpandableArray<Type>? TargetTypeConditions { get; set; }
        internal ExpandableArray<Func<TypeInformation, bool>>? ResolutionConditions { get; set; }
        internal ExpandableArray<Type>? AttributeConditions { get; set; }
    }
}
