using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents the state of a scoped registration.
    /// </summary>
    public class RegistrationContextData
    {
        /// <summary>
        /// Empty registration data.
        /// </summary>
        public static RegistrationContextData New() => new RegistrationContextData();

        /// <summary>
        /// The service type.
        /// </summary>
        public Type ServiceType { get; internal set; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        public Type ImplementationType { get; internal set; }

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
        public InjectionParameter[] InjectionParameters { get; internal set; }

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
        public HashSet<Type> AttributeConditions { get; internal set; }

        /// <summary>
        /// The stored instance.
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
        /// The auto memeber injection rule for the registration.
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
        /// The name of the scope in which this registration should be used.
        /// </summary>
        public object UsedScopeName { get; internal set; }

        /// <summary>
        /// The name of the scope this registration defines.
        /// </summary>
        public object DefinedScopeName { get; internal set; }

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        public Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> ConstructorSelectionRule { get; internal set; }

        /// <summary>
        /// Constructs a <see cref="RegistrationContextData"/>
        /// </summary>
        public RegistrationContextData()
        {
            this.AttributeConditions = new HashSet<Type>();
            this.AutoMemberInjectionEnabled = false;
        }

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns>The copy of this instance.</returns>
        public RegistrationContextData CreateCopy()
        {
            var data = (RegistrationContextData)this.MemberwiseClone();
            data.Lifetime = data.Lifetime?.Create();
            return data;
        }
    }
}
