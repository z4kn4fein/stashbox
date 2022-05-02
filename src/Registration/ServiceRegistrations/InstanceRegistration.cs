namespace Stashbox.Registration.ServiceRegistrations
{
    /// <summary>
    /// Describes an instance registration.
    /// </summary>
    public class InstanceRegistration : ComplexRegistration
    {
        /// <summary>
        /// If true, the existing instance will be wired into the container, it will perform member and method injection on it.
        /// </summary>
        public readonly bool IsWireUp;

        /// <summary>
        /// The already stored instance which was provided by instance or wired up registration.
        /// </summary>
        public readonly object ExistingInstance;

        internal InstanceRegistration(object existingInstance, bool isWireUp, ServiceRegistration baseRegistration)
            : base(existingInstance.GetType(), baseRegistration.Name, baseRegistration)
        {
            this.IsWireUp = isWireUp;
            this.ExistingInstance = existingInstance;
        }
    }
}