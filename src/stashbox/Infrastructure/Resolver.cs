using Stashbox.Entity;

namespace Stashbox.Infrastructure
{
    public abstract class Resolver
    {
        protected readonly IContainerContext BuilderContext;
        protected readonly TypeInformation TypeInfo;

        protected Resolver(IContainerContext containerContext, TypeInformation typeInfo)
        {
            this.BuilderContext = containerContext;
            this.TypeInfo = typeInfo;
        }

        public abstract object Resolve(ResolutionInfo resolutionInfo);
    }
}
