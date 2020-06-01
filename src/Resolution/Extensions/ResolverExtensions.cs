using Stashbox.Utils;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Resolution.Extensions
{
    internal static class ResolverExtensions
    {
        public static bool CanResolve(this ImmutableArray<IResolver> resolvers, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var length = resolvers.Length;
            for (var i = 0; i < length; i++)
                if (resolvers[i].CanUseForResolution(typeInfo, resolutionContext))
                    return true;

            return false;
        }

        public static Expression BuildResolutionExpression(this ImmutableArray<IResolver> resolvers,
            TypeInformation typeInfo, ResolutionContext resolutionContext, IResolutionStrategy resolutionStrategy)
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

        public static IEnumerable<Expression> BuildAllResolutionExpressions(this ImmutableArray<IResolver> resolvers,
            TypeInformation typeInfo, ResolutionContext resolutionContext, IResolutionStrategy resolutionStrategy)
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
