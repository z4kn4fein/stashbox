using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.MetaInfo;
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
        private readonly IContainerContext containerContext;
        private readonly ILifetime lifetimeManager;
        private readonly IObjectBuilder objectBuilder;
        private readonly HashSet<Type> attributeConditions;
        private readonly Type targetTypeCondition;
        private readonly Func<TypeInformation, bool> resolutionCondition;
        private readonly MetaInfoProvider metaInfoProvider;

        internal ServiceRegistration(string registrationName, IContainerContext containerContext, ILifetime lifetimeManager,
            IObjectBuilder objectBuilder, MetaInfoProvider metaInfoProvider, HashSet<Type> attributeConditions = null, Type targetTypeCondition = null,
            Func<TypeInformation, bool> resolutionCondition = null)
        {
            this.RegistrationName = registrationName;
            this.containerContext = containerContext;
            this.lifetimeManager = lifetimeManager;
            this.objectBuilder = objectBuilder;
            this.attributeConditions = attributeConditions;
            this.targetTypeCondition = targetTypeCondition;
            this.resolutionCondition = resolutionCondition;
            this.metaInfoProvider = metaInfoProvider;
            this.RegistrationNumber = containerContext.ReserveRegistrationNumber();
        }

        /// <inheritdoc />
        public int RegistrationNumber { get; }

        /// <inheritdoc />
        public string RegistrationName { get; }

        /// <inheritdoc />
        public object GetInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType) =>
            this.lifetimeManager.GetInstance(this.containerContext, this.objectBuilder, resolutionInfo, resolveType);

        /// <inheritdoc />
        public bool IsUsableForCurrentContext(TypeInformation typeInfo) => (this.targetTypeCondition == null && this.resolutionCondition == null && (this.attributeConditions == null || !this.attributeConditions.Any())) ||
                   (this.targetTypeCondition != null && typeInfo.ParentType != null && this.targetTypeCondition == typeInfo.ParentType) ||
                   (this.attributeConditions != null && typeInfo.CustomAttributes != null &&
                    this.attributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any()) ||
                   (this.resolutionCondition != null && this.resolutionCondition(typeInfo));

        /// <inheritdoc />
        public bool HasCondition => this.targetTypeCondition != null || this.resolutionCondition != null ||
            (this.attributeConditions != null && this.attributeConditions.Any());

        /// <inheritdoc />
        public bool ValidateGenericContraints(TypeInformation typeInformation) =>
            this.metaInfoProvider.HasGenericTypeConstraints && this.metaInfoProvider.ValidateGenericContraints(typeInformation);

        /// <inheritdoc />
        public void ServiceUpdated(RegistrationInfo registrationInfo) =>
            this.objectBuilder.ServiceUpdated(registrationInfo);

        /// <inheritdoc />
        public void CleanUp()
        {
            this.objectBuilder.CleanUp();
            this.lifetimeManager.CleanUp();
        }

        /// <inheritdoc />
        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType) =>
            this.lifetimeManager.GetExpression(this.containerContext, this.objectBuilder, resolutionInfo, resolveType);
    }
}
