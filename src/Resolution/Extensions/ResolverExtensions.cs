using Stashbox.Resolution.Resolvers;
using Stashbox.Utils.Data.Immutable;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Extensions
{
    internal static class ResolverExtensions
    {
        public static bool IsWrappedTypeRegistered(this ImmutableBucket<IResolver> resolvers, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var length = resolvers.Length;
            for (var i = 0; i < length; i++)
            {
                var item = resolvers[i];
                if (item is IWrapper wrapper && wrapper.TryUnWrap(typeInfo, out var unWrappedTypes))
                {
                    if (wrapper is EnumerableResolver)
                        return true;

                    foreach (var type in unWrappedTypes)
                        if (resolutionContext.CurrentContainerContext.RegistrationRepository.ContainsRegistration(type, typeInfo.DependencyName))
                            return true;
                }
            }

            return false;
        }

        public static bool CanLookupService(this ImmutableBucket<IResolver> resolvers, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var length = resolvers.Length;
            for (var i = 0; i < length; i++)
            {
                var item = resolvers[i];
                if (item is ILookup lookup && lookup.CanLookupService(typeInfo, resolutionContext))
                    return true;
            }

            return false;
        }

        public static Expression BuildResolutionExpression(this ImmutableBucket<IResolver> resolvers,
            IResolutionStrategy resolutionStrategy, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var length = resolvers.Length;
            for (var i = 0; i < length; i++)
            {
                var item = resolvers[i];
                if (item.CanUseForResolution(typeInfo, resolutionContext))
                    return item.GetExpression(resolutionStrategy, typeInfo, resolutionContext);
            }

            return null;
        }

        public static IEnumerable<Expression> BuildAllResolutionExpressions(this ImmutableBucket<IResolver> resolvers,
            IResolutionStrategy resolutionStrategy, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var length = resolvers.Length;
            for (var i = 0; i < length; i++)
            {
                var item = resolvers[i];
                if (item is IEnumerableSupportedResolver resolver && item.CanUseForResolution(typeInfo, resolutionContext))
                    return resolver.GetExpressionsForEnumerableRequest(resolutionStrategy, typeInfo, resolutionContext);
            }

            return null;
        }
    }
}
