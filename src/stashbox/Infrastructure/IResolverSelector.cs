using Stashbox.Entity;
using System;

namespace Stashbox.Infrastructure
{
    public interface IResolverSelector
    {
        bool CanResolve(IBuilderContext builderContext, TypeInformation typeInfo);
        bool TryChooseResolver(IBuilderContext builderContext, TypeInformation typeInfo, out Resolver resolver);
        void AddResolverStrategy(Func<IBuilderContext, TypeInformation, bool> resolverPredicate, ResolverFactory factory);
    }
}
