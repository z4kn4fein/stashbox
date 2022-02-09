using Stashbox.Utils;
using System;
using System.Runtime.Serialization;

namespace Stashbox.Exceptions
{
    /// <summary>
    /// Represents the exception the container throws when a circular dependency is found.
    /// </summary>
    [Serializable]
    public class CircularDependencyException : Exception
    {
        /// <summary>
        /// The type the container is currently resolving.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Constructs a <see cref="CircularDependencyException"/>.
        /// </summary>
        /// <param name="type">The type of the service type.</param>
        /// <param name="innerException">The inner exception.</param>
        public CircularDependencyException(Type type, Exception innerException = null)
            : base($"Circular dependency detected during the resolution of {type.FullName}.", innerException)
        {
            this.Type = type;
        }

        /// <inheritdoc />
        protected CircularDependencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Type = (Type)info.GetValue("Type", typeof(Type));
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Shield.EnsureNotNull(info, "info");

            info.AddValue("Type", this.Type, typeof(Type));
            base.GetObjectData(info, context);
        }
    }
}
