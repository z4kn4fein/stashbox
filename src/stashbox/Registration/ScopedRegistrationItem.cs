using System;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents an item of the scoped registrations collection.
    /// </summary>
    public class ScopedRegistrationItem
    {
        /// <summary>
        /// The type that will be requested.		
        /// </summary>		
        public Type TypeFrom { get; }

        /// <summary>		
        /// The type that will be returned.		
        /// </summary>		
        public Type TypeTo { get; }

        /// <summary>
        /// The context data of a service registration.
        /// </summary>
        public RegistrationContextData RegistrationContextData { get; }

        /// <summary>
        /// Constructs a <see cref="ScopedRegistrationItem"/>.
        /// </summary>
        /// <param name="typeFrom">The type that will be requested.</param>
        /// <param name="typeTo">The type that will be returned.</param>
        /// <param name="registrationContextData">The context data of a service registration.</param>
        public ScopedRegistrationItem(Type typeFrom, Type typeTo, RegistrationContextData registrationContextData)
        {
            this.TypeFrom = typeFrom;
            this.TypeTo = typeTo;
            this.RegistrationContextData = registrationContextData;
        }
    }
}
