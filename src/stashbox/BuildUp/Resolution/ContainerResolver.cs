using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class ContainerResolver : Resolver
    {
        private readonly IServiceRegistration registrationCache;

        public ContainerResolver(IContainerContext containerContext, TypeInformation typeInfo, IServiceRegistration serviceRegistration)
            : base(containerContext, typeInfo)
        {
            this.registrationCache = serviceRegistration;
        }

        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            return registrationCache.GetExpression(resolutionInfo, base.TypeInfo);
        }
    }
}
