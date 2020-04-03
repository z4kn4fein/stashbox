using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public interface IServiceRegistration
    {
        /// <summary>
        /// The implementation type.
        /// </summary>
        Type ImplementationType { get; }

        /// <summary>
        /// The <see cref="TypeInfo"/> of the implementation type.
        /// </summary>
        TypeInfo ImplementationTypeInfo { get; }

        /// <summary>
        /// The registration context.
        /// </summary>
        RegistrationContext RegistrationContext { get; }

        /// <summary>
        /// The registration number.
        /// </summary>
        int RegistrationId { get; }

        /// <summary>
        /// The registration id.
        /// </summary>
        object RegistrationName { get; }

        /// <summary>
        /// True if the registration contains any condition, otherwise false.
        /// </summary>
        bool HasCondition { get; }

        /// <summary>
        /// True if the registration is a decorator.
        /// </summary>
        bool IsDecorator { get; }

        /// <summary>
        /// True if the registration is restricted to a named scope.
        /// </summary>
        bool HasScopeName { get; }

        /// <summary>
        /// True if the registration contains a disposable service which should be tracked.
        /// </summary>
        bool ShouldHandleDisposal { get; }

        /// <summary>
        /// True if the registration can used by an unnamed resolution request.
        /// </summary>
        bool IsResolvableByUnnamedRequest { get; }

        /// <summary>
        /// Creates an expression for creating the resolved instance.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="resolutionContext">The info about the current resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The expression.</returns>
        Expression GetExpression(IContainerContext containerContext, ResolutionContext resolutionContext, Type resolveType);

        /// <summary>
        /// Checks whether the registration can be used for a current resolution.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>True if the registration can be used for the current resolution, otherwise false.</returns>
        bool IsUsableForCurrentContext(TypeInformation typeInfo);

        /// <summary>
        /// Checks that the registration can be injected into a named scope.
        /// </summary>
        /// <param name="scopeNames">The scope names.</param>
        /// <returns>True if the registration could be injected into a named scope, otherwise false.</returns>
        bool CanInjectIntoNamedScope(IEnumerable<object> scopeNames);

        /// <summary>
        /// Clones the registration with new underlying type.
        /// </summary>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="objectBuilder">The object builder.</param>
        /// <returns>The new registration.</returns>
        IServiceRegistration Clone(Type implementationType, IObjectBuilder objectBuilder);

        /// <summary>
        /// Indicates that the current registration replaces another one, it copies the necessary information from the original.
        /// </summary>
        /// <param name="serviceRegistration">The registration to copy the id from.</param>
        void Replaces(IServiceRegistration serviceRegistration);
    }
}
