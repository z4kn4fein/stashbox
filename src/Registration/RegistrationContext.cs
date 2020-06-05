using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Registration.Fluent;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents the state of a scoped registration.
    /// </summary>
    public class RegistrationContext
    {
        internal static RegistrationContext Empty = new RegistrationContext();

        /// <summary>
        /// Indicates that the current registration should replace an existing one.
        /// </summary>
        public bool ReplaceExistingRegistration { get; internal set; }

        /// <summary>
        /// Contains the additional service types the current registration mapped to.
        /// </summary>
        public IEnumerable<Type> AdditionalServiceTypes { get; internal set; }

        /// <summary>
        /// Name of the registration.
        /// </summary>
        public object Name { get; internal set; }

        /// <summary>
        /// Container factory of the registration.
        /// </summary>
        public Delegate ContainerFactory { get; internal set; }

        /// <summary>
        /// Parameterless factory of the registration.
        /// </summary>
        public Delegate SingleFactory { get; internal set; }

        /// <summary>
        /// Injection parameters of the registration.
        /// </summary>
        public IEnumerable<KeyValuePair<string, object>> InjectionParameters { get; internal set; }

        /// <summary>
        /// The selected constructor if any was set.
        /// </summary>
        public ConstructorInfo SelectedConstructor { get; internal set; }

        /// <summary>
        /// The arguments of the selected constructor if any was set.
        /// </summary>
        public object[] ConstructorArguments { get; internal set; }

        /// <summary>
        /// Lifetime of the registration.
        /// </summary>
        public LifetimeDescriptor Lifetime { get; internal set; }

        /// <summary>
        /// Target type condition of the registration.
        /// </summary>
        public Type TargetTypeCondition { get; internal set; }

        /// <summary>
        /// Resolution condition of the registration.
        /// </summary>
        public Func<TypeInformation, bool> ResolutionCondition { get; internal set; }

        /// <summary>
        /// Attribute condition collection of the registration.
        /// </summary>
        public IEnumerable<Type> AttributeConditions { get; internal set; }

        /// <summary>
        /// Member names which are explicitly set to be filled by the container.
        /// </summary>
        public Dictionary<string, object> InjectionMemberNames { get; internal set; }

        /// <summary>
        /// Dependency names or types that are bound to named registrations.
        /// </summary>
        public Dictionary<object, object> DependencyBindings { get; internal set; }

        /// <summary>
        /// The already stored instance which was provided by instance or wireup registration.
        /// </summary>
        public object ExistingInstance { get; internal set; }

        /// <summary>
        /// The cleanup delegate.
        /// </summary>
        public Delegate Finalizer { get; internal set; }

        /// <summary>
        /// The initializer delegate.
        /// </summary>
        public Delegate Initializer { get; internal set; }

        /// <summary>
        /// The auto member injection rule for the registration.
        /// </summary>
        public Rules.AutoMemberInjectionRules AutoMemberInjectionRule { get; internal set; }

        /// <summary>
        /// True if auto member injection is enabled on this instance.
        /// </summary>
        public bool AutoMemberInjectionEnabled { get; internal set; }

        /// <summary>
        /// True if the lifetime of the service is owned externally.
        /// </summary>
        public bool IsLifetimeExternallyOwned { get; internal set; }

        /// <summary>
        /// Holds the func delegate, if the registration is a factory.
        /// </summary>
        public Delegate FuncDelegate { get; internal set; }

        /// <summary>
        /// The name of the scope this registration defines.
        /// </summary>
        public object DefinedScopeName { get; internal set; }

        /// <summary>
        /// The scope name set by <see cref="FluentServiceConfigurator{TConfigurator}.InNamedScope(object)"/> and <see cref="FluentServiceConfigurator{TConfigurator}.InScopeDefinedBy(Type)"/> to tell where this registration must used.
        /// </summary>
        public object NamedScopeRestrictionIdentifier { get; internal set; }

        /// <summary>
        /// If true, the existing instance will be wired into the container, it will perform member and method injection on it.
        /// </summary>
        public bool IsWireUp { get; internal set; }

        /// <summary>
        /// Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.
        /// </summary>
        public bool IsFactoryDelegateACompiledLambda { get; internal set; }

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        public Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> ConstructorSelectionRule { get; internal set; }

        /// <summary>
        /// A filter delegate used to determine which members should be auto injected and which are not.
        /// </summary>
        public Func<MemberInfo, bool> AutoMemberInjectionFilter { get; internal set; }

        internal RegistrationContext()
        {
            this.AttributeConditions = new ExpandableArray<Type>();
            this.AdditionalServiceTypes = new ExpandableArray<Type>();
            this.InjectionParameters = new ExpandableArray<KeyValuePair<string, object>>();
            this.InjectionMemberNames = new Dictionary<string, object>();
            this.DependencyBindings = new Dictionary<object, object>();
        }
    }
}
