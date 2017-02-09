using Stashbox.Configuration;
using Stashbox.Infrastructure;
using Stashbox.Registration;
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
        
        internal ContainerContext(IRegistrationRepository registrationRepository, IDelegateRepository delegateRepository, IStashboxContainer container,
            IResolutionStrategy resolutionStrategy, IResolverSelector resolverSelector, ContainerConfiguration containerConfiguration)
        {
            this.ResolverSelector = resolverSelector;
            this.ResolutionStrategy = resolutionStrategy;
            this.RegistrationRepository = registrationRepository;
            this.DelegateRepository = delegateRepository;
            this.Container = container;
            this.Bag = new ConcurrentKeyValueStore<object, object>();
            this.ScopedRegistrations = new ConcurrentTree<string, ScopedRegistrationItem>();
            this.TrackedTransientObjects = new ConcurrentStore<object>();
            this.ContainerConfiguration = containerConfiguration;
        }
        
        /// <inheritdoc />
        public IRegistrationRepository RegistrationRepository { get; }

        /// <inheritdoc />
        public IDelegateRepository DelegateRepository { get; }

        /// <inheritdoc />
        public IStashboxContainer Container { get; }

        /// <inheritdoc />
        public IResolutionStrategy ResolutionStrategy { get; }

        /// <inheritdoc />
        public IResolverSelector ResolverSelector { get; }

        /// <inheritdoc />
        public ConcurrentTree<string, ScopedRegistrationItem> ScopedRegistrations { get; }

        /// <inheritdoc />
        public ConcurrentKeyValueStore<object, object> Bag { get; }

        /// <inheritdoc />
        public ConcurrentStore<object> TrackedTransientObjects { get; }

        /// <inheritdoc />
        public ContainerConfiguration ContainerConfiguration { get; internal set; }

        /// <inheritdoc />
        public int ReserveRegistrationNumber()
        {
            return Interlocked.Increment(ref this.registrationNumber);
        }
    }
}
