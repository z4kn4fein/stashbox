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
        public TKey Resolve<TKey>(string name = null, IEnumerable<object> factoryParameters = null, params Override[] overrides) where TKey : class
        {
            return this.ResolveInternal(typeof(TKey), overrides, name, factoryParameters) as TKey;
        }

        public object Resolve(Type typeFrom, string name = null, IEnumerable<object> factoryParameters = null, params Override[] overrides)
        {
            Shield.EnsureNotNull(typeFrom);
            return this.ResolveInternal(typeFrom, overrides, name, factoryParameters);
        }

        public IEnumerable<TKey> ResolveAll<TKey>(IEnumerable<object> factoryParameters = null, params Override[] overrides) where TKey : class
        {
            var type = typeof(TKey);
            var factoryParams = factoryParameters as object[] ?? factoryParameters.ToArray();
            IDictionary<string, IServiceRegistration> registrations;
            if (!this.registrationRepository.TryGetTypedRepositoryRegistrations(type, out registrations)) yield break;
            foreach (var registration in registrations)
            {
                yield return registration.Value.GetInstance(new ResolutionInfo
                {
                    OverrideManager = new OverrideManager(overrides),
                    FactoryParams = factoryParams,
                    ResolveType = new TypeInformation { Type = type }
                }) as TKey;
            }
        }

        private object ResolveInternal(Type typeFrom, IEnumerable<Override> overrides, string name = null, IEnumerable<object> factoryParameters = null)
        {
            Resolver resolver;
            if (this.resolverSelector.TryChooseResolver(this.builderContext,
                new TypeInformation { Type = typeFrom, DependencyName = name }, out resolver))
            {
                return resolver.Resolve(new ResolutionInfo
                {
                    OverrideManager = new OverrideManager(overrides),
                    FactoryParams = factoryParameters,
                    ResolveType = new TypeInformation { Type = typeFrom, DependencyName = name }
                });
            }
            throw new ResolutionFailedException(typeFrom.FullName);
        }
    }
}
