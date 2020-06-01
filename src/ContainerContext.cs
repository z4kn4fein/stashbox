using Stashbox.Configuration;
using Stashbox.Expressions;
using Stashbox.Registration;
using Stashbox.Resolution;

namespace Stashbox
{
    internal class ContainerContext : IContainerContext
    {
        public ContainerContext(IContainerContext parentContext, ResolutionStrategy resolutionStrategy,
            ExpressionFactory expressionFactory, ContainerConfiguration containerConfiguration)
        {
            this.ContainerConfiguration = containerConfiguration;
            this.ParentContext = parentContext;
            this.RegistrationRepository = new RegistrationRepository(containerConfiguration);
            this.DecoratorRepository = new DecoratorRepository();
            this.RootScope = new ResolutionScope(resolutionStrategy, expressionFactory, this);
        }

        public IRegistrationRepository RegistrationRepository { get; }

        public IDecoratorRepository DecoratorRepository { get; }

        public IContainerContext ParentContext { get; }

        public IResolutionScope RootScope { get; }

        public ContainerConfiguration ContainerConfiguration { get; internal set; }
    }
}
