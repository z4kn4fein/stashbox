using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;
using Stashbox.Exceptions;

namespace Stashbox.BuildUp.Resolution
{
    internal class ParentContainerResolver : Resolver
    {
        private readonly IServiceRegistration registrationCache;

        public ParentContainerResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            this.registrationCache = containerContext.Container.ParentContainer.ContainerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo, true);
            if (this.registrationCache == null)
                throw new ResolutionFailedException(typeInfo.Type.FullName);
        }
        
        public override Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            return this.registrationCache.GetExpression(resolutionInfo, base.TypeInfo);
        }
    }
}
