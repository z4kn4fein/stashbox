using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;

namespace Stashbox
{
    internal class DelegateCache
    {
        public ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>> ServiceDelegates = 
            ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>>.Empty;
        public ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>> RequestContextAwareDelegates =
            ImmutableTree<object, Func<IResolutionScope, IRequestContext, object>>.Empty;
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
}
