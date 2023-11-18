using System;
using System.Runtime.Serialization;

namespace Stashbox.Exceptions;

/// <summary>
/// Represents an exception the container throws when it detects an invalid registration.
/// </summary>
[Serializable]
public class InvalidRegistrationException : Exception
{
    /// <summary>
    /// The type the container is trying to register.
    /// </summary>
    public Type? Type { get; }

    /// <summary>
    /// Constructs a <see cref="InvalidRegistrationException"/>.
    /// </summary>
    /// <param name="type">The type of the service.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public InvalidRegistrationException(Type? type, string message, Exception? innerException = null)
        : base($"Invalid registration with type '{type?.FullName}'. Details: {message}", innerException)
    {
        this.Type = type;
    }

    /// <inheritdoc />
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#endif
    protected InvalidRegistrationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}