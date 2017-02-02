using Stashbox.BuildUp.Resolution;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;

namespace Stashbox.Resolution
{
    internal class ActivationContext : IActivationContext
    {
        private readonly IResolverSelector resolverSelector;
        private readonly IContainerContext containerContext;

        public ActivationContext(IResolverSelector resolverSelector, IContainerContext containerContext)
        {
            this.resolverSelector = resolverSelector;
            this.containerContext = containerContext;
        }

        public object Activate(ResolutionInfo resolutionInfo, TypeInformation typeInfo)
        {
            var instance = this.containerContext.DelegateRepository.ActivateFromDelegateCacheOrDefault(typeInfo);
            if (instance != null)
                return instance;

            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo);
            if (registration != null)
                return registration.GetInstance(resolutionInfo, typeInfo);

            Resolver resolver;
            if (this.resolverSelector.TryChooseResolver(this.containerContext, typeInfo, out resolver))
                return resolver.Resolve(resolutionInfo);

            throw new ResolutionFailedException(typeInfo.Type.FullName);
        }
    }
}
