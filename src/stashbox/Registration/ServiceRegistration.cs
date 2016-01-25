using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration : IServiceRegistration
    {
        private readonly ILifetime lifetimeManager;
        private readonly IObjectBuilder objectBuilder;
        private readonly HashSet<Type> attributeConditions;
        private readonly Type targetTypeCondition;
        private readonly Func<TypeInformation, bool> resolutionCondition;

        /// <summary>
        /// Constructs a <see cref="ServiceRegistration"/>
        /// </summary>
        /// <param name="lifetimeManager">The lifetime manager.</param>
        /// <param name="objectBuilder">THe object builder.</param>
        /// <param name="attributeConditions">The attribute conditions.</param>
        /// <param name="targetTypeCondition">The target type condition.</param>
        /// <param name="resolutionCondition">The resolution condition.</param>
        public ServiceRegistration(ILifetime lifetimeManager, IObjectBuilder objectBuilder, HashSet<Type> attributeConditions = null,
            Type targetTypeCondition = null, Func<TypeInformation, bool> resolutionCondition = null)
        {
            this.lifetimeManager = lifetimeManager;
            this.objectBuilder = objectBuilder;
            this.attributeConditions = attributeConditions;
            this.targetTypeCondition = targetTypeCondition;
            this.resolutionCondition = resolutionCondition;
        }

        /// <summary>
        /// Gets the resolved instance.
        /// </summary>
        /// <param name="resolutionInfo">The info about the current resolution.</param>
        /// <param name="resolveType">The resolve type.</param>
        /// <returns>The created object.</returns>
        public object GetInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return this.lifetimeManager.GetInstance(this.objectBuilder, resolutionInfo, resolveType);
        }

        /// <summary>
        /// Checks whether the registration can be used for a current resolution.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>True if the registration can be used for the current resolution, otherwise false.</returns>
        public bool IsUsableForCurrentContext(TypeInformation typeInfo)
        {
            return (this.targetTypeCondition == null && this.resolutionCondition == null && (this.attributeConditions == null || !this.attributeConditions.Any())) ||
                   (this.targetTypeCondition != null && typeInfo.ParentType != null && this.targetTypeCondition == typeInfo.ParentType) ||
                   (this.attributeConditions != null && typeInfo.CustomAttributes != null &&
                    this.attributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any()) ||
                   (this.resolutionCondition != null && this.resolutionCondition(typeInfo));
        }

        /// <summary>
        /// True if the registration contains any condition, otherwise false.
        /// </summary>
        public bool HasCondition => this.targetTypeCondition != null || this.resolutionCondition != null ||
            (this.attributeConditions != null && this.attributeConditions.Any());

        /// <summary>
        /// Indicates a service updated event when a ReMap occures on a tracked service.
        /// </summary>
        /// <param name="registrationInfo">The new service registration.</param>
        public void ServiceUpdated(RegistrationInfo registrationInfo)
        {
            this.objectBuilder.ServiceUpdated(registrationInfo);
        }

        /// <summary>
        /// Cleans up the registration.
        /// </summary>
        public void CleanUp()
        {
            this.objectBuilder.CleanUp();
            this.lifetimeManager.CleanUp();
        }

        /// <summary>
        /// Creates an expression for creating the resolved instance.
        /// </summary>
        /// <param name="resolutionInfo">The info about the current resolution.</param>
        /// <param name="resolutionInfoExpression">The expression of the info about the current resolution.</param>
        /// <param name="resolveType">The resolve type.</param>
        /// <returns>The expression.</returns>
        public Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            return this.lifetimeManager.GetExpression(this.objectBuilder, resolutionInfo, resolutionInfoExpression, resolveType);
        }
    }
}
