# Wrappers & resolvers 

Stashbox uses so-called *Wrapper* and *Resolver* implementations to handle those special resolution requests that none of the service registrations can fulfill. Functionalities like [wrapper](advanced/generics?id=wrappers) and [unknown type](advanced/special-resolution-cases?id=unknown-type-resolution) resolution, [cross-container requests](advanced/child-containers), [optional](advanced/special-resolution-cases?id=optional-value-injection) and [default value](advanced/special-resolution-cases?id=default-value-injection) injection are all built with resolvers.

## Pre-defined wrappers & resolvers
* `EnumerableWrapper`: Used to resolve a collection of services of the same type.
* `LazyWrapper`: Used to resolve services [wrapped](advanced/generics?id=lazy) in `Lazy<>`.
* `FuncWrapper`: Used to resolve services [wrapped](advanced/generics?id=func) in `Func<>` delegates.
* `MetadataWrapper`: Used to resolve services [wrapped](advanced/generics?id=metadata-amp-tuple) in `Tuple<>` or `Metadata<>`.
* `KeyValueWrapper`: Used to resolve services [wrapped](advanced/generics?id=keyvaluepair-amp-readonlykeyvalue) in `KeyValuePair` or `ReadOnlyKeyValue<>`.
* `OptionalValueResolver`: Used to resolve optional parameters.
* `DefaultValueResolver`: Used to resolve default values.
* `ParentContainerResolver`: Used to resolve services that are only registered in one of the parent containers.
* `UnknownTypeResolver`: Used to resolve services that are not registered into the container.


## User-defined wrappers & resolvers
You can add support for more wrapper types by implementing the `IServiceWrapper` interface.
```cs
class CustomWrapper : IServiceWrapper
{
    // this method is supposed to generate the expression for the given wrapper's 
    // instantiation when it's selected by the container to resolve the actual service.
    public Expression WrapExpression(
        TypeInformation originalTypeInformation, 
        TypeInformation wrappedTypeInformation, 
        ServiceContext serviceContext)
    {
        // produce the expression for the wrapper.
    }

    // this method is called by the container to determine whether a 
    // given requested type is wrapped by a supported wrapper type.
    public bool TryUnWrap(
        TypeInformation typeInformation, 
        out TypeInformation unWrappedType)
    {
        // this is just a reference implementation of 
        // un-wrapping a service from a given wrapper.
        if (!CanUnWrapServiceType(typeInformation.Type))
        {
            unWrappedType = null;
            return false;
        }

        var type = UnWrapServiceType(typeInformation.Type)

        unWrappedType = typeInformation.Clone(type);
        return true;
    }
}
```

You can extend the functionality of the container by implementing the `IServiceResolver` interface.
```cs
class CustomResolver : IServiceResolver
{
    // called to generate the expression for the given service
    // when this resolver is selected (through CanUseForResolution()) 
    // to fulfill the request.
    public ServiceContext GetExpression(
        IResolutionStrategy resolutionStrategy,
        TypeInformation typeInfo,
        ResolutionContext resolutionContext)
    {
        var expression = GenerateExpression(); // resolution expression generation.
        return expression.AsServiceContext();
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
Then you can register your custom wrapper or resolver like this:
```cs
container.RegisterResolver(new CustomWrapper());
container.RegisterResolver(new CustomResolver());
```

## Visiting order
Stashbox visits the wrappers and resolvers in the following order to satisfy the actual resolution request:

1. `EnumerableWrapper`
2. `LazyWrapper`
3. `FuncWrapper`
4. `MetadataWrapper`
5. `KeyValueWrapper`
6. **Custom, user-defined wrappers & resolvers**
7. `OptionalValueResolver`
8. `DefaultValueResolver`
9. `ParentContainerResolver`
10. `UnknownTypeResolver`