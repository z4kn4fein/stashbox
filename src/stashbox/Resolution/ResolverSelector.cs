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

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            for (var i = 0; i < this.resolverRepository.Lenght; i++)
                if (this.resolverRepository.Get(i).CanUseForResolution(containerContext, typeInfo, resolutionInfo))
                    return true;

            return this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionInfo) ||
                this.unknownTypeResolver.CanUseForResolution(containerContext, typeInfo, resolutionInfo);
        }

        public Expression GetResolverExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            for (var i = 0; i < this.resolverRepository.Lenght; i++)
            {
                var item = this.resolverRepository.Get(i);
                if (item.CanUseForResolution(containerContext, typeInfo, resolutionInfo))
                    return item.GetExpression(containerContext, typeInfo, resolutionInfo);
            }

            if (this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionInfo))
                return this.parentContainerResolver.GetExpression(containerContext, typeInfo, resolutionInfo);

            return this.unknownTypeResolver.CanUseForResolution(containerContext, typeInfo, resolutionInfo) ? this.unknownTypeResolver.GetExpression(containerContext, typeInfo, resolutionInfo) : null;
        }

        public Expression[] GetResolverExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionInfo resolutionInfo)
        {
            for (var i = 0; i < this.resolverRepository.Lenght; i++)
            {
                var item = this.resolverRepository.Get(i);
                if (item.SupportsMany && item.CanUseForResolution(containerContext, typeInfo, resolutionInfo))
                    return item.GetExpressions(containerContext, typeInfo, resolutionInfo);
            }

            return this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionInfo) ? this.parentContainerResolver.GetExpressions(containerContext, typeInfo, resolutionInfo) : null;
        }

        public void AddResolver(Resolver resolver) =>
            this.resolverRepository.Add(resolver);
    }
}
