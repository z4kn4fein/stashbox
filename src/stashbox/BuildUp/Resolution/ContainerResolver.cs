using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp.Resolution
{
    internal class ContainerResolver : Resolver
    {
        private readonly IServiceRegistration registrationCache;

        internal ContainerResolver(IBuilderContext builderContext, TypeInformation typeInfo)
            : base(builderContext, typeInfo)
        {
            builderContext.RegistrationRepository.TryGetRegistration(typeInfo.Type,
                out this.registrationCache, base.TypeInfo.DependencyName);
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
        public override Resolver Create(IBuilderContext builderContext, TypeInformation typeInfo)
        {
            return new ContainerResolver(builderContext, typeInfo);
        }
    }
}
