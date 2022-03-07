using System;
using Stashbox.Configuration;

namespace Stashbox.Registration
{
    /// <summary>
    /// Describes an instance registration.
    /// </summary>
    public class InstanceRegistration : ServiceRegistration
    {
        /// <summary>
        /// If true, the existing instance will be wired into the container, it will perform member and method injection on it.
        /// </summary>
        public readonly bool IsWireUp;
        
        /// <summary>
        /// The already stored instance which was provided by instance or wired up registration.
        /// </summary>
        public readonly object ExistingInstance;
        
        internal InstanceRegistration(Type implementationType, RegistrationContext registrationContext, 
            ContainerConfiguration containerConfiguration, bool isDecorator, object existingInstance) 
            : base(implementationType, registrationContext, containerConfiguration, isDecorator)
        {
            this.IsWireUp = registrationContext.IsWireUp;
            this.ExistingInstance = existingInstance;
        }
    }
}