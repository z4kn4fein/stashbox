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
        private readonly bool autoMemberInjectionEnabled;
        private readonly Rules.AutoMemberInjection autoMemberInjectionRule;

        public Type TypeTo { get; }

        public bool HasGenericTypeConstraints => this.metaInformation.GenericTypeConstraints.Count > 0;

        private readonly MetaInformation metaInformation;

        public MetaInfoProvider(IContainerContext containerContext, RegistrationContextData registrationData, Type typeTo)
        {
            this.metaInformation = GetOrCreateMetaInfo(typeTo);

            this.TypeTo = typeTo;

            this.autoMemberInjectionEnabled = containerContext.ContainerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationEnabled || registrationData.AutoMemberInjectionEnabled;
            this.autoMemberInjectionRule = registrationData.AutoMemberInjectionEnabled ? registrationData.AutoMemberInjectionRule :
                containerContext.ContainerConfigurator.ContainerConfiguration.MemberInjectionWithoutAnnotationRule;

            this.containerContext = containerContext;
        }

        private static MetaInformation GetOrCreateMetaInfo(Type typeTo)
        {
            var found = MetaRepository.GetOrDefault(typeTo);
            if (found != null) return found;

            var meta = new MetaInformation(typeTo);
            MetaRepository.AddOrUpdate(typeTo, meta);
            return meta;
        }

        public bool TryChooseConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null) =>
            this.TryGetConstructor(out resolutionConstructor, resolutionInfo, injectionParameters);

        public ResolutionMethod[] GetResolutionMethods(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            if (this.metaInformation.InjectionMethods.Length == 0) return null;

            return this.metaInformation.InjectionMethods
               .Select(methodInfo => new ResolutionMethod
               {
                   Method = methodInfo.Method,
                   Parameters = methodInfo.Parameters.Select(parameter =>
                   this.containerContext.ResolutionStrategy.BuildResolutionExpression(this.containerContext, resolutionInfo, parameter, injectionParameters)).ToArray()
               }).ToArray();
        }

        public ResolutionMember[] GetResolutionMembers(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            if (this.metaInformation.InjectionMembers.Length == 0) return null;

            return this.SelectFieldsAndProperties(this.metaInformation.InjectionMembers)
                .Select(memberInfo => new ResolutionMember
                {
                    Expression = this.containerContext.ResolutionStrategy
                        .BuildResolutionExpression(this.containerContext, resolutionInfo, memberInfo.TypeInformation, injectionParameters),
                    MemberInfo = memberInfo.MemberInfo
                }).Where(info => info.Expression != null).ToArray();
        }

        public bool ValidateGenericContraints(TypeInformation typeInformation)
        {
            var typeInfo = typeInformation.Type.GetTypeInfo();
            var length = typeInfo.GenericTypeArguments.Length;

            for (var i = 0; i < length; i++)
                if (this.metaInformation.GenericTypeConstraints.ContainsKey(i) && !this.metaInformation.GenericTypeConstraints[i].Contains(typeInfo.GenericTypeArguments[i]))
                    return false;

            return true;
        }

        private bool TryGetConstructor(out ResolutionConstructor resolutionConstructor,
            ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            var usableConstructors = this.CreateResolutionConstructors(resolutionInfo, injectionParameters)
                .Where(ctor => ctor.Parameters.All(param => param != null)).ToArray();

            if (usableConstructors.Any())
            {
                resolutionConstructor = this.SelectBestConstructor(usableConstructors);
                return true;
            }

            resolutionConstructor = null;
            return false;
        }

        private IEnumerable<ResolutionConstructor> CreateResolutionConstructors(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null) =>
            this.metaInformation.Constructors.Select(constructorInformation => new ResolutionConstructor
            {
                Constructor = constructorInformation.Constructor,
                Parameters = constructorInformation.Parameters.Select(parameter =>
                    this.containerContext.ResolutionStrategy.BuildResolutionExpression(this.containerContext, resolutionInfo, parameter, injectionParameters)).ToArray()
            });

        private ResolutionConstructor SelectBestConstructor(IEnumerable<ResolutionConstructor> constructors) =>
            this.containerContext.ContainerConfigurator.ContainerConfiguration.ConstructorSelectionRule(constructors);

        private IEnumerable<MemberInformation> SelectFieldsAndProperties(IEnumerable<MemberInformation> members)
        {
            if (this.autoMemberInjectionEnabled)
                return members.Where(member => member.TypeInformation.HasDependencyAttribute ||
                    member.MemberInfo is FieldInfo && this.autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PrivateFields) ||
                    member.MemberInfo is PropertyInfo && (this.autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PropertiesWithPublicSetter) && ((PropertyInfo)member.MemberInfo).HasSetMethod() ||
                     this.autoMemberInjectionRule.HasFlag(Rules.AutoMemberInjection.PropertiesWithLimitedAccess)));

            return members.Where(member => member.TypeInformation.HasDependencyAttribute);
        }
    }
}
