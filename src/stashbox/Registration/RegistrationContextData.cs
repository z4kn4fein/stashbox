using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents the state of a scoped registration.
    /// </summary>
    public class RegistrationContextData
    {
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
        /// Indicates that the scope management enabled or not on this registration.
        /// </summary>
        public bool ScopeManagementEnabled { get; set; }

        /// <summary>
        /// Indicates that the scope management enabled or not on this registration.
        /// </summary>
        public object ExistingInstance { get; set; }

        /// <summary>
        /// Constructs a <see cref="RegistrationContextData"/>
        /// </summary>
        public RegistrationContextData()
        {
            this.AttributeConditions = new HashSet<Type>();
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
