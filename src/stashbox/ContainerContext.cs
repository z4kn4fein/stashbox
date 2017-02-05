using System;
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
        
        internal ContainerContext(IRegistrationRepository registrationRepository, ConcurrentTree<Type, Func<object>> delegateRepository, IStashboxContainer container,
            IResolutionStrategy resolutionStrategy, ContainerConfiguration containerConfiguration)
        {
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
        public ConcurrentTree<Type, Func<object>> DelegateRepository { get; }

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
