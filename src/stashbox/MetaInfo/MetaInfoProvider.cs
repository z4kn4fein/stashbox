using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using Stashbox.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stashbox.Utils;

namespace Stashbox.MetaInfo
{
    internal class MetaInfoProvider : IMetaInfoProvider
    {
        private static readonly ConcurrentTree<Type, MetaInformation> MetaRepository = new ConcurrentTree<Type, MetaInformation>();

        private readonly IContainerContext containerContext;
        private readonly RegistrationContextData registrationData;
        private readonly InjectionParameter[] injectionParameters;

        public bool HasGenericTypeConstraints => this.metaInformation.GenericTypeConstraints.Count > 0;

        private readonly MetaInformation metaInformation;

        public MetaInfoProvider(IContainerContext containerContext, RegistrationContextData registrationData, Type typeTo, InjectionParameter[] injectionParameters = null)
        {
            this.metaInformation = GetOrCreateMetaInfo(typeTo);
            this.containerContext = containerContext;
            this.registrationData = registrationData;
            this.injectionParameters = injectionParameters;
        }

        public bool TryChooseConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo) =>
            this.TryGetConstructor(out resolutionConstructor, resolutionInfo);

        public ResolutionMethod[] GetResolutionMethods(ResolutionInfo resolutionInfo)
        {
            if (this.metaInformation.InjectionMethods.Length == 0) return null;

            return this.metaInformation.InjectionMethods
               .Select(methodInfo => new ResolutionMethod
               {
                   Method = methodInfo.Method,
                   Parameters = methodInfo.Parameters.Select(parameter =>
                   this.containerContext.ResolutionStrategy.BuildResolutionExpression(this.containerContext, resolutionInfo, parameter, this.injectionParameters)).CastToArray()
               }).CastToArray();
        }

        public ResolutionMember[] GetResolutionMembers(ResolutionInfo resolutionInfo)
        {
            if (this.metaInformation.InjectionMembers.Length == 0) return null;

            return this.SelectFieldsAndProperties(this.metaInformation.InjectionMembers)
                .Select(memberInfo => new ResolutionMember
                {
                    Expression = this.containerContext.ResolutionStrategy
                        .BuildResolutionExpression(this.containerContext, resolutionInfo, memberInfo.TypeInformation, this.injectionParameters),
                    MemberInfo = memberInfo.MemberInfo
                }).Where(info => info.Expression != null).CastToArray();
        }

        public bool ValidateGenericContraints(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var length = typeInfo.GenericTypeArguments.Length;

            for (var i = 0; i < length; i++)
                if (this.metaInformation.GenericTypeConstraints.ContainsKey(i) && !this.metaInformation.GenericTypeConstraints[i].Contains(typeInfo.GenericTypeArguments[i]))
                    return false;

            return true;
        }
        
        private static MetaInformation GetOrCreateMetaInfo(Type typeTo)
        {
            var found = MetaRepository.GetOrDefault(typeTo);
            if (found != null) return found;

            var meta = new MetaInformation(typeTo);
            MetaRepository.AddOrUpdate(typeTo, meta);
            return meta;
        }

        private bool TryGetConstructor(out ResolutionConstructor resolutionConstructor,
            ResolutionInfo resolutionInfo)
        {
            var usableConstructors = this.CreateResolutionConstructors(resolutionInfo).CastToArray();

            if (usableConstructors.Any())
            {
                resolutionConstructor = this.SelectBestConstructor(usableConstructors);
                return true;
            }

            resolutionConstructor = null;
            return false;
        }

        private IEnumerable<ResolutionConstructor> CreateResolutionConstructors(ResolutionInfo resolutionInfo) =>
            this.metaInformation.Constructors.Select(constructorInformation => new ResolutionConstructor
            {
                Constructor = constructorInformation.Constructor,
                Parameters = constructorInformation.Parameters.Select(parameter =>
                    this.containerContext.ResolutionStrategy.BuildResolutionExpression(this.containerContext, resolutionInfo, parameter, this.injectionParameters)).CastToArray()
            }).Where(ctor => ctor.Parameters.All(param => param != null));

        private ResolutionConstructor SelectBestConstructor(IEnumerable<ResolutionConstructor> constructors)
        {
            var rule = this.registrationData.ConstructorSelectionRule ?? this.containerContext.ContainerConfigurator.ContainerConfiguration.ConstructorSelectionRule;
            return rule(constructors);
        }

        private IEnumerable<MemberInformation> SelectFieldsAndProperties(IEnumerable<MemberInformation> members)
        {
            var autoMemberInjectionEnabled = this.containerContext.ContainerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationEnabled || this.registrationData.AutoMemberInjectionEnabled;
            var autoMemberInjectionRule = this.registrationData.AutoMemberInjectionEnabled ? this.registrationData.AutoMemberInjectionRule :
                this.containerContext.ContainerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationRule;

            if (autoMemberInjectionEnabled)
                return members.Where(member => member.TypeInformation.HasDependencyAttribute ||
                    member.MemberInfo is FieldInfo && autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PrivateFields) ||
                    member.MemberInfo is PropertyInfo && (autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PropertiesWithPublicSetter) && ((PropertyInfo)member.MemberInfo).HasSetMethod() ||
                     autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PropertiesWithLimitedAccess)));

            return members.Where(member => member.TypeInformation.HasDependencyAttribute);
        }
    }
}
