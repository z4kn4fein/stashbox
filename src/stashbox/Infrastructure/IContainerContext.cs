using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Extensions;
using Stashbox.MetaInfo;
using System;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents the container context.
    /// </summary>
    public interface IContainerContext
    {
        /// <summary>
        /// The repository of the registrations.
        /// </summary>
        IRegistrationRepository RegistrationRepository { get; }

        /// <summary>
        /// The container itself.
        /// </summary>
        IStashboxContainer Container { get; }

        /// <summary>
        /// The resolution strategy.
        /// </summary>
        IResolutionStrategy ResolutionStrategy { get; }

        /// <summary>
        /// The meta information repository.
        /// </summary>
        ExtendedImmutableTree<MetaInfoCache> MetaInfoRepository { get; }

        /// <summary>
        /// Repository of the compiled delegates.
        /// </summary>
        ExtendedImmutableTree<Func<ResolutionInfo, object>> DelegateRepository { get; }

        /// <summary>
        /// A generic key-value store.
        /// </summary>
        ConcurrentKeyValueStore<object, object> Bag { get; }
    }
}
