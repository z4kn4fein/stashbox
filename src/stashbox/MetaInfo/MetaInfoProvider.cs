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

        public HashSet<Type> SensitivityList { get; private set; }

        public Type TypeTo => this.metaInfoCache.TypeTo;

        private readonly bool hasInjectionMethod;
        public bool HasInjectionMethod => this.hasInjectionMethod;

        private readonly bool hasInjectionProperty;
        public bool HasInjectionProperty => this.hasInjectionProperty;

        public MetaInfoProvider(IContainerContext containerContext, Type typeTo)
        {
            this.containerContext = containerContext;
            this.metaInfoCache = new MetaInfoCache(typeTo);
            this.hasInjectionMethod = this.metaInfoCache.InjectionMethods.Any();
            this.hasInjectionProperty = this.metaInfoCache.InjectionProperties.Any();
            this.BuildSensitivityList();
        }

        public bool TryChooseConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo = null, InjectionParameter[] injectionParameters = null)
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
                    methodInfo.Method)
               });
        }

        public IEnumerable<ResolutionProperty> GetResolutionProperties(InjectionParameter[] injectionParameters = null)
        {
                return this.metaInfoCache.InjectionProperties
                .Select(propertyInfo => new ResolutionProperty
                {
                    ResolutionTarget = containerContext.ResolutionStrategy.BuildResolutionTarget(containerContext, propertyInfo.TypeInformation, injectionParameters),
                    PropertySetter = propertyInfo.PropertyInfo.GetPropertySetter(),
                    PropertyInfo = propertyInfo.PropertyInfo
                });
        }

        private void BuildSensitivityList()
        {
            this.SensitivityList = new HashSet<Type>(this.metaInfoCache.Constructors.SelectMany(constructor => constructor.Parameters, (constructor, parameter) => parameter.Type)
                        .Concat(this.metaInfoCache.InjectionMethods.SelectMany(method => method.Parameters, (method, parameter) => parameter.Type))
                        .Concat(this.metaInfoCache.InjectionProperties.Select(property => property.TypeInformation.Type)).Distinct());
        }

        private bool TryGetBestConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo = null, InjectionParameter[] injectionParameters = null)
        {
            return this.TryGetConstructor(this.metaInfoCache.Constructors.Where(constructor => constructor.HasInjectionAttribute), out resolutionConstructor, resolutionInfo, injectionParameters) ||
                this.TryGetConstructor(this.metaInfoCache.Constructors.Where(constructor => !constructor.HasInjectionAttribute), out resolutionConstructor, resolutionInfo, injectionParameters);
        }

        private bool TryGetConstructor(IEnumerable<ConstructorInformation> constructors, out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo = null, InjectionParameter[] injectionParameters = null)
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

        private IEnumerable<ConstructorInformation> GetUsableConstructors(IEnumerable<ConstructorInformation> constructors, ResolutionInfo resolutionInfo = null, InjectionParameter[] injectionParameters = null)
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
            return constructors.OrderBy(constructor => constructor.Parameters.Count()).First();
        }
    }
}
