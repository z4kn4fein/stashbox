using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using Stashbox.Entity.Resolution;

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
        public static RegistrationContextData Empty = new RegistrationContextData();

        /// <summary>
        /// Name of the registration.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Container factory of the registration.
        /// </summary>
        public Func<IStashboxContainer, object> ContainerFactory { get; set; }

        /// <summary>
        /// Parameterless factory of the registration.
        /// </summary>
        public Func<object> SingleFactory { get; set; }

        /// <summary>
        /// Injection parameters of the registration.
        /// </summary>
        public InjectionParameter[] InjectionParameters { get; set; }

        /// <summary>
        /// Lifetime of the registration.
        /// </summary>
        public ILifetime Lifetime { get; set; }

        /// <summary>
        /// Target type condition of the registration.
        /// </summary>
        public Type TargetTypeCondition { get; set; }

        /// <summary>
        /// Resolution condition of the registration.
        /// </summary>
        public Func<TypeInformation, bool> ResolutionCondition { get; set; }

        /// <summary>
        /// Attribute condition collection of the registration.
        /// </summary>
        public HashSet<Type> AttributeConditions { get; set; }

        /// <summary>
        /// The stored instance.
        /// </summary>
        public object ExistingInstance { get; set; }

        /// <summary>
        /// The auto memeber injection rule for the registration.
        /// </summary>
        public Rules.AutoMemberInjection AutoMemberInjectionRule { get; set; }

        /// <summary>
        /// True if auto member injection is enabled on this instance.
        /// </summary>
        public bool AutoMemberInjectionEnabled { get; set; }

        /// <summary>
        /// True if the lifetime of the service is owned externally.
        /// </summary>
        public bool IsLifetimeExternallyOwned { get; set; }

        /// <summary>
        /// The constructor selection rule.
        /// </summary>
        public Func<IEnumerable<ResolutionConstructor>, ResolutionConstructor> ConstructorSelectionRule { get; set; }

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
