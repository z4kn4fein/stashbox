# Resolvers

Stashbox is using so-called *Resolver* implementations to handle special resolution requests. [Wrappers](usage/generics?id=wrappers), unknown type resolution, child-container requests, optional/default value injection; these are all handled by *Resolvers*.

## Pre-defined Resolvers
* `EnumerableResolver`: Used to resolve every registered implementation of a service.
* `LazyResolver`: Used to wrap services in `Lazy<>`.
* `FuncResolver`: Used to wrap services in `Func<>` delegates.
* `TupleResolver`: Used to wrap services in `Tuple<>`.
* `OptionalValueResolver`: Used to resolve optional parameters.
* `DefaultValueResolver`: Used to resolve default values.
* `UnknownTypeResolver`: Used to resolve services that are not registered into the container.
* `ParentContainerResolver`: Used to resolve services that are only registered in one of the parent containers.

## User-defined Resolvers
You can implement your resolver to extend the functionality of the container.
```cs
class CustomResolver : IResolver
{
    public Expression GetExpression(
        IResolutionStrategy resolutionStrategy,
        TypeInformation typeInfo,
        ResolutionContext resolutionContext)
    {
        // resolution expression generation
    }

    public bool CanUseForResolution(
        TypeInformation typeInfo,
        ResolutionContext resolutionContext)
    {
	    // the predicate that determines whether the resolver 
        // is able to resolve the requested service or not.
        return IsUsableFor(typeInfo);
    }
}
```
Then you can register your custom resolver like:
```cs
container.RegisterResolver(new CustomResolver());
```

## Visiting order
Stashbox visits the resolvers in the following order to fulfill the current resolution request:

1. `EnumerableResolver`
2. `LazyResolver`
3. `FuncResolver`
4. `TupleResolver`
5. `OptionalValueResolver`
6. `DefaultValueResolver`
7. **Custom, user-defined resolvers**
8. `ParentContainerResolver`
9. `UnknownTypeResolver`