using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.ContainerExtensions.PropertyInjection
{
    public class PropertyInjectionExtension : IPostBuildExtension, IRegistrationExtension
    {
        private readonly ConcurrentKeyValueStore<Type, PropertyInfoCache> propertyInfoRepository;

        public PropertyInjectionExtension()
        {
            this.propertyInfoRepository = new ConcurrentKeyValueStore<Type, PropertyInfoCache>();
        }

        public void OnRegistration(IBuilderContext builderContext, RegistrationInfo registrationInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            var properties = registrationInfo.TypeTo.GetTypeInfo().DeclaredProperties
                .Where(propertyInfo => propertyInfo.GetCustomAttribute<InjectionPropertyAttribute>() != null &&
                       (builderContext.ResolverSelector.CanResolve(builderContext, new TypeInformation
                       {
                           DependencyName = propertyInfo.GetCustomAttribute<InjectionPropertyAttribute>().Name,
                           Type = propertyInfo.PropertyType
                       })) || (injectionParameters != null && injectionParameters.Any(param => param.Name == propertyInfo.Name)))
                .Select(propertyInfo =>
                {
                    Resolver resolver;
                    builderContext.ResolverSelector.TryChooseResolver(builderContext, new TypeInformation
                    {
                        DependencyName = propertyInfo.GetCustomAttribute<InjectionPropertyAttribute>().Name,
                        Type = propertyInfo.PropertyType
                    }, out resolver);

                    return new PropertyInfoItem
                    {
                        ResolutionTarget = new ResolutionTarget
                        {
                            Resolver = resolver,
                            TypeInformation = new TypeInformation
                            {
                                DependencyName = propertyInfo.GetCustomAttribute<InjectionPropertyAttribute>().Name,
                                Type = propertyInfo.PropertyType
                            },
                            ResolutionTargetValue = injectionParameters?.FirstOrDefault(param => param.Name == propertyInfo.Name)?.Value
                        },

                        PropertySetter = propertyInfo.GetPropertySetter(),

                    };
                });

            this.propertyInfoRepository.Add(registrationInfo.TypeTo, new PropertyInfoCache
            {
                Properties = new HashSet<PropertyInfoItem>(properties)
            });
        }

        public object PostBuild(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            PropertyInfoCache properties;
            if (this.propertyInfoRepository.TryGet(instance.GetType(), out properties))
            {
                var evaluatedProperties = properties.Properties.ToDictionary(key => key, property =>
                {
                    if (resolutionInfo.OverrideManager.ContainsValue(new TypeInformation { DependencyName = property.ResolutionTarget.TypeInformation.DependencyName, Type = property.ResolutionTarget.TypeInformation.Type }))
                    {
                        return resolutionInfo.OverrideManager.GetOverriddenValue(property.ResolutionTarget.TypeInformation.Type, property.ResolutionTarget.TypeInformation.DependencyName);
                    }
                    else if (property.ResolutionTarget.ResolutionTargetValue != null)
                    {
                        return property.ResolutionTarget.ResolutionTargetValue;
                    }
                    else
                        return property.ResolutionTarget.Resolver.Resolve(new ResolutionInfo
                        {
                            FactoryParams = resolutionInfo.FactoryParams,
                            OverrideManager = resolutionInfo.OverrideManager,
                            ResolveType = property.ResolutionTarget.TypeInformation
                        });
                });

                foreach (var evaluatedProperty in evaluatedProperties)
                {
                    evaluatedProperty.Key.PropertySetter(instance, evaluatedProperty.Value);
                }
            }

            return instance;
        }
    }
}
