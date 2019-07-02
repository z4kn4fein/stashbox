using Stashbox.BuildUp;
using Stashbox.Entity;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        /// The registration context.
        /// </summary>
        RegistrationContextData RegistrationContext { get; }

        /// <summary>
        /// The registration number.
        /// </summary>
        int RegistrationNumber { get; }

        /// <summary>
        /// The registration id.
        /// </summary>
        object RegistrationId { get; }

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
        /// True if the registration has a name set.
        /// </summary>
        bool HasName { get; }

        /// <summary>
        /// Holds the injection member of the service.
        /// </summary>
        MemberInformation[] InjectionMembers { get; }

        /// <summary>
        /// Holds the constructors of the service.
        /// </summary>
        ConstructorInformation[] Constructors { get; }

        /// <summary>
        /// Holds the injection methods of the service.
        /// </summary>
        MethodInformation[] InjectionMethods { get; }

        /// <summary>
        /// Holds the information about the constructor, selected by the user at configuration time.
        /// </summary>
        ConstructorInformation SelectedConstructor { get; }

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
        /// Validates that the given type's generic argument fulfills the generic constraint or not 
        /// </summary>
        /// <param name="type">The type information.</param>
        /// <returns>True if the argument is valid.</returns>
        bool ValidateGenericConstraints(Type type);

        /// <summary>
        /// Checks that the registration can be injected into a named scope.
        /// </summary>
        /// <param name="scopeNames">The scope names.</param>
        /// <returns>True if the registration could be injected into a named scope, otherwise false.</returns>
        bool CanInjectIntoNamedScope(ISet<object> scopeNames);

        /// <summary>
        /// Clones the registration with new underlying type.
        /// </summary>
        /// <param name="implementationType">The implementation type.</param>
        /// <param name="objectBuilder">The object builder.</param>
        /// <returns>The new registration.</returns>
        IServiceRegistration Clone(Type implementationType, IObjectBuilder objectBuilder);
    }
}
