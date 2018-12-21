using Stashbox.BuildUp;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Lifetime;
using Stashbox.Resolution;
using Stashbox.Utils;
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
        internal static int GlobalRegistrationNumber;
        internal static AvlTreeKeyValue<Type, MetaInformation> MetaRepository = AvlTreeKeyValue<Type, MetaInformation>.Empty;
        private readonly IContainerConfigurator containerConfigurator;
        private readonly IObjectBuilder objectBuilder;
        private readonly MetaInformation metaInformation;

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

        /// <inheritdoc />
        public object RegistrationId { get; }

        /// <inheritdoc />
        public bool HasName { get; }

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

        internal ServiceRegistration(Type serviceType, Type implementationType, IContainerConfigurator containerConfigurator,
             IObjectBuilder objectBuilder, RegistrationContextData registrationContextData,
             bool isDecorator, bool shouldHandleDisposal)
        {
            this.objectBuilder = objectBuilder;
            this.containerConfigurator = containerConfigurator;
            this.ImplementationType = implementationType;
            this.ServiceType = serviceType;
            this.metaInformation = GetOrCreateMetaInfo(implementationType);
            this.Constructors = this.metaInformation.Constructors;
            this.InjectionMethods = this.metaInformation.InjectionMethods;
            this.InjectionMembers = this.ConstructInjectionMembers(registrationContextData, this.metaInformation);
            this.SelectedConstructor = this.FindSelectedConstructor(registrationContextData, this.metaInformation);
            this.RegistrationNumber = ReserveRegistrationNumber();
            this.RegistrationContext = registrationContextData;
            this.IsDecorator = isDecorator;
            this.ShouldHandleDisposal = shouldHandleDisposal;

            this.HasName = this.RegistrationContext.Name != null;

            this.HasScopeName = this.RegistrationContext.Lifetime is NamedScopeLifetime;

            this.HasCondition = this.RegistrationContext.TargetTypeCondition != null || this.RegistrationContext.ResolutionCondition != null ||
                this.RegistrationContext.AttributeConditions != null && this.RegistrationContext.AttributeConditions.Count > 0;

            this.RegistrationId = this.RegistrationContext.Name ??
                (containerConfigurator.ContainerConfiguration.SetUniqueRegistrationNames
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
        public bool ValidateGenericContraints(Type type) =>
            this.metaInformation.ValidateGenericContraints(type);

        /// <inheritdoc />
        public Expression GetExpression(IContainerContext containerContext, ResolutionContext resolutionContext, Type resolveType)
        {
            if (this.IsDecorator || this.metaInformation.IsOpenGenericType) return this.ConstructExpression(containerContext, resolutionContext, resolveType);

            var expression = resolutionContext.GetCachedExpression(this.RegistrationNumber);
            if (expression != null) return expression;

            expression = this.ConstructExpression(containerContext, resolutionContext, resolveType);
            resolutionContext.CacheExpression(this.RegistrationNumber, expression);
            return expression;
        }

        /// <inheritdoc />
        public bool CanInjectMember(MemberInformation member)
        {
            var autoMemberInjectionEnabled = this.containerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationEnabled || this.RegistrationContext.AutoMemberInjectionEnabled;
            var autoMemberInjectionRule = this.RegistrationContext.AutoMemberInjectionEnabled ? this.RegistrationContext.AutoMemberInjectionRule :
                this.containerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationRule;

            if (autoMemberInjectionEnabled)
                return member.TypeInformation.ForcedDependency ||
                       member.TypeInformation.MemberType == MemberType.Field &&
                           (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PrivateFields) == Rules.AutoMemberInjectionRules.PrivateFields ||
                       member.TypeInformation.MemberType == MemberType.Property &&
                           ((autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter) == Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter &&
                           ((PropertyInfo)member.MemberInfo).HasSetMethod() ||
                           (autoMemberInjectionRule & Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess) == Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess);

            return member.TypeInformation.ForcedDependency;
        }

        /// <inheritdoc />
        public bool CanInjectIntoNamedScope(ISet<object> scopeNames) => scopeNames.Contains(((NamedScopeLifetime)this.RegistrationContext.Lifetime).ScopeName);

        private ConstructorInformation FindSelectedConstructor(RegistrationContextData registrationContextData, MetaInformation metaInfo)
        {
            if (registrationContextData.SelectedConstructor == null)
                return null;

            var length = metaInfo.Constructors.Length;
            for (var i = 0; i < length; i++)
            {
                var current = metaInfo.Constructors[i];
                if (current.Constructor == registrationContextData.SelectedConstructor)
                    return current;
            }

            return null;
        }

        private MemberInformation[] ConstructInjectionMembers(RegistrationContextData registrationContextData, MetaInformation metaInfo)
        {
            if (registrationContextData.InjectionMemberNames.Count == 0)
                return metaInfo.InjectionMembers;

            var length = metaInfo.InjectionMembers.Length;
            var members = new MemberInformation[length];
            for (var i = 0; i < length; i++)
            {
                var member = metaInfo.InjectionMembers[i];
                if (registrationContextData.InjectionMemberNames.TryGetValue(member.MemberInfo.Name,
                    out var dependencyName))
                {
                    var copy = member.Clone();
                    copy.TypeInformation.ForcedDependency = true;
                    copy.TypeInformation.DependencyName = dependencyName;
                    members[i] = copy;
                }
                else
                    members[i] = member;
            }

            return members;
        }

        private Expression ConstructExpression(IContainerContext containerContext, ResolutionContext resolutionContext, Type resolveType) =>
            this.RegistrationContext.Lifetime == null || this.ServiceType.IsOpenGenericType()
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

        private static MetaInformation GetOrCreateMetaInfo(Type typeTo)
        {
            var found = MetaRepository.GetOrDefault(typeTo);
            if (found != null) return found;

            var meta = new MetaInformation(typeTo);
            Swap.SwapValue(ref MetaRepository, repo => repo.AddOrUpdate(typeTo, meta));
            return meta;
        }

        private static int ReserveRegistrationNumber() =>
            Interlocked.Increment(ref GlobalRegistrationNumber);
    }
}
