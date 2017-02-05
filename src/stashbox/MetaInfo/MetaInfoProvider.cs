using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stashbox.BuildUp.Expressions;

namespace Stashbox.MetaInfo
{
    internal class MetaInfoProvider : IMetaInfoProvider
    {
        private readonly IContainerContext containerContext;
        private readonly MetaInfoCache metaInfoCache;
        private readonly Lazy<HashSet<Type>> sensitivityList;

        public Type TypeTo => this.metaInfoCache.TypeTo;

        public bool HasInjectionMethod { get; }

        public bool HasInjectionMembers { get; }

        public bool HasGenericTypeConstraints { get; }

        public HashSet<Type> SensitivityList => this.sensitivityList.Value;

        public MetaInfoProvider(IContainerContext containerContext, MetaInfoCache metaInfoCache)
        {
            this.containerContext = containerContext;
            this.metaInfoCache = metaInfoCache;
            this.HasInjectionMethod = this.metaInfoCache.InjectionMethods.Any();
            this.HasInjectionMembers = this.metaInfoCache.InjectionMembers.Any();
            this.sensitivityList = new Lazy<HashSet<Type>>(() => new HashSet<Type>(this.metaInfoCache.Constructors.SelectMany(constructor => constructor.Parameters, (constructor, parameter) => parameter.Type)
                        .Concat(this.metaInfoCache.InjectionMethods.SelectMany(method => method.Parameters, (method, parameter) => parameter.Type))
                        .Concat(this.metaInfoCache.InjectionMembers.Select(members => members.TypeInformation.Type)).Distinct()));
            this.HasGenericTypeConstraints = this.metaInfoCache.GenericTypeConstraints.Count > 0;
        }

        public bool TryChooseConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null) =>
            this.TryGetConstructor(this.metaInfoCache.Constructors, out resolutionConstructor, resolutionInfo, injectionParameters);

        public ResolutionMethod[] GetResolutionMethods(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            return this.metaInfoCache.InjectionMethods
               .Select(methodInfo => new ResolutionMethod
               {
                   Method = methodInfo.Method,
                   Parameters = methodInfo.Parameters.Select(parameter =>
                   this.containerContext.ResolutionStrategy.BuildResolutionTarget(this.containerContext, resolutionInfo, parameter, injectionParameters)).ToArray()
               }).ToArray();
        }

        public ResolutionMember[] GetResolutionMembers(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null) =>
            this.metaInfoCache.InjectionMembers
                .Select(memberInfo => new ResolutionMember
                {
                    ResolutionTarget = this.containerContext.ResolutionStrategy.BuildResolutionTarget(this.containerContext, resolutionInfo, memberInfo.TypeInformation, injectionParameters),
                    MemberInfo = memberInfo.MemberInfo
                }).ToArray();

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
            var usableConstructors = this.CreateResolutionConstructors(constructors, resolutionInfo, injectionParameters).ToArray();

            if (usableConstructors.Any(constructor => constructor.Parameters.All(param => param.IsValid())))
            {
                resolutionConstructor = this.SelectBestConstructor(usableConstructors);
                return true;
            }

            resolutionConstructor = null;
            return false;
        }
        
        private IEnumerable<ResolutionConstructor> CreateResolutionConstructors(IEnumerable<ConstructorInformation> constructorInformations, 
            ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null) =>
            constructorInformations.Select(constructorInformation =>  new ResolutionConstructor
            {
                Constructor = constructorInformation.Constructor,
                Parameters = constructorInformation.Parameters.Select(parameter => 
                    this.containerContext.ResolutionStrategy.BuildResolutionTarget(this.containerContext, resolutionInfo, parameter, injectionParameters)).ToArray()
            });

        private ResolutionConstructor SelectBestConstructor(IEnumerable<ResolutionConstructor> constructors)=>
            this.containerContext.ContainerConfiguration.ConstructorSelectionRule(constructors);
    }
}
