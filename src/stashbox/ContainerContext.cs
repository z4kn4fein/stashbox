using Stashbox.Configuration;
using Stashbox.Infrastructure;
using Stashbox.Registration;
using Stashbox.Utils;
using System.Threading;

namespace Stashbox
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public class ContainerContext : IContainerContext
    {
        private int registrationNumber;

        /// <summary>
        /// Constructs a <see cref="ContainerContext"/>
        /// </summary>
        /// <param name="registrationRepository">The repository of the registrations.</param>
        /// <param name="container">The container itself.</param>
        /// <param name="resolutionStrategy">The resolution strategy.</param>
        /// <param name="containerConfiguration">The container configuration.</param>
        internal ContainerContext(IRegistrationRepository registrationRepository, IStashboxContainer container,
            IResolutionStrategy resolutionStrategy, ContainerConfiguration containerConfiguration)
        {
            this.ResolutionStrategy = resolutionStrategy;
            this.RegistrationRepository = registrationRepository;
            this.Container = container;
            this.Bag = new ConcurrentKeyValueStore<object, object>();
            this.ScopedRegistrations = new ConcurrentTree<string, ScopedRegistrationItem>();
            this.TrackedTransientObjects = new ConcurrentStore<object>();
            this.ContainerConfiguration = containerConfiguration;
        }

        /// <inheritdoc />
        public IRegistrationRepository RegistrationRepository { get; }

        /// <inheritdoc />
        public IStashboxContainer Container { get; }

        /// <inheritdoc />
        public IResolutionStrategy ResolutionStrategy { get; }

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
