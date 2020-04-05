using Stashbox.BuildUp;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration : IServiceRegistration
    {
        private static int globalRegistrationOrder;
        private readonly ContainerConfiguration containerConfiguration;
        private readonly IObjectBuilder objectBuilder;
        private readonly bool isOpenGenericType;

        /// <inheritdoc />
        public Type ImplementationType { get; }

        /// <inheritdoc />
        public TypeInfo ImplementationTypeInfo { get; }

        /// <inheritdoc />
        public RegistrationContext RegistrationContext { get; }

        /// <inheritdoc />
        public bool IsDecorator { get; }

        /// <inheritdoc />
        public bool ShouldHandleDisposal { get; }

        /// <inheritdoc />
        public int RegistrationId { get; private set; }

        /// <inheritdoc />
        public object RegistrationName { get; }

        /// <inheritdoc />
        public bool IsResolvableByUnnamedRequest { get; }

        /// <inheritdoc />
        public bool HasScopeName { get; }

        /// <inheritdoc />
        public bool HasCondition { get; }

        internal ServiceRegistration(Type implementationType, ContainerConfiguration containerConfiguration,
             IObjectBuilder objectBuilder, RegistrationContext registrationContext,
             bool isDecorator, bool shouldHandleDisposal)
        {
            this.containerConfiguration = containerConfiguration;
            this.objectBuilder = objectBuilder;
            this.ImplementationType = implementationType;
            this.ImplementationTypeInfo = implementationType.GetTypeInfo();
            this.isOpenGenericType = this.ImplementationTypeInfo.IsOpenGenericType();
            this.RegistrationContext = registrationContext;
            this.IsDecorator = isDecorator;
            this.ShouldHandleDisposal = shouldHandleDisposal;

            this.IsResolvableByUnnamedRequest = this.RegistrationContext.Name == null || containerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled;

            this.HasScopeName = this.RegistrationContext.Lifetime is NamedScopeLifetime;

            this.HasCondition = this.RegistrationContext.TargetTypeCondition != null || this.RegistrationContext.ResolutionCondition != null ||
                this.RegistrationContext.AttributeConditions != null && this.RegistrationContext.AttributeConditions.Any();

            this.RegistrationId = ReserveRegistrationOrder();
            this.RegistrationName = this.RegistrationContext.Name ??
                (containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.PreserveDuplications
                ? (object)this.RegistrationId
                : implementationType);
        }

        /// <inheritdoc />
        public bool IsUsableForCurrentContext(TypeInformation typeInfo) =>
            this.HasParentTypeConditionAndMatch(typeInfo) ||
            this.HasAttributeConditionAndMatch(typeInfo) ||
            this.HasResolutionConditionAndMatch(typeInfo);

        /// <inheritdoc />
        public bool CanInjectIntoNamedScope(IEnumerable<object> scopeNames) => scopeNames.Last() == ((NamedScopeLifetime)this.RegistrationContext.Lifetime).ScopeName;

        /// <inheritdoc />
        public Expression GetExpression(IContainerContext containerContext, ResolutionContext resolutionContext, Type resolveType)
        {
            if (this.IsDecorator || this.isOpenGenericType) return this.ConstructExpression(containerContext, resolutionContext, resolveType);

            var expression = resolutionContext.GetCachedExpression(this.RegistrationId);
            if (expression != null)
                return expression;

            if (this.RegistrationContext.FactoryCacheDisabled)
                resolutionContext.ShouldCacheFactoryDelegate = false;

            expression = this.ConstructExpression(containerContext, resolutionContext, resolveType);
            resolutionContext.CacheExpression(this.RegistrationId, expression);
            return expression;
        }

        /// <inheritdoc />
        public IServiceRegistration Clone(Type implementationType, IObjectBuilder objectBuilder) =>
            new ServiceRegistration(implementationType, this.containerConfiguration, objectBuilder,
                this.RegistrationContext.Clone(), this.IsDecorator, this.ShouldHandleDisposal);

        /// <inheritdoc />
        public void Replaces(IServiceRegistration serviceRegistration)
        {
            if (this.containerConfiguration.RegistrationBehavior == Rules.RegistrationBehavior.ThrowException)
                throw new ServiceAlreadyRegisteredException(this.ImplementationType);

            this.RegistrationId = serviceRegistration.RegistrationId;
        }

        private Expression ConstructExpression(IContainerContext containerContext, ResolutionContext resolutionContext, Type resolveType) =>
            this.RegistrationContext.Lifetime == null || this.isOpenGenericType
                ? this.objectBuilder.GetExpression(containerContext, this, resolutionContext, resolveType)
                : this.RegistrationContext.Lifetime.GetExpression(containerContext, this, this.objectBuilder,
                    resolutionContext, resolveType);

        private bool HasParentTypeConditionAndMatch(TypeInformation typeInfo) =>
            this.RegistrationContext.TargetTypeCondition != null && typeInfo.ParentType != null && this.RegistrationContext.TargetTypeCondition == typeInfo.ParentType;

        private bool HasAttributeConditionAndMatch(TypeInformation typeInfo) =>
            this.RegistrationContext.AttributeConditions != null && typeInfo.CustomAttributes != null &&
            this.RegistrationContext.AttributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any();

        private bool HasResolutionConditionAndMatch(TypeInformation typeInfo) =>
            this.RegistrationContext.ResolutionCondition != null && this.RegistrationContext.ResolutionCondition(typeInfo);

        private static int ReserveRegistrationOrder() =>
            Interlocked.Increment(ref globalRegistrationOrder);
    }
}
