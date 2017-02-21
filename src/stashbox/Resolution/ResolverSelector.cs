using System.Linq;
using System.Threading;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Utils;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    internal class ResolverSelector : IResolverSelector
    {
        private readonly ConcurrentTree<int, Resolver> resolverRepository;
        private int resolverCounter;
        public ResolverSelector()
        {
            this.resolverRepository = new ConcurrentTree<int, Resolver>();
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo) =>
            this.resolverRepository.Any(resolver => resolver.CanUseForResolution(containerContext, typeInfo));

        public Expression GetResolverExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            this.resolverRepository.FirstOrDefault(resolver => resolver.CanUseForResolution(containerContext, typeInfo))?
                .GetExpression(containerContext, typeInfo, resolutionInfo);

        public Expression[] GetResolverExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo) =>
            this.resolverRepository.FirstOrDefault(resolver => resolver.SupportsMany && resolver.CanUseForResolution(containerContext, typeInfo))?
                .GetExpressions(containerContext, typeInfo, resolutionInfo);

        public void AddResolver(Resolver resolver) =>
            this.resolverRepository.AddOrUpdate(Interlocked.Increment(ref resolverCounter), resolver);
    }
}
