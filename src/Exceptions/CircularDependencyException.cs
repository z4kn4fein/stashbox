using System;
using System.Runtime.Serialization;

namespace Stashbox.Exceptions;

/// <summary>
/// Represents the exception the container throws when a circular dependency is found.
/// </summary>
[Serializable]
public class CircularDependencyException : Exception
{
    /// <summary>
    /// The type the container is currently resolving.
    /// </summary>
    public Type? Type { get; }

    /// <summary>
    /// Constructs a <see cref="CircularDependencyException"/>.
    /// </summary>
    /// <param name="type">The type of the service type.</param>
    /// <param name="innerException">The inner exception.</param>
    public CircularDependencyException(Type? type, Exception? innerException = null)
        : base($"Circular dependency detected during the resolution of '{type?.FullName}'.", innerException)
    {
        this.Type = type;
    }

    /// <inheritdoc />
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#endif
    protected CircularDependencyException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }
}