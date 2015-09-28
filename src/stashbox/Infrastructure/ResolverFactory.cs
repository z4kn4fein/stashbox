
using Stashbox.Entity;

namespace Stashbox.Infrastructure
{
    public abstract class ResolverFactory
    {
        public abstract Resolver Create(IContainerContext containerContext, TypeInformation typeInfo);
    }
}
