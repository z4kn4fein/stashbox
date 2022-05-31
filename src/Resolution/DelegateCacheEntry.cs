using System;
using System.Collections.Generic;

namespace Stashbox.Resolution
{
    /// <summary>
    /// Details about Stashbox's internal delegate cache state.
    /// </summary>
    public readonly struct DelegateCacheEntry
    {
        /// <summary>
        /// The service type.
        /// </summary>
        public readonly Type ServiceType;

        /// <summary>
        /// The cached resolution delegate.
        /// </summary>
        public readonly Func<IResolutionScope, IRequestContext, object>? CachedDelegate;

        /// <summary>
        /// Named resolution delegates cached for this service.
        /// </summary>
        public readonly IEnumerable<NamedCacheEntry>? NamedCacheEntries;

        /// <summary>
        /// Constructs a <see cref="DelegateCacheEntry"/>.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <param name="cachedDelegate">The cached resolution delegate.</param>
        /// <param name="namedCacheEntries">Named resolution delegates cached for this service.</param>
        public DelegateCacheEntry(Type serviceType, Func<IResolutionScope, IRequestContext, object>? cachedDelegate, IEnumerable<NamedCacheEntry>? namedCacheEntries) : this()
        {
            this.ServiceType = serviceType;
            this.CachedDelegate = cachedDelegate;
            this.NamedCacheEntries = namedCacheEntries;
        }
    }

    /// <summary>
    /// Details about a named delegate cache entry.
    /// </summary>
    public readonly struct NamedCacheEntry
    {
        /// <summary>
        /// The service name.
        /// </summary>
        public readonly object Name;

        /// <summary>
        /// The cached resolution delegate.
        /// </summary>
        public readonly Func<IResolutionScope, IRequestContext, object> CachedDelegate;

        /// <summary>
        /// Constructs a <see cref="NamedCacheEntry"/>.
        /// </summary>
        /// <param name="name">The service name.</param>
        /// <param name="cachedDelegate">The cached resolution delegate.</param>
        public NamedCacheEntry(object name, Func<IResolutionScope, IRequestContext, object> cachedDelegate) : this()
        {
            this.Name = name;
            this.CachedDelegate = cachedDelegate;
        }
    }
}
