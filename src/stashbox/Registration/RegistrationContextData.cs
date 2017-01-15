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
        /// Represents the type of the service the implementation bound to.
        /// </summary>
        public Type TypeFrom { get; set; }

        /// <summary>
        /// The type of the actual service implementation.
        /// </summary>
        public Type TypeTo { get; set; }

        /// <summary>
        /// Name of the registration.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Parameterless factory of the registration.
        /// </summary>
        public Func<object> SingleFactory { get; set; }

        /// <summary>
        /// One parameter factory of the registration.
        /// </summary>
        public Func<object, object> OneParameterFactory { get; set; }

        /// <summary>
        /// Two parameters factory of the registration.
        /// </summary>
        public Func<object, object, object> TwoParametersFactory { get; set; }

        /// <summary>
        /// Three parameters factory of the registration.
        /// </summary>
        public Func<object, object, object, object> ThreeParametersFactory { get; set; }

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
        /// Constructs a <see cref="RegistrationContextData"/>
        /// </summary>
        public RegistrationContextData()
        {
            this.AttributeConditions = new HashSet<Type>();
        }
    }
}
