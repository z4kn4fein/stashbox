using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.MetaInfo;
using Stashbox.Utils;
using System;
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
        private static int GlobalRegistrationNumber;
        private static readonly ConcurrentTree<Type, MetaInformation> MetaRepository = new ConcurrentTree<Type, MetaInformation>();
        private readonly IContainerConfigurator containerConfigurator;
        private readonly IObjectBuilder objectBuilder;

        /// <inheritdoc />
        public MetaInformation MetaInformation { get; }

        /// <inheritdoc />
        public Type ServiceType { get; }

        /// <inheritdoc />
        public Type ImplementationType { get; }

        /// <inheritdoc />
        public RegistrationContextData RegistrationContext { get; }

        /// <inheritdoc />
        public bool IsDecorator { get; }

        /// <inheritdoc />
        public bool ShouldHandleDisposal { get; }

        /// <inheritdoc />
        public int RegistrationNumber { get; }

        internal ServiceRegistration(Type serviceType, Type implementationType, IContainerConfigurator containerConfigurator,
             IObjectBuilder objectBuilder, RegistrationContextData registrationContextData,
             bool isDecorator, bool shouldHandleDisposal)
        {
            this.objectBuilder = objectBuilder;
            this.containerConfigurator = containerConfigurator;
            this.ImplementationType = implementationType;
            this.ServiceType = serviceType;
            this.MetaInformation = GetOrCreateMetaInfo(implementationType);
            this.RegistrationNumber = ReserveRegistrationNumber();
            this.RegistrationContext = registrationContextData;
            this.IsDecorator = isDecorator;
            this.ShouldHandleDisposal = shouldHandleDisposal;

            if (this.RegistrationContext.Name == null)
                this.RegistrationContext.Name = containerConfigurator.ContainerConfiguration.SetUniqueRegistrationNames ? (object)this.RegistrationNumber : implementationType;
        }

        /// <inheritdoc />
        public bool IsUsableForCurrentContext(TypeInformation typeInfo) =>
            !this.HasCondition ||
            this.HasParentTypeConditionAndMatch(typeInfo) ||
            this.HasAttributeConditionAndMatch(typeInfo) ||
            this.HasResolutionConditionAndMatch(typeInfo);

        /// <inheritdoc />
        public bool HasCondition => this.RegistrationContext.TargetTypeCondition != null || this.RegistrationContext.ResolutionCondition != null ||
            this.RegistrationContext.AttributeConditions != null && this.RegistrationContext.AttributeConditions.Count > 0;

        /// <inheritdoc />
        public bool ValidateGenericContraints(Type type) =>
            this.MetaInformation.GenericTypeConstraints.Count == 0 || this.MetaInformation.ValidateGenericContraints(type);

        /// <inheritdoc />
        public Expression GetExpression(IContainerContext containerContext, ResolutionInfo resolutionInfo, Type resolveType) =>
            this.RegistrationContext.Lifetime == null || this.ServiceType.IsOpenGenericType() ?
                this.objectBuilder.GetExpression(containerContext, this, this.RegistrationContext.DefinedScopeName == null ? resolutionInfo : resolutionInfo.CreateNew(scopeName: this.RegistrationContext.DefinedScopeName), resolveType) :
                this.RegistrationContext.Lifetime.GetExpression(containerContext, this, this.objectBuilder, this.RegistrationContext.DefinedScopeName == null ? resolutionInfo : resolutionInfo.CreateNew(scopeName: this.RegistrationContext.DefinedScopeName), resolveType);

        /// <inheritdoc />
        public bool CanInjectMember(MemberInformation member)
        {
            var autoMemberInjectionEnabled = this.containerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationEnabled || this.RegistrationContext.AutoMemberInjectionEnabled;
            var autoMemberInjectionRule = this.RegistrationContext.AutoMemberInjectionEnabled ? this.RegistrationContext.AutoMemberInjectionRule :
                this.containerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationRule;

            if (autoMemberInjectionEnabled)
                return member.TypeInformation.ForcedDependency ||
                       member.MemberInfo is FieldInfo && autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjectionRules.PrivateFields) ||
                       member.MemberInfo is PropertyInfo && (autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter) && ((PropertyInfo)member.MemberInfo).HasSetMethod() ||
                       autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess));

            return member.TypeInformation.ForcedDependency;
        }

        private bool HasParentTypeConditionAndMatch(TypeInformation typeInfo) =>
            this.RegistrationContext.TargetTypeCondition != null && typeInfo.ParentType != null && this.RegistrationContext.TargetTypeCondition == typeInfo.ParentType;

        private bool HasAttributeConditionAndMatch(TypeInformation typeInfo) =>
            this.RegistrationContext.AttributeConditions != null && typeInfo.CustomAttributes != null &&
            this.RegistrationContext.AttributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any();

        private bool HasResolutionConditionAndMatch(TypeInformation typeInfo) =>
            this.RegistrationContext.ResolutionCondition != null && this.RegistrationContext.ResolutionCondition(typeInfo);

        private static MetaInformation GetOrCreateMetaInfo(Type typeTo)
        {
            var found = MetaRepository.GetOrDefault(typeTo);
            if (found != null) return found;

            var meta = new MetaInformation(typeTo);
            MetaRepository.AddOrUpdate(typeTo, meta);
            return meta;
        }

        private static int ReserveRegistrationNumber() =>
            Interlocked.Increment(ref GlobalRegistrationNumber);
    }
}
