using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Resolution
{
    internal class ContainerResolver : Resolver
    {
        private readonly IServiceRegistration registrationCache;

        internal ContainerResolver(IContainerContext containerContext, TypeInformation typeInfo)
            : base(containerContext, typeInfo)
        {
            containerContext.RegistrationRepository.TryGetRegistrationWithConditions(typeInfo,
                out this.registrationCache);
        }

        public override Expression GetExpression(Expression resolutionInfoExpression)
        {
            if (this.registrationCache == null)
                throw new ResolutionFailedException(base.TypeInfo.Type.FullName);

            return registrationCache.GetExpression(resolutionInfoExpression, base.TypeInfo);
        }

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            if (this.registrationCache == null)
                throw new ResolutionFailedException(base.TypeInfo.Type.FullName);

            return registrationCache.GetInstance(new ResolutionInfo
            {
                FactoryParams = resolutionInfo.FactoryParams,
                OverrideManager = resolutionInfo.OverrideManager
            }, base.TypeInfo);
        }
    }

    internal class ContainerResolverFactory : ResolverFactory
    {
        public override Resolver Create(IContainerContext containerContext, TypeInformation typeInfo)
        {
            return new ContainerResolver(containerContext, typeInfo);
        }
    }
}
