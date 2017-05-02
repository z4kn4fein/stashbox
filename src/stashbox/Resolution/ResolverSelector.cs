using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Utils;
using System.Linq.Expressions;
using Stashbox.BuildUp.Resolution;

namespace Stashbox.Resolution
{
    internal class ResolverSelector : IResolverSelector
    {
        private readonly ConcurrentOrderedStore<Resolver> resolverRepository;
        private readonly UnknownTypeResolver unknownTypeResolver;
        private readonly ParentContainerResolver parentContainerResolver;

        public ResolverSelector()
        {
            this.resolverRepository = new ConcurrentOrderedStore<Resolver>();
            this.unknownTypeResolver = new UnknownTypeResolver();
            this.parentContainerResolver = new ParentContainerResolver();
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo)
        {
            for (var i = 0; i < this.resolverRepository.Lenght; i++)
                if (this.resolverRepository.Get(i).CanUseForResolution(containerContext, typeInfo))
                    return true;

            return this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo) ||
                this.unknownTypeResolver.CanUseForResolution(containerContext, typeInfo);
        }

        public Expression GetResolverExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            for (var i = 0; i < this.resolverRepository.Lenght; i++)
            {
                var item = this.resolverRepository.Get(i);
                if (item.CanUseForResolution(containerContext, typeInfo))
                    return item.GetExpression(containerContext, typeInfo, resolutionInfo);
            }

            if (this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo))
                return this.parentContainerResolver.GetExpression(containerContext, typeInfo, resolutionInfo);

            return this.unknownTypeResolver.CanUseForResolution(containerContext, typeInfo) ? this.unknownTypeResolver.GetExpression(containerContext, typeInfo, resolutionInfo) : null;
        }

        public Expression[] GetResolverExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            for (var i = 0; i < this.resolverRepository.Lenght; i++)
            {
                var item = this.resolverRepository.Get(i);
                if (item.SupportsMany && item.CanUseForResolution(containerContext, typeInfo))
                    return item.GetExpressions(containerContext, typeInfo, resolutionInfo);
            }

            return this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo) ? this.parentContainerResolver.GetExpressions(containerContext, typeInfo, resolutionInfo) : null;
        }

        public void AddResolver(Resolver resolver) =>
            this.resolverRepository.Add(resolver);
    }
}
