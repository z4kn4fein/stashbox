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

        public void OnRegistration(IContainerContext containerContext, RegistrationInfo registrationInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            var properties = registrationInfo.TypeTo.GetTypeInfo().DeclaredProperties
                .Where(propertyInfo => propertyInfo.GetCustomAttribute<InjectionPropertyAttribute>() != null &&
                       (containerContext.ResolutionStrategy.CanResolve(containerContext, new TypeInformation
                       {
                           DependencyName = propertyInfo.GetCustomAttribute<InjectionPropertyAttribute>().Name,
                           Type = propertyInfo.PropertyType,
                           ParentType = registrationInfo.TypeTo,
                           MemberName = propertyInfo.Name,
                           CustomAttributes = new HashSet<Attribute>(propertyInfo.GetCustomAttributes())
                       }, injectionParameters)))
                .Select(propertyInfo => new PropertyInfoItem
                {
                    ResolutionTarget = containerContext.ResolutionStrategy.BuildResolutionTarget(containerContext, new TypeInformation
                    {
                        DependencyName = propertyInfo.GetCustomAttribute<InjectionPropertyAttribute>().Name,
                        Type = propertyInfo.PropertyType,
                        ParentType = registrationInfo.TypeTo,
                        MemberName = propertyInfo.Name,
                        CustomAttributes = new HashSet<Attribute>(propertyInfo.GetCustomAttributes())
                    }, injectionParameters),
                    PropertySetter = propertyInfo.GetPropertySetter(),

                });

            this.propertyInfoRepository.Add(registrationInfo.TypeTo, new PropertyInfoCache
            {
                Properties = new HashSet<PropertyInfoItem>(properties)
            });
        }

        public object PostBuild(object instance, Type targetType, IContainerContext containerContext, ResolutionInfo resolutionInfo, HashSet<InjectionParameter> injectionParameters = null)
        {
            PropertyInfoCache properties;
            if (!this.propertyInfoRepository.TryGet(targetType, out properties)) return instance;
            var evaluatedProperties = properties.Properties.ToDictionary(key => key, property =>
                containerContext.ResolutionStrategy.EvaluateResolutionTarget(containerContext, property.ResolutionTarget, resolutionInfo));

            foreach (var evaluatedProperty in evaluatedProperties)
            {
                evaluatedProperty.Key.PropertySetter(instance, evaluatedProperty.Value);
            }

            return instance;
        }
    }
}
