using System;
using System.Runtime.Serialization;

namespace Stashbox.Exceptions;

/// <summary>
/// Represents the exception the container throws when a service resolution is failed.
/// </summary>
[Serializable]
public class ResolutionFailedException : Exception
{
    private const string DefaultMessage = "Service is not registered properly or unresolvable type requested.";
    
    /// <summary>
    /// The type the container is currently resolving.
    /// </summary>
    public Type? Type { get; }

    /// <summary>
    /// Constructs a <see cref="ResolutionFailedException"/>.
    /// </summary>
    /// <param name="type">The type of the service.</param>
    /// <param name="name">The name of the service.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ResolutionFailedException(Type? type,
        object? name = null,
        string? message = null,
        Exception? innerException = null)
        : base(ConstructMessage(type, name, message), innerException)
    {
        this.Type = type;
    }

    /// <inheritdoc />
#if NET8_0_OR_GREATER
    [Obsolete(DiagnosticId = "SYSLIB0051")] // add this attribute to the serialization ctor
#endif
    protected ResolutionFailedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    { }

    private static string ConstructMessage(Type? serviceType,
        object? name = null,
        string? message = null) =>
        $"Unable to resolve type '{serviceType?.FullName}'{(name != null ? " with name \'" + name + "\'" : "")}.{Environment.NewLine}{message ?? DefaultMessage}";
    
    internal static Exception CreateWithDesiredExceptionType(Type? serviceType,
        object? name = null,
        string? message = null,
        Exception? innerException = null,
        Type? externalExceptionType = null)
    {
        if (externalExceptionType == null) 
            return new ResolutionFailedException(serviceType, name, message, innerException);

        return (Exception)(Activator.CreateInstance(externalExceptionType, ConstructMessage(serviceType, name, message), innerException) ?? new ResolutionFailedException(serviceType, name, message, innerException));
    }
}