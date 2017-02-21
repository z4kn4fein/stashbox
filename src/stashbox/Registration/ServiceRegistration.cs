using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
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
        private readonly Type serviceType;
        private readonly IContainerContext containerContext;
        private readonly ILifetime lifetimeManager;
        private readonly IObjectBuilder objectBuilder;
        private readonly HashSet<Type> attributeConditions;
        private readonly Type targetTypeCondition;
        private readonly Func<TypeInformation, bool> resolutionCondition;
        private readonly MetaInfoProvider metaInfoProvider;

        internal ServiceRegistration(string registrationName, Type serviceType, IContainerContext containerContext,
            ILifetime lifetimeManager, IObjectBuilder objectBuilder, MetaInfoProvider metaInfoProvider, HashSet<Type> attributeConditions = null,
            Type targetTypeCondition = null, Func<TypeInformation, bool> resolutionCondition = null)
        {
            this.serviceType = serviceType;
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
        public bool IsUsableForCurrentContext(TypeInformation typeInfo) => (this.targetTypeCondition == null && this.resolutionCondition == null && (this.attributeConditions == null || !this.attributeConditions.Any())) ||
                   (this.targetTypeCondition != null && typeInfo.ParentType != null && this.targetTypeCondition == typeInfo.ParentType) ||
                   (this.attributeConditions != null && typeInfo.CustomAttributes != null &&
                    this.attributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any()) ||
                   (this.resolutionCondition != null && this.resolutionCondition(typeInfo));

        /// <inheritdoc />
        public bool HasCondition => this.targetTypeCondition != null || this.resolutionCondition != null ||
            (this.attributeConditions != null && this.attributeConditions.Any());

        /// <inheritdoc />
        public bool ValidateGenericContraints(TypeInformation typeInformation) => !this.metaInfoProvider.HasGenericTypeConstraints ||
            this.metaInfoProvider.HasGenericTypeConstraints && this.metaInfoProvider.ValidateGenericContraints(typeInformation);
        
        /// <inheritdoc />
        public void CleanUp()
        {
            this.objectBuilder.CleanUp();
            this.lifetimeManager.CleanUp();
        }

        /// <inheritdoc />
        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            var expr = this.lifetimeManager.GetExpression(this.containerContext, this.objectBuilder, resolutionInfo, resolveType);
            if (!this.containerContext.ContainerConfigurator.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                !this.lifetimeManager.IsTransient || this.objectBuilder.HandlesObjectDisposal) return expr;

            var call = Expression.Call(Expression.Constant(this), "AddTransientObjectTracking", null, expr);
            return Expression.Convert(call, resolveType.Type);
        }

        private object AddTransientObjectTracking(object instance)
        {
            if (instance is IDisposable)
                this.containerContext.TrackedTransientObjects.Add(instance);
            return instance;
        }
    }
}
