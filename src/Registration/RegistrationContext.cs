using Stashbox.Configuration;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents the state of a scoped registration.
    /// </summary>
    public class RegistrationContext
    {
        internal static readonly RegistrationContext Empty = new RegistrationContext();

        /// <summary>
        /// Name of the registration.
        /// </summary>
        public object Name { get; internal set; }

        /// <summary>
        /// Container factory of the registration.
        /// </summary>
        public Delegate Factory { get; internal set; }

        /// <summary>
        /// Parameters to inject for the factory registration.
        /// </summary>
        public Type[] FactoryParameters { get; internal set; }

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
        public Action<object> Finalizer { get; internal set; }

        /// <summary>
        /// The initializer delegate.
        /// </summary>
        public Delegate Initializer { get; internal set; }

        /// <summary>
        /// The async initializer delegate.
        /// </summary>
        public Func<object, IDependencyResolver, CancellationToken, Task> AsyncInitializer { get; internal set; }

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
        /// The delegate to resolve when the registration is a 'Func{}'.
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
        public Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> ConstructorSelectionRule { get; internal set; }

        /// <summary>
        /// A filter delegate used to determine which members should be auto injected and which are not.
        /// </summary>
        public Func<MemberInfo, bool> AutoMemberInjectionFilter { get; internal set; }

        internal ExpandableArray<Type> TargetTypeConditions { get; set; }
        internal ExpandableArray<Func<TypeInformation, bool>> ResolutionConditions { get; set; }
        internal ExpandableArray<Type> AttributeConditions { get; set; }
        internal bool ReplaceExistingRegistration { get; set; }
        internal bool ReplaceExistingRegistrationOnlyIfExists { get; set; }
        internal ExpandableArray<Type> AdditionalServiceTypes { get; set; }
        internal ExpandableArray<KeyValuePair<string, object>> InjectionParameters { get; set; }

        internal RegistrationContext()
        {
            this.AttributeConditions = new ExpandableArray<Type>();
            this.TargetTypeConditions = new ExpandableArray<Type>();
            this.ResolutionConditions = new ExpandableArray<Func<TypeInformation, bool>>();
            this.AdditionalServiceTypes = new ExpandableArray<Type>();
            this.InjectionParameters = new ExpandableArray<KeyValuePair<string, object>>();
            this.DependencyBindings = new Dictionary<object, object>();
        }
    }
}
