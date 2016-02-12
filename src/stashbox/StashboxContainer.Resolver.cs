using Ronin.Common;
using Stashbox.BuildUp.Resolution;
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
        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="factoryParameters">The parameters for the registered factory delegate.</param>
        /// <param name="overrides">Parameter overrides.</param>
        /// <returns>The resolved object.</returns>
        public TKey Resolve<TKey>(string name = null, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null) where TKey : class
        {
            return this.ResolveInternal(typeof(TKey), overrides, name, factoryParameters) as TKey;
        }

        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instance.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="factoryParameters">The parameters for the registered factory delegate.</param>
        /// <param name="overrides">Parameter overrides.</param>
        /// <returns>The resolved object.</returns>
        public object Resolve(Type typeFrom, string name = null, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            return this.ResolveInternal(typeFrom, overrides, name, factoryParameters);
        }

        /// <summary>
        /// Resolves all registered types of a service.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="factoryParameters">The parameters for the registered factory delegate.</param>
        /// <param name="overrides">Parameter overrides.</param>
        /// <returns>The resolved object.</returns>
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

        private object ResolveInternal(Type typeFrom, IEnumerable<Override> overrides, string name = null, IEnumerable<object> factoryParameters = null)
        {
            var typeInfo = new TypeInformation { Type = typeFrom, DependencyName = name };
            var resolutionInfo = new ResolutionInfo
            {
                OverrideManager = overrides == null ? null : new OverrideManager(overrides.ToArray()),
                FactoryParams = factoryParameters,
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

            if (this.ParentContainer != null)
                return this.ParentContainer.Resolve(typeFrom, name, factoryParameters, overrides);

            throw new ResolutionFailedException(typeFrom.FullName);
        }
    }
}
