using Stashbox.Entity;
using Stashbox.Extensions;
using Stashbox.Infrastructure;
using Stashbox.MetaInfo;
using System;
using Stashbox.Utils;

namespace Stashbox
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public class ContainerContext : IContainerContext
    {
        /// <summary>
        /// Constructs a <see cref="ContainerContext"/>
        /// </summary>
        /// <param name="registrationRepository">The repository of the registrations.</param>
        /// <param name="container">The container itself.</param>
        /// <param name="resolutionStrategy">The resolution strategy.</param>
        /// <param name="metaInfoRepository">The meta information repository.</param>
        /// <param name="delegateRepository">Repository of the compiled delegates.</param>
        internal ContainerContext(IRegistrationRepository registrationRepository, IStashboxContainer container,
            IResolutionStrategy resolutionStrategy, ExtendedImmutableTree<MetaInfoCache> metaInfoRepository,
            ExtendedImmutableTree<Func<ResolutionInfo, object>> delegateRepository)
        {
            this.RegistrationRepository = registrationRepository;
            this.Container = container;
            this.ResolutionStrategy = resolutionStrategy;
            this.MetaInfoRepository = metaInfoRepository;
            this.DelegateRepository = delegateRepository;
            this.Bag = new ConcurrentKeyValueStore<object, object>();
        }

        /// <summary>
        /// The repository of the registrations.
        /// </summary>
        public IRegistrationRepository RegistrationRepository { get; }

        /// <summary>
        /// The container itself.
        /// </summary>
        public IStashboxContainer Container { get; }

        /// <summary>
        /// The resolution strategy.
        /// </summary>
        public IResolutionStrategy ResolutionStrategy { get; }

        /// <summary>
        /// The meta information repository.
        /// </summary>
        public ExtendedImmutableTree<MetaInfoCache> MetaInfoRepository { get; }

        /// <summary>
        /// Repository of the compiled delegates.
        /// </summary>
        public ExtendedImmutableTree<Func<ResolutionInfo, object>> DelegateRepository { get; }

        /// <summary>
        /// A generic key-value store.
        /// </summary>
        public ConcurrentKeyValueStore<object, object> Bag { get; }
    }
}
