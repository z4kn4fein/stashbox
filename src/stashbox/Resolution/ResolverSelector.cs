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

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            for (var i = 0; i < this.resolverRepository.Lenght; i++)
                if (this.resolverRepository.Get(i).CanUseForResolution(containerContext, typeInfo, resolutionContext))
                    return true;

            return this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext) ||
                this.unknownTypeResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext);
        }

        public Expression GetResolverExpression(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            for (var i = 0; i < this.resolverRepository.Lenght; i++)
            {
                var item = this.resolverRepository.Get(i);
                if (item.CanUseForResolution(containerContext, typeInfo, resolutionContext))
                    return item.GetExpression(containerContext, typeInfo, resolutionContext);
            }

            if (this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext))
                return this.parentContainerResolver.GetExpression(containerContext, typeInfo, resolutionContext);

            return this.unknownTypeResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext) ? this.unknownTypeResolver.GetExpression(containerContext, typeInfo, resolutionContext) : null;
        }

        public Expression[] GetResolverExpressions(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            for (var i = 0; i < this.resolverRepository.Lenght; i++)
            {
                var item = this.resolverRepository.Get(i);
                if (item.SupportsMany && item.CanUseForResolution(containerContext, typeInfo, resolutionContext))
                    return item.GetExpressions(containerContext, typeInfo, resolutionContext);
            }

            return this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext) ? this.parentContainerResolver.GetExpressions(containerContext, typeInfo, resolutionContext) : null;
        }

        public void AddResolver(Resolver resolver) =>
            this.resolverRepository.Add(resolver);
    }
}
