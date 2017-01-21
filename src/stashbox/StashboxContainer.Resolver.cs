using Stashbox.BuildUp.Resolution;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.MetaInfo;
using Stashbox.Overrides;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public TKey Resolve<TKey>(string name = null, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null) where TKey : class
        {
            return this.ResolveInternal(typeof(TKey), overrides, name, factoryParameters) as TKey;
        }

        /// <inheritdoc />
        public object Resolve(Type typeFrom, string name = null, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            return this.ResolveInternal(typeFrom, overrides, name, factoryParameters);
        }

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null) where TKey : class
        {
            var type = typeof(TKey);
            var factoryParams = factoryParameters as object[] ?? factoryParameters?.ToArray();
            var typeInfo = new TypeInformation { Type = type };
            IServiceRegistration[] registrations;
            if (!this.registrationRepository.TryGetTypedRepositoryRegistrations(typeInfo, out registrations)) yield break;
            var overridesEnumerated = overrides as Override[] ?? overrides?.ToArray();
            foreach (var registration in registrations)
            {
                yield return registration.GetInstance(new ResolutionInfo
                {
                    OverrideManager = overridesEnumerated == null ? null : new OverrideManager(overridesEnumerated),
                    FactoryParams = factoryParams,
                }, typeInfo) as TKey;
            }
        }

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null)
        {
            var factoryParams = factoryParameters as object[] ?? factoryParameters?.ToArray();
            var typeInfo = new TypeInformation { Type = typeFrom };
            IServiceRegistration[] registrations;
            if (!this.registrationRepository.TryGetTypedRepositoryRegistrations(typeInfo, out registrations)) yield break;
            var overridesEnumerated = overrides as Override[] ?? overrides?.ToArray();
            foreach (var registration in registrations)
            {
                yield return registration.GetInstance(new ResolutionInfo
                {
                    OverrideManager = overridesEnumerated == null ? null : new OverrideManager(overridesEnumerated),
                    FactoryParams = factoryParams,
                }, typeInfo);
            }
        }

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance)
        {
            var typeTo = instance.GetType();
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, new MetaInfoCache(typeTo));
            var objectExtender = new ObjectExtender(metaInfoProvider);

            var resolutionInfo = new ResolutionInfo();

            objectExtender.FillResolutionMembers(instance, this.ContainerContext, resolutionInfo);
            objectExtender.FillResolutionMethods(instance, this.ContainerContext, resolutionInfo);
            this.containerExtensionManager.ExecutePostBuildExtensions(instance, this.ContainerContext,
                resolutionInfo, new TypeInformation { Type = typeTo });

            return instance;
        }

        private object ResolveInternal(Type typeFrom, IEnumerable<Override> overrides = null, string name = null, IEnumerable<object> factoryParameters = null)
        {
            var typeInfo = new TypeInformation { Type = typeFrom, DependencyName = name };
            var enumOverrides = overrides?.ToArray();
            var enumFactoryParameters = factoryParameters?.ToArray();
            var resolutionInfo = new ResolutionInfo
            {
                OverrideManager = overrides == null ? null : new OverrideManager(enumOverrides),
                FactoryParams = enumFactoryParameters,
            };

            IServiceRegistration registration;
            if (this.registrationRepository.TryGetRegistration(typeInfo, out registration))
                return registration.GetInstance(resolutionInfo, typeInfo);

            Resolver resolver;
            if (this.resolverSelector.TryChooseResolver(this.ContainerContext,
                typeInfo, out resolver, res => res.ResolverType != typeof(ContainerResolver)))
            {
                return resolver.Resolve(resolutionInfo);
            }

            throw new ResolutionFailedException(typeFrom.FullName);
        }
    }
}
