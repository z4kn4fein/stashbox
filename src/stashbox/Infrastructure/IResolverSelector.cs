using Stashbox.Entity;

namespace Stashbox.Infrastructure
{
    internal interface IResolverSelector
    {
        bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo);
        bool TryChooseResolver(IContainerContext containerContext, TypeInformation typeInfo, out Resolver resolver);
        void AddResolver(ResolverRegistration resolverRegistration);
        IResolverSelector CreateCopy();
    }
}
