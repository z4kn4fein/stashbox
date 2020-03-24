using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Lifetime;
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
        /// <summary>
        /// Empty registration data.
        /// </summary>
        public static readonly RegistrationContext Empty = New();

        /// <summary>
        /// Empty registration data.
        /// </summary>
        public static RegistrationContext New() => new RegistrationContext();

        /// <summary>
        /// Indicates that the current registration should replace an existing one.
        /// </summary>
        public bool ReplaceExistingRegistration { get; internal set; }

        /// <summary>
        /// Indicates that the factory delegate for the current registration shouldn't be cached.
        /// </summary>
        public bool FactoryCacheDisabled { get; internal set; }

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
        public Func<IDependencyResolver, object> ContainerFactory { get; internal set; }

        /// <summary>
        /// Parameterless factory of the registration.
        /// </summary>
        public Func<object> SingleFactory { get; internal set; }

        /// <summary>
        /// Injection parameters of the registration.
        /// </summary>
        public IEnumerable<InjectionParameter> InjectionParameters { get; internal set; }

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
        public ILifetime Lifetime { get; internal set; }

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
        public object Finalizer { get; internal set; }

        /// <summary>
        /// The initializer delegate.
        /// </summary>
        public object Initializer { get; internal set; }

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
        public Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> ConstructorSelectionRule { get; internal set; }

        /// <summary>
        /// A filter delegate used to determine which members should be auto injected and which are not.
        /// </summary>
        public Func<TypeInformation, bool> MemberInjectionFilter { get; internal set; }

        /// <summary>
        /// Constructs a <see cref="RegistrationContext"/>
        /// </summary>
        public RegistrationContext()
        {
            this.AttributeConditions = new List<Type>();
            this.AdditionalServiceTypes = new List<Type>();
            this.InjectionParameters = new List<InjectionParameter>();
            this.InjectionMemberNames = new Dictionary<string, object>();
            this.DependencyBindings = new Dictionary<object, object>();
            this.AutoMemberInjectionEnabled = false;
        }

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns>The copy of this instance.</returns>
        public RegistrationContext Clone()
        {
#if IL_EMIT
            var data = Cloner<RegistrationContext>.Clone(this);
#else
            var data = (RegistrationContext)this.MemberwiseClone();
#endif
            data.Lifetime = data.Lifetime?.Create();
            return data;
        }
    }
}
