using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp.Resolution
{
    public class ContainerResolverFactory : ResolverFactory
    {
        public override Resolver Create(IBuilderContext builderContext, TypeInformation typeInfo)
        {
            return new ContainerResolver(builderContext, typeInfo);
        }
    }
}
