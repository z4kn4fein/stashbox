using System.Linq.Expressions;
using Stashbox.Entity;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public interface IServiceRegistration
    {
        /// <summary>
        /// The service type.
        /// </summary>
        Type ServiceType { get;  }

        /// <summary>
        /// The implementation type.
        /// </summary>
        Type ImplementationType { get;  }

        /// <summary>
        /// The lifetime manager.
        /// </summary>
        ILifetime LifetimeManager { get;  }

        /// <summary>
        /// The object builder.
        /// </summary>
        IObjectBuilder ObjectBuilder { get;  }

        /// <summary>
        /// The attribute conditions.
        /// </summary>
        HashSet<Type> AttributeConditions { get;  }

        /// <summary>
        /// The target type condition.
        /// </summary>
        Type TargetTypeCondition { get;  }

        /// <summary>
        /// The resolution condition.
        /// </summary>
        Func<TypeInformation, bool> ResolutionCondition { get; }

        /// <summary>
        /// The registration number.
        /// </summary>
        int RegistrationNumber { get; }
        
        /// <summary>
        /// Creates an expression for creating the resolved instance.
        /// </summary>
        /// <param name="resolutionInfo">The info about the current resolution.</param>
        /// <param name="resolveType">The resolve type.</param>
        /// <returns>The expression.</returns>
        Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType);
        
        /// <summary>
        /// Checks whether the registration can be used for a current resolution.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>True if the registration can be used for the current resolution, otherwise false.</returns>
        bool IsUsableForCurrentContext(TypeInformation typeInfo);

        /// <summary>
        /// True if the registration contains any condition, otherwise false.
        /// </summary>
        bool HasCondition { get; }

        /// <summary>
        /// Validates that the given type's generic argument fullfills the generic constraint or not 
        /// </summary>
        /// <param name="typeInformation">The type information.</param>
        /// <returns>True if the argument is valid.</returns>
        bool ValidateGenericContraints(TypeInformation typeInformation);
        
        /// <summary>
        /// Cleans up the registration.
        /// </summary>
        void CleanUp();
    }
}
