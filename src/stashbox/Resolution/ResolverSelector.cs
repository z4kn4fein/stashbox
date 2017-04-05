using System.Linq;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Utils;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    internal class ResolverSelector : IResolverSelector
    {
        private readonly ConcurrentOrderedStore<Resolver> resolverRepository;

        public ResolverSelector()
        {
            this.resolverRepository = new ConcurrentOrderedStore<Resolver>();
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo)
        {
            for (var i = 0; i < this.resolverRepository.Lenght; i++)
                if (this.resolverRepository.Get(i).CanUseForResolution(containerContext, typeInfo))
                    return true;

            return false;
        }

        public Expression GetResolverExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            for (int i = 0; i < this.resolverRepository.Lenght; i++)
            {
                var item = this.resolverRepository.Get(i);
                if (item.CanUseForResolution(containerContext, typeInfo))
                    return item.GetExpression(containerContext, typeInfo, resolutionInfo);
            }

            return null;
        }

        public Expression[] GetResolverExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            for (int i = 0; i < this.resolverRepository.Lenght; i++)
            {
                var item = this.resolverRepository.Get(i);
                if (item.SupportsMany && item.CanUseForResolution(containerContext, typeInfo))
                    return item.GetExpressions(containerContext, typeInfo, resolutionInfo);
            }

            return null;
        }

        public void AddResolver(Resolver resolver) =>
            this.resolverRepository.Add(resolver);
    }
}
