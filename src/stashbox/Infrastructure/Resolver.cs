using Stashbox.Entity;

namespace Stashbox.Infrastructure
{
    public abstract class Resolver
    {
        protected readonly IBuilderContext BuilderContext;
        protected readonly TypeInformation TypeInfo;

        protected Resolver(IBuilderContext builderContext, TypeInformation typeInfo)
        {
            this.BuilderContext = builderContext;
            this.TypeInfo = typeInfo;
        }

        public abstract object Resolve(ResolutionInfo resolutionInfo);
    }
}
