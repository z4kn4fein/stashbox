using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;

namespace Stashbox.Resolution;

internal class DelegateCache
{
    public ImmutableTree<Type, ImmutableTree<CacheEntry>> ServiceDelegates = ImmutableTree<Type, ImmutableTree<CacheEntry>>.Empty;
    public ImmutableTree<Type, ImmutableTree<CacheEntry>> RequestContextAwareDelegates = ImmutableTree<Type, ImmutableTree<CacheEntry>>.Empty;
}

internal class CacheEntry
{
    public readonly Func<IResolutionScope, IRequestContext, object>? ServiceFactory;

    public readonly ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>>? NamedFactories;

    public CacheEntry(Func<IResolutionScope, IRequestContext, object>? serviceFactory, ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>>? namedFactories)
    {
        this.ServiceFactory = serviceFactory;
        this.NamedFactories = namedFactories;
    }
}

internal class DelegateCacheProvider
{
    public readonly DelegateCache DefaultCache = new();
    public ImmutableTree<object, DelegateCache> NamedCache = ImmutableTree<object, DelegateCache>.Empty;

    public DelegateCache GetNamedCache(object name)
    {
        var cache = this.NamedCache.GetOrDefaultByValue(name);
        if (cache != null) return cache;

        var newCache = new DelegateCache();
        return Swap.SwapValue(ref this.NamedCache, (t1, t2, _, _, items) =>
            items.AddOrUpdate(t1, t2, false), name, newCache, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder)
            ? newCache
            : this.NamedCache.GetOrDefaultByValue(name) ?? newCache;
    }
}