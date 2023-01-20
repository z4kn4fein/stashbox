using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Resolution;

/// <summary>
/// Represents a wrapper that can wrap a service.
/// </summary>
public interface IServiceWrapper : IResolver
{
    /// <summary>
    /// Wraps the expression that describes the service.
    /// </summary>
    /// <param name="originalTypeInformation">The requested type's meta information.</param>
    /// <param name="wrappedTypeInformation">The wrapped type's meta information.</param>
    /// <param name="serviceContext">The wrapped service's context that contains the actual instantiation expression and additional meta information.</param>
    /// <returns>The wrapped service expression.</returns>
    Expression WrapExpression(TypeInformation originalTypeInformation,
        TypeInformation wrappedTypeInformation,
        ServiceContext serviceContext);

    /// <summary>
    /// Un-wraps the underlying service type from a wrapped type request.
    /// </summary>
    /// <param name="typeInformation">The requested type's meta information.</param>
    /// <param name="unWrappedType">The un-wrapped service type.</param>
    /// <returns>True if the un-wrapping was successful, otherwise false.</returns>
    bool TryUnWrap(TypeInformation typeInformation, out TypeInformation unWrappedType);
}

/// <summary>
/// Represents a wrapper that can wrap a collection of a service.
/// </summary>
public interface IEnumerableWrapper : IResolver
{
    /// <summary>
    /// Wraps the expression that describes the service.
    /// </summary>
    /// <param name="originalTypeInformation">The requested type's meta information.</param>
    /// <param name="wrappedTypeInformation">The wrapped type's meta information.</param>
    /// <param name="serviceContexts">The service contexts that contains the actual instantiation expressions and additional meta information.</param>
    /// <returns>The wrapped service expression.</returns>
    Expression WrapExpression(TypeInformation originalTypeInformation, TypeInformation wrappedTypeInformation,
        IEnumerable<ServiceContext> serviceContexts);

    /// <summary>
    /// Un-wraps the underlying service type from a wrapped type request.
    /// </summary>
    /// <param name="typeInformation">The requested type's meta information.</param>
    /// <param name="unWrappedType">The un-wrapped service type.</param>
    /// <returns>True if the un-wrapping was successful, otherwise false.</returns>
    bool TryUnWrap(TypeInformation typeInformation, out TypeInformation unWrappedType);
}

/// <summary>
/// Represents a wrapper that can wrap a service with function parameters.
/// </summary>
public interface IParameterizedWrapper : IResolver
{
    /// <summary>
    /// Wraps the expression that describes the service.
    /// </summary>
    /// <param name="originalTypeInformation">The requested type's meta information.</param>
    /// <param name="wrappedTypeInformation">The wrapped type's meta information.</param>
    /// <param name="serviceContext">The wrapped service's context that contains the actual instantiation expression and additional meta information.</param>
    /// <param name="parameterExpressions">The wrapper's parameter expressions.</param>
    /// <returns>The wrapped service expression.</returns>
    Expression WrapExpression(TypeInformation originalTypeInformation, TypeInformation wrappedTypeInformation,
        ServiceContext serviceContext, IEnumerable<ParameterExpression> parameterExpressions);

    /// <summary>
    /// Un-wraps the underlying service type from a wrapped type request.
    /// </summary>
    /// <param name="typeInformation">The requested type's meta information.</param>
    /// <param name="unWrappedType">The un-wrapped service type.</param>
    /// <param name="parameterTypes">The wrapper's parameter types.</param>
    /// <returns>True if the un-wrapping was successful, otherwise false.</returns>
    bool TryUnWrap(TypeInformation typeInformation, out TypeInformation unWrappedType, out IEnumerable<Type> parameterTypes);
}