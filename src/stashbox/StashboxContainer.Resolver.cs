using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.MetaInfo;
using Stashbox.Overrides;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.BuildUp.Expressions;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public TKey Resolve<TKey>(string name = null, params TypeOverride[] overrides) where TKey : class
        {
            return this.ResolveInternal(typeof(TKey), overrides, name) as TKey;
        }

        /// <inheritdoc />
        public object Resolve(Type typeFrom, string name = null, params TypeOverride[] overrides)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            return this.ResolveInternal(typeFrom, overrides, name);
        }

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(params TypeOverride[] overrides) where TKey : class
        {
            var type = typeof(TKey);
            var typeInfo = new TypeInformation { Type = type };

            var registrations = this.registrationRepository.GetRegistrationsOrDefault(typeInfo);
            if (registrations == null)
                yield break;

            foreach (var registration in registrations)
            {
                yield return registration.GetFactory(new ResolutionInfo
                {
                    OverrideManager = overrides.Length == 0 ? null : new OverrideManager(overrides)
                }, typeInfo)() as TKey;
            }
        }

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, params TypeOverride[] overrides)
        {
            var typeInfo = new TypeInformation { Type = typeFrom };

            var registrations = this.registrationRepository.GetRegistrationsOrDefault(typeInfo);
            if (registrations == null)
                yield break;

            foreach (var registration in registrations)
            {
                yield return registration.GetFactory(new ResolutionInfo
                {
                    OverrideManager = overrides.Length == 0 ? null : new OverrideManager(overrides)
                }, typeInfo)();
            }
        }

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance)
        {
            var typeTo = instance.GetType();
            var metaInfoProvider = new MetaInfoProvider(this.ContainerContext, new MetaInfoCache(this.ContainerContext.ContainerConfiguration, typeTo));

            var resolutionInfo = new ResolutionInfo();
            var typeInfo = new TypeInformation { Type = typeTo };

            var expr = ExpressionDelegateFactory.CreateFillExpression(this.containerExtensionManager, this.ContainerContext,
                Expression.Constant(instance), resolutionInfo, typeInfo, null, metaInfoProvider.GetResolutionMembers(resolutionInfo), metaInfoProvider.GetResolutionMethods(resolutionInfo));

            var factory = Expression.Lambda<Func<TTo>>(expr).Compile();
            return factory();
        }

        private object ResolveInternal(Type typeFrom, TypeOverride[] overrides, string name = null)
        {
            var typeInfo = new TypeInformation { Type = typeFrom, DependencyName = name };
            var resolutionInfo = new ResolutionInfo
            {
                OverrideManager = overrides.Length == 0 ? null : new OverrideManager(overrides)
            };

            return this.activationContext.Activate(resolutionInfo, typeInfo);
        }
    }
}
