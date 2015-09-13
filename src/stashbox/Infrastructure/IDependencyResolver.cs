using Stashbox.Overrides;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    public interface IDependencyResolver
    {
        TKey Resolve<TKey>(string name = null, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null)
           where TKey : class;

        object Resolve(Type typeFrom, string name = null, IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null);

        IEnumerable<TKey> ResolveAll<TKey>(IEnumerable<object> factoryParameters = null, IEnumerable<Override> overrides = null)
             where TKey : class;
    }
}
