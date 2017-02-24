using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using Stashbox.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.MetaInfo
{
    internal class MetaInfoProvider : IMetaInfoProvider
    {
        private readonly IContainerContext containerContext;
        private readonly MetaInfoCache metaInfoCache;

        private readonly bool hasInjectionMethod;
        private readonly bool hasInjectionMembers;

        public Type TypeTo => this.metaInfoCache.TypeTo;

        public bool HasGenericTypeConstraints { get; }

        public MetaInfoProvider(IContainerContext containerContext, RegistrationContextData registrationData, Type typeTo)
        {
            this.metaInfoCache = new MetaInfoCache(containerContext.ContainerConfigurator, registrationData, typeTo);
            this.containerContext = containerContext;
            this.hasInjectionMethod = this.metaInfoCache.InjectionMethods.Any();
            this.hasInjectionMembers = this.metaInfoCache.InjectionMembers.Any();
            
            this.HasGenericTypeConstraints = this.metaInfoCache.GenericTypeConstraints.Count > 0;
        }

        public bool TryChooseConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null) =>
            this.TryGetConstructor(this.metaInfoCache.Constructors, out resolutionConstructor, resolutionInfo, injectionParameters);

        public ResolutionMethod[] GetResolutionMethods(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            if (!this.hasInjectionMethod) return null;

            return this.metaInfoCache.InjectionMethods
               .Select(methodInfo => new ResolutionMethod
               {
                   Method = methodInfo.Method,
                   Parameters = methodInfo.Parameters.Select(parameter =>
                   this.containerContext.ResolutionStrategy.BuildResolutionExpression(this.containerContext, resolutionInfo, parameter, injectionParameters)).ToArray()
               }).ToArray();
        }

        public ResolutionMember[] GetResolutionMembers(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            if (!this.hasInjectionMembers) return null;

            return this.metaInfoCache.InjectionMembers
                .Select(memberInfo => new ResolutionMember
                {
                    Expression = this.containerContext.ResolutionStrategy
                        .BuildResolutionExpression(this.containerContext, resolutionInfo, memberInfo.TypeInformation, injectionParameters),
                    MemberInfo = memberInfo.MemberInfo
                }).ToArray();
        }

        public bool ValidateGenericContraints(TypeInformation typeInformation)
        {
            var typeInfo = typeInformation.Type.GetTypeInfo();
            var length = typeInfo.GenericTypeArguments.Length;

            for (var i = 0; i < length; i++)
                if (this.metaInfoCache.GenericTypeConstraints.ContainsKey(i) && !this.metaInfoCache.GenericTypeConstraints[i].Contains(typeInfo.GenericTypeArguments[i]))
                    return false;

            return true;
        }

        private bool TryGetConstructor(IEnumerable<ConstructorInformation> constructors, out ResolutionConstructor resolutionConstructor,
            ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            var usableConstructors = this.CreateResolutionConstructors(constructors, resolutionInfo, injectionParameters)
                .Where(ctor => ctor.Parameters.All(param => param != null)).ToArray();

            if (usableConstructors.Any())
            {
                resolutionConstructor = this.SelectBestConstructor(usableConstructors);
                return true;
            }

            resolutionConstructor = null;
            return false;
        }

        private IEnumerable<ResolutionConstructor> CreateResolutionConstructors(IEnumerable<ConstructorInformation> constructorInformations,
            ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null) =>
            constructorInformations.Select(constructorInformation => new ResolutionConstructor
            {
                Constructor = constructorInformation.Constructor,
                Parameters = constructorInformation.Parameters.Select(parameter =>
                    this.containerContext.ResolutionStrategy.BuildResolutionExpression(this.containerContext, resolutionInfo, parameter, injectionParameters)).ToArray()
            });

        private ResolutionConstructor SelectBestConstructor(IEnumerable<ResolutionConstructor> constructors)=>
            this.containerContext.ContainerConfigurator.ContainerConfiguration.ConstructorSelectionRule(constructors);
    }
}
