using Stashbox.BuildUp.DelegateFactory;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
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
        private readonly Lazy<HashSet<Type>> sensitivityList;

        public Type TypeTo => this.metaInfoCache.TypeTo;

        public bool HasInjectionMethod { get; }

        public bool HasInjectionMembers { get; }

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
        }

        public bool TryChooseConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            return this.TryGetBestConstructor(out resolutionConstructor, resolutionInfo, injectionParameters);
        }

        public IEnumerable<ResolutionMethod> GetResolutionMethods(InjectionParameter[] injectionParameters = null)
        {
            return this.metaInfoCache.InjectionMethods
                .Where(methodInfo => methodInfo.Parameters.All(parameter =>
                                 containerContext.ResolutionStrategy.CanResolve(containerContext, parameter, injectionParameters)))
           .Select(methodInfo => new ResolutionMethod
           {
               MethodDelegate = ExpressionDelegateFactory.CreateMethodExpression(this.containerContext,
                 methodInfo.Parameters.Select(parameter =>
                    this.containerContext.ResolutionStrategy.BuildResolutionTarget(this.containerContext, parameter, injectionParameters)).ToArray(),
                methodInfo.Method),
               Method = methodInfo.Method
           });
        }

        public IEnumerable<ResolutionMember> GetResolutionMembers(InjectionParameter[] injectionParameters = null)
        {
            return this.metaInfoCache.InjectionMembers.Where(propertyInfo =>
                   containerContext.ResolutionStrategy.CanResolve(containerContext, propertyInfo.TypeInformation, injectionParameters))
                .Select(memberInfo => new ResolutionMember
                {
                    ResolutionTarget = containerContext.ResolutionStrategy.BuildResolutionTarget(containerContext, memberInfo.TypeInformation, injectionParameters),
                    MemberSetter = memberInfo.MemberInfo.GetMemberSetter(),
                    MemberInfo = memberInfo.MemberInfo
                });
        }

        private bool TryGetBestConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo,
            InjectionParameter[] injectionParameters = null)
        {
            return this.TryGetConstructor(this.metaInfoCache.Constructors.Where(constructor => constructor.HasInjectionAttribute), out resolutionConstructor, resolutionInfo, injectionParameters) ||
                this.TryGetConstructor(this.metaInfoCache.Constructors.Where(constructor => !constructor.HasInjectionAttribute), out resolutionConstructor, resolutionInfo, injectionParameters);
        }

        private bool TryGetConstructor(IEnumerable<ConstructorInformation> constructors, out ResolutionConstructor resolutionConstructor,
            ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null)
        {
            var usableConstructors = this.GetUsableConstructors(constructors, resolutionInfo, injectionParameters).ToArray();

            if (usableConstructors.Any())
            {
                resolutionConstructor = this.CreateResolutionConstructor(this.SelectBestConstructor(usableConstructors), injectionParameters);
                return true;
            }

            resolutionConstructor = null;
            return false;
        }

        private IEnumerable<ConstructorInformation> GetUsableConstructors(IEnumerable<ConstructorInformation> constructors, ResolutionInfo resolutionInfo,
            InjectionParameter[] injectionParameters = null)
        {
            if (resolutionInfo?.OverrideManager == null)
                return constructors
                    .Where(constructor => constructor.Parameters
                        .All(parameter => this.containerContext.ResolutionStrategy.CanResolve(this.containerContext, parameter,
                        injectionParameters)));

            return constructors
                .Where(constructor => constructor.Parameters
                    .All(parameter => this.containerContext.ResolutionStrategy.CanResolve(this.containerContext, parameter,
                        injectionParameters) ||
                         resolutionInfo.OverrideManager.ContainsValue(parameter)));
        }

        private ResolutionConstructor CreateResolutionConstructor(ConstructorInformation constructorInformation, InjectionParameter[] injectionParameters = null)
        {
            return new ResolutionConstructor
            {
                Constructor = constructorInformation.Constructor,
                Parameters = constructorInformation.Parameters.Select(parameter => this.containerContext.ResolutionStrategy.BuildResolutionTarget(this.containerContext, parameter, injectionParameters)).ToArray()
            };
        }

        private ConstructorInformation SelectBestConstructor(IEnumerable<ConstructorInformation> constructors)
        {
            return constructors.OrderByDescending(constructor => constructor.Parameters.Length).First();
        }
    }
}
