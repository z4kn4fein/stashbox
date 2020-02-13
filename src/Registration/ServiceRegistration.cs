using Stashbox.BuildUp;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration : IServiceRegistration
    {
        private static int GlobalRegistrationNumber;
        private readonly ContainerConfiguration containerConfiguration;
        private readonly IObjectBuilder objectBuilder;
        private readonly MetaInformation metaInformation;

        /// <inheritdoc />
        public Type ImplementationType { get; }

        /// <inheritdoc />
        public RegistrationContext RegistrationContext { get; }

        /// <inheritdoc />
        public bool IsDecorator { get; }

        /// <inheritdoc />
        public bool ShouldHandleDisposal { get; }

        /// <inheritdoc />
        public int RegistrationNumber { get; }

        /// <inheritdoc />
        public object RegistrationId { get; }

        /// <inheritdoc />
        public bool HasName { get; }

        /// <inheritdoc />
        public bool IsResolvableByUnnamedRequest { get; }

        /// <inheritdoc />
        public MemberInformation[] InjectionMembers { get; }

        /// <inheritdoc />
        public ConstructorInformation[] Constructors { get; }

        /// <inheritdoc />
        public ConstructorInformation SelectedConstructor { get; }

        /// <inheritdoc />
        public MethodInformation[] InjectionMethods { get; }

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
            this.metaInformation = MetaInformation.GetOrCreateMetaInfo(implementationType);
            this.Constructors = this.metaInformation.GetConstructors(registrationContext, containerConfiguration);
            this.InjectionMethods = this.metaInformation.GetInjectionMethods(registrationContext, containerConfiguration);
            this.InjectionMembers = this.metaInformation.SelectInjectionMembers(registrationContext,
                containerConfiguration);
            this.SelectedConstructor = this.metaInformation.FindSelectedConstructor(registrationContext);
            this.RegistrationNumber = ReserveRegistrationNumber();
            this.RegistrationContext = registrationContext;
            this.IsDecorator = isDecorator;
            this.ShouldHandleDisposal = shouldHandleDisposal;

            this.HasName = this.RegistrationContext.Name != null;
            this.IsResolvableByUnnamedRequest = !this.HasName || containerConfiguration.NamedDependencyResolutionForUnNamedRequestsEnabled;

            this.HasScopeName = this.RegistrationContext.Lifetime is NamedScopeLifetime;

            this.HasCondition = this.RegistrationContext.TargetTypeCondition != null || this.RegistrationContext.ResolutionCondition != null ||
                this.RegistrationContext.AttributeConditions != null && this.RegistrationContext.AttributeConditions.Any();

            this.RegistrationId = this.RegistrationContext.Name ??
                (containerConfiguration.SetUniqueRegistrationNames
                ? (object)this.RegistrationNumber
                : implementationType);
        }

        /// <inheritdoc />
        public bool IsUsableForCurrentContext(TypeInformation typeInfo) =>
            !this.HasCondition ||
            this.HasParentTypeConditionAndMatch(typeInfo) ||
            this.HasAttributeConditionAndMatch(typeInfo) ||
            this.HasResolutionConditionAndMatch(typeInfo);

        /// <inheritdoc />
        public bool ValidateGenericConstraints(Type type) =>
            this.metaInformation.ValidateGenericContraints(type);

        /// <inheritdoc />
        public Expression GetExpression(IContainerContext containerContext, ResolutionContext resolutionContext, Type resolveType)
        {
            if (this.IsDecorator || this.metaInformation.IsOpenGenericType) return this.ConstructExpression(containerContext, resolutionContext, resolveType);

            var expression = resolutionContext.GetCachedExpression(this.RegistrationNumber);
            if (expression != null)
                return expression;

            if (this.RegistrationContext.FactoryCacheDisabled)
                resolutionContext.ShouldCacheFactoryDelegate = false;

            expression = this.ConstructExpression(containerContext, resolutionContext, resolveType);
            resolutionContext.CacheExpression(this.RegistrationNumber, expression);
            return expression;
        }

        /// <inheritdoc />
        public bool CanInjectIntoNamedScope(ISet<object> scopeNames) => scopeNames.Last() == ((NamedScopeLifetime)this.RegistrationContext.Lifetime).ScopeName;

        /// <inheritdoc />
        public IServiceRegistration Clone(Type implementationType, IObjectBuilder objectBuilder) =>
            new ServiceRegistration(implementationType, this.containerConfiguration, objectBuilder,
                this.RegistrationContext.Clone(), this.IsDecorator, this.ShouldHandleDisposal);

        private Expression ConstructExpression(IContainerContext containerContext, ResolutionContext resolutionContext, Type resolveType) =>
            this.RegistrationContext.Lifetime == null || this.metaInformation.IsOpenGenericType
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

        private static int ReserveRegistrationNumber() =>
            Interlocked.Increment(ref GlobalRegistrationNumber);
    }
}
