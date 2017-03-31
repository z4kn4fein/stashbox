using Stashbox.Infrastructure;
using Stashbox.Utils;
using System.Threading;
using Stashbox.Infrastructure.Registration;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public class ContainerContext : IContainerContext
    {
        private int registrationNumber;
        
        internal ContainerContext(IRegistrationRepository registrationRepository, IDelegateRepository delegateRepository, 
            IStashboxContainer container, IResolutionStrategy resolutionStrategy, IContainerConfigurator containerConfigurator, 
            IDecoratorRepository decoratorRepository)
        {
            this.ResolutionStrategy = resolutionStrategy;
            this.RegistrationRepository = registrationRepository;
            this.DelegateRepository = delegateRepository;
            this.Container = container;
            this.Bag = new ConcurrentKeyValueStore<object, object>();
            this.ContainerConfigurator = containerConfigurator;
            this.DecoratorRepository = decoratorRepository;
        }
        
        /// <inheritdoc />
        public IRegistrationRepository RegistrationRepository { get; }

        /// <inheritdoc />
        public IDelegateRepository DelegateRepository { get; }

        /// <inheritdoc />
        public IDecoratorRepository DecoratorRepository { get; }

        /// <inheritdoc />
        public IStashboxContainer Container { get; }

        /// <inheritdoc />
        public IResolutionStrategy ResolutionStrategy { get; }

        /// <inheritdoc />
        public ConcurrentKeyValueStore<object, object> Bag { get; }

        /// <inheritdoc />
        public IContainerConfigurator ContainerConfigurator { get; internal set; }

        /// <inheritdoc />
        public int ReserveRegistrationNumber() =>
            Interlocked.Increment(ref this.registrationNumber);
    }
}
