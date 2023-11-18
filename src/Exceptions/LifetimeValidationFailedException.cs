using System;
using System.Runtime.Serialization;

namespace Stashbox.Exceptions;

/// <summary>
/// Represents the exception the container throws when the lifetime validation is failed.
/// </summary>
[Serializable]
public class LifetimeValidationFailedException : Exception
{
    /// <summary>
    /// The type the container is currently resolving.
    /// </summary>
    public Type? Type { get; }

    /// <summary>
    /// Constructs a <see cref="LifetimeValidationFailedException"/>.
    /// </summary>
    /// <param name="type">The type of the service.</param>
    /// <param name="message">The exception message.</param>
    public LifetimeValidationFailedException(Type? type, string message)
        : base(message)
    {
        this.Type = type;
    }

    /// <inheritdoc />
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#endif
    protected LifetimeValidationFailedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}