using Stashbox.Utils.Data;
using System;
using System.Linq;
using Stashbox.Utils;

namespace Stashbox.Resolution
{
    internal class RequestContext : IPerRequestInstanceHolder
    {
        public static readonly RequestContext Empty = new();

        public static RequestContext FromOverrides(object[] overrides) => new(overrides);

        public static RequestContext Begin() => new();

        private readonly Tree<object> perRequestInstances = new();
        private readonly object[] overrides;
        
        private RequestContext(object[] overrides = null)
        {
            this.overrides = overrides;
        }

        public object GetOrAddInstance(int key, Func<IResolutionScope, IRequestContext, object> factory, IResolutionScope scope)
        {
            var instance = this.perRequestInstances.GetOrDefault(key);
            if (instance != null) return instance;

            instance = factory(scope, this);
            this.perRequestInstances.Add(key, instance);
            return instance;
        }

        public object GetDependencyOverrideOrDefault(Type dependencyType) =>
            this.overrides?.FirstOrDefault(dependencyType.IsInstanceOfType);

        public TResult GetDependencyOverrideOrDefault<TResult>() =>
            (TResult)this.GetDependencyOverrideOrDefault(typeof(TResult));

        public object[] GetOverrides() => this.overrides ?? Constants.EmptyArray<object>();
    }
}
