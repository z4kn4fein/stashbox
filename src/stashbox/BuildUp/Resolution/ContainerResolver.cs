using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;

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

        public override object Resolve(ResolutionInfo resolutionInfo)
        {
            if (this.registrationCache == null)
                throw new ResolutionFailedException(base.TypeInfo.Type.FullName);

            return registrationCache.GetInstance(new ResolutionInfo
            {
                ResolveType = base.TypeInfo,
                FactoryParams = resolutionInfo.FactoryParams,
                OverrideManager = resolutionInfo.OverrideManager
            });
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
