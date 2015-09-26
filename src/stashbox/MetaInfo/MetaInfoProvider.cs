using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Overrides;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox.MetaInfo
{
    internal class MetaInfoProvider : IMetaInfoProvider
    {
        private readonly IBuilderContext builderContext;
        private readonly MetaInfoCache metaInfoCache;

        public Type[] SensitivityList { get; private set; }

        public Type TypeTo => this.metaInfoCache.TypeTo;

        public MetaInfoProvider(IBuilderContext builderContext, Type typeTo)
        {
            this.builderContext = builderContext;
            this.metaInfoCache = new MetaInfoCache(typeTo);
            this.BuildSensitivityList();
        }

        public bool TryChooseConstructor(out ResolutionConstructor resolutionConstructor, OverrideManager overrideManager = null, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            return this.TryGetBestConstructor(out resolutionConstructor, overrideManager, injectionParameters);
        }

        private void BuildSensitivityList()
        {
            this.SensitivityList = this.metaInfoCache.Constructors.SelectMany(constructor => constructor.Parameters, (constructor, parameter) => parameter.TypeInformation.Type).ToArray();
        }

        private bool TryGetBestConstructor(out ResolutionConstructor resolutionConstructor, OverrideManager overrideManager = null, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            return this.TryGetConstructor(this.metaInfoCache.Constructors.Where(constructor => constructor.HasInjectionAttribute), out resolutionConstructor, overrideManager, injectionParameters) ||
                this.TryGetConstructor(this.metaInfoCache.Constructors.Where(constructor => !constructor.HasInjectionAttribute), out resolutionConstructor, overrideManager, injectionParameters);
        }

        private bool TryGetConstructor(IEnumerable<ConstructorInformation> constructors, out ResolutionConstructor resolutionConstructor, OverrideManager overrideManager = null, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            var usableConstructors = this.GetUsableConstructors(constructors, overrideManager, injectionParameters).ToArray();

            if (usableConstructors.Any())
            {
                resolutionConstructor = this.CreateResolutionConstructor(this.SelectBestConstructor(usableConstructors), injectionParameters);
                return true;
            }

            resolutionConstructor = null;
            return false;
        }

        private IEnumerable<ConstructorInformation> GetUsableConstructors(IEnumerable<ConstructorInformation> constructors, OverrideManager overrideManager = null, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            if (overrideManager == null)
                return constructors
                    .Where(constructor => constructor.Parameters
                        .All(parameter => this.builderContext.ResolverSelector.CanResolve(this.builderContext, parameter.TypeInformation) ||
                                          (injectionParameters != null &&
                                           injectionParameters.Any(injectionParameter => injectionParameter.Name == parameter.ResolutionTargetName))));

            return constructors
                .Where(constructor => constructor.Parameters
                    .All(parameter => this.builderContext.ResolverSelector.CanResolve(this.builderContext, parameter.TypeInformation) ||
                         overrideManager.ContainsValue(parameter.TypeInformation) || (injectionParameters != null &&
                         injectionParameters.Any(injectionParameter => injectionParameter.Name == parameter.ResolutionTargetName))));
        }

        private ResolutionConstructor CreateResolutionConstructor(ConstructorInformation constructorInformation, IEnumerable<InjectionParameter> injectionParameters = null)
        {
            return new ResolutionConstructor
            {
                Constructor = constructorInformation,
                Parameters = constructorInformation.Parameters.Select(parameter =>
                {
                    var parameterValue = injectionParameters?.FirstOrDefault(injectionParameter => injectionParameter.Name == parameter.ResolutionTargetName);
                    if (parameterValue != null)
                    {
                        parameter.ResolutionTargetValue = parameterValue;
                        return parameter;
                    }
                    else
                    {
                        Resolver resolver;
                        this.builderContext.ResolverSelector.TryChooseResolver(this.builderContext, parameter.TypeInformation, out resolver);
                        parameter.Resolver = resolver;
                        return parameter;
                    }
                }).ToArray()
            };
        }

        private ConstructorInformation SelectBestConstructor(IEnumerable<ConstructorInformation> constructors)
        {
            return constructors.OrderBy(constructor => constructor.Parameters.Count()).First();
        }
    }
}
