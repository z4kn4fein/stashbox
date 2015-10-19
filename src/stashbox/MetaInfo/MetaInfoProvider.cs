using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.MetaInfo
{
    internal class MetaInfoProvider : IMetaInfoProvider
    {
        private readonly IContainerContext containerContext;
        private readonly MetaInfoCache metaInfoCache;

        public HashSet<Type> SensitivityList { get; private set; }

        public Type TypeTo => this.metaInfoCache.TypeTo;

        public MetaInfoProvider(IContainerContext containerContext, Type typeTo)
        {
            this.containerContext = containerContext;
            this.metaInfoCache = new MetaInfoCache(typeTo);
            this.BuildSensitivityList();
        }

        public bool TryChooseConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo = null, HashSet<InjectionParameter> injectionParameters = null)
        {
            return this.TryGetBestConstructor(out resolutionConstructor, resolutionInfo, injectionParameters);
        }

        private void BuildSensitivityList()
        {
            this.SensitivityList = new HashSet<Type>(this.metaInfoCache.Constructors.SelectMany(constructor => constructor.Parameters, (constructor, parameter) => parameter.TypeInformation.Type));
        }

        private bool TryGetBestConstructor(out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo = null, HashSet<InjectionParameter> injectionParameters = null)
        {
            return this.TryGetConstructor(this.metaInfoCache.Constructors.Where(constructor => constructor.HasInjectionAttribute), out resolutionConstructor, resolutionInfo, injectionParameters) ||
                this.TryGetConstructor(this.metaInfoCache.Constructors.Where(constructor => !constructor.HasInjectionAttribute), out resolutionConstructor, resolutionInfo, injectionParameters);
        }

        private bool TryGetConstructor(IEnumerable<ConstructorInformation> constructors, out ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo = null, HashSet<InjectionParameter> injectionParameters = null)
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

        private IEnumerable<ConstructorInformation> GetUsableConstructors(IEnumerable<ConstructorInformation> constructors, ResolutionInfo resolutionInfo = null, HashSet<InjectionParameter> injectionParameters = null)
        {
            if (resolutionInfo?.OverrideManager == null)
                return constructors
                    .Where(constructor => constructor.Parameters
                        .All(parameter => this.containerContext.ResolutionStrategy.CanResolve(this.containerContext, parameter.TypeInformation,
                        injectionParameters, parameter.ResolutionTargetName)));

            return constructors
                .Where(constructor => constructor.Parameters
                    .All(parameter => this.containerContext.ResolutionStrategy.CanResolve(this.containerContext, parameter.TypeInformation,
                        injectionParameters, parameter.ResolutionTargetName) ||
                         resolutionInfo.OverrideManager.ContainsValue(parameter.TypeInformation)));
        }

        private ResolutionConstructor CreateResolutionConstructor(ConstructorInformation constructorInformation, HashSet<InjectionParameter> injectionParameters = null)
        {
            return new ResolutionConstructor
            {
                Constructor = constructorInformation,
                Parameters = constructorInformation.Parameters.Select(parameter => this.containerContext.ResolutionStrategy.BuildResolutionTarget(this.containerContext, parameter.TypeInformation, injectionParameters, parameter.ResolutionTargetName)).ToArray()
            };
        }

        private ConstructorInformation SelectBestConstructor(IEnumerable<ConstructorInformation> constructors)
        {
            return constructors.OrderBy(constructor => constructor.Parameters.Count()).First();
        }
    }
}
