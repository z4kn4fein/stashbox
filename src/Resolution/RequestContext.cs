using Stashbox.Utils;
using Stashbox.Utils.Data;
using System;
using System.Linq;

namespace Stashbox.Resolution;

internal class RequestContext : IInternalRequestContext
{
    public static readonly RequestContext Empty = new();

    public static RequestContext FromOverrides(object[]? overrides) => new(overrides);

    public static RequestContext Begin() => new();

    private readonly Tree<object> excludedInstances = new();
    private readonly Tree<object> perRequestInstances = new();
    private readonly object[]? overrides;

    private RequestContext(object[]? overrides = null)
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

    public object? GetDependencyOverrideOrDefault(Type dependencyType) =>
        this.overrides?.FirstOrDefault(dependencyType.IsInstanceOfType);

    public TResult? GetDependencyOverrideOrDefault<TResult>() =>
        (TResult?)this.GetDependencyOverrideOrDefault(TypeCache<TResult>.Type);

    public object[] GetOverrides() => this.overrides ?? TypeCache.EmptyArray<object>();

    public bool IsInstanceExcludedFromTracking(object instance)
    {
        var excluded = this.excludedInstances.GetOrDefault(instance.GetHashCode());
        return excluded != null && ReferenceEquals(excluded, instance);
    }

    public TInstance ExcludeFromTracking<TInstance>(TInstance value)
        where TInstance : class
    {
        this.excludedInstances.Add(value.GetHashCode(), value);
        return value;
    }
}