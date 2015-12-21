using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Overrides;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        public TKey Resolve<TKey>(string name = null, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null) where TKey : class
        {
            return this.ResolveInternal(typeof(TKey), overrides, name, factoryParameters) as TKey;
        }

        public object Resolve(Type typeFrom, string name = null, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            return this.ResolveInternal(typeFrom, overrides, name, factoryParameters);
        }

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
                    ResolveType = typeInfo
                }) as TKey;
            }
        }

        private object ResolveInternal(Type typeFrom, IEnumerable<Override> overrides, string name = null, IEnumerable<object> factoryParameters = null)
        {
            var typeInfo = new TypeInformation { Type = typeFrom, DependencyName = name };
            var resolutionInfo = new ResolutionInfo
            {
                OverrideManager = overrides == null ? null : new OverrideManager(overrides.ToArray()),
                FactoryParams = factoryParameters,
                ResolveType = typeInfo
            };

            IServiceRegistration registration;
            if (this.registrationRepository.TryGetRegistration(typeInfo, out registration))
                return registration.GetInstance(resolutionInfo);

            Resolver resolver;
            if (this.resolverSelectorContainerExcluded.TryChooseResolver(this.ContainerContext,
                typeInfo, out resolver))
            {
                return resolver.Resolve(resolutionInfo);
            }

            if (this.ParentContainer != null)
                return this.ParentContainer.Resolve(typeFrom, name, factoryParameters, overrides);

            throw new ResolutionFailedException(typeFrom.FullName);
        }
    }
}
