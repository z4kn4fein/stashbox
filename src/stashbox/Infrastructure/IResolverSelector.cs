using Stashbox.Entity;
using System;

namespace Stashbox.Infrastructure
{
    internal interface IResolverSelector
    {
        bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo);
        bool TryChooseResolver(IContainerContext containerContext, TypeInformation typeInfo, out Resolver resolver);
        void AddResolverStrategy(Func<IContainerContext, TypeInformation, bool> resolverPredicate, ResolverFactory factory);
    }
}
