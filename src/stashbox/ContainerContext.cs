using Stashbox.Entity;
using Stashbox.Extensions;
using Stashbox.Infrastructure;
using Stashbox.MetaInfo;
using Stashbox.Registration;
using Stashbox.Utils;
using System;
using System.Threading;
using Stashbox.Configuration;

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
        /// <param name="metaInfoRepository">The meta information repository.</param>
        /// <param name="delegateRepository">Repository of the compiled delegates.</param>
        /// <param name="containerConfiguration">The container configuration.</param>
        internal ContainerContext(IRegistrationRepository registrationRepository, IStashboxContainer container,
            IResolutionStrategy resolutionStrategy, ExtendedImmutableTree<MetaInfoCache> metaInfoRepository,
            ExtendedImmutableTree<Func<ResolutionInfo, object>> delegateRepository, ContainerConfiguration containerConfiguration)
        {
            this.RegistrationRepository = registrationRepository;
            this.Container = container;
            this.ResolutionStrategy = resolutionStrategy;
            this.DelegateRepository = delegateRepository;
            this.Bag = new ConcurrentKeyValueStore<object, object>();
            this.ScopedRegistrations = new ConcurrentKeyValueStore<string, ScopedRegistrationItem>();
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
        public ExtendedImmutableTree<Func<ResolutionInfo, object>> DelegateRepository { get; }

        /// <inheritdoc />
        public ConcurrentKeyValueStore<string, ScopedRegistrationItem> ScopedRegistrations { get; }

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
