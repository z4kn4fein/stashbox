using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Resolution;

namespace Stashbox;

internal class ContainerContext : IContainerContext
{
    public ContainerContext(IContainerContext? parentContext,
        IResolutionStrategy resolutionStrategy, ContainerConfiguration containerConfiguration)
    {
        this.ContainerConfiguration = containerConfiguration;
        this.ParentContext = parentContext;
        this.RootScope = new ResolutionScope(this);
        this.RegistrationRepository = new RegistrationRepository(containerConfiguration);
        this.DecoratorRepository = new DecoratorRepository(containerConfiguration);
        this.ResolutionStrategy = resolutionStrategy;
    }

    public IRegistrationRepository RegistrationRepository { get; }

    public IDecoratorRepository DecoratorRepository { get; }

    public IContainerContext? ParentContext { get; }

    public IResolutionScope RootScope { get; }

    public IResolutionStrategy ResolutionStrategy { get; }

    public ContainerConfiguration ContainerConfiguration { get; }

}