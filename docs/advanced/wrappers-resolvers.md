# Wrappers & resolvers 

Stashbox uses so-called *Wrapper* and *Resolver* implementations to handle those special resolution requests that none of the service registrations can fulfill. Functionalities like [wrapper](advanced/wrappers-resolvers?id=wrappers) and [unknown type](advanced/special-resolution-cases?id=unknown-type-resolution) resolution, [cross-container requests](advanced/child-containers), [optional](advanced/special-resolution-cases?id=optional-value-injection) and [default value](advanced/special-resolution-cases?id=default-value-injection) injection are all built with resolvers.

## Pre-defined wrappers & resolvers
* `EnumerableWrapper`: Used to resolve a collection of services wrapped in one of the collection interfaces that a .NET `Array` implements. (`IEnumerable<>`, `IList<>`, `ICollection<>`, `IReadOnlyList<>`, `IReadOnlyCollection<>`) 
* `LazyWrapper`: Used to resolve services [wrapped](advanced/wrappers-resolvers?id=lazy) in `Lazy<>`.
* `FuncWrapper`: Used to resolve services [wrapped](advanced/wrappers-resolvers?id=func) in `Func<>` delegates.
* `MetadataWrapper`: Used to resolve services [wrapped](advanced/wrappers-resolvers?id=metadata-amp-tuple) in `ValueTuple<,>`, `Tuple<,>`, or `Metadata<,>`.
* `KeyValueWrapper`: Used to resolve services [wrapped](advanced/wrappers-resolvers?id=keyvaluepair-amp-readonlykeyvalue) in `KeyValuePair<,>` or `ReadOnlyKeyValue<,>`.
* `OptionalValueResolver`: Used to resolve optional parameters.
* `DefaultValueResolver`: Used to resolve default values.
* `ParentContainerResolver`: Used to resolve services that are only registered in one of the parent containers.
* `UnknownTypeResolver`: Used to resolve services that are not registered into the container.

## Wrappers
Stashbox can implicitly wrap your services into different data structures. All functionalities covered in the [service resolution](usage/service-resolution) are applied to the wrappers. Every wrapper request starts as a standard resolution; only the result is wrapped in the requested structure.

<!-- panels:start -->

<!-- div:left-panel -->
### Enumerable
Stashbox can compose a collection from each implementation registered to a service type. The requested type can be wrapped by any of the collection interfaces that a .NET `Array` implements.

<!-- div:right-panel -->

```cs
IJob[] jobs = container.Resolve<IJob[]>();
IEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>();
IList<IJob> jobs = container.Resolve<IList<IJob>>();
ICollection<IJob> jobs = container.Resolve<ICollection<IJob>>();
IReadOnlyList<IJob> jobs = container.Resolve<IReadOnlyList<IJob>>();
IReadOnlyCollection<IJob> jobs = container.Resolve<IReadOnlyCollection<IJob>>();
```

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
### Lazy
When requesting `Lazy<>`, the container implicitly constructs a new `Lazy<>` instance with a factory delegate as its constructor argument used to instantiate the underlying service. 

<!-- div:right-panel -->
```cs
container.Register<IJob, DbBackup>();

// new Lazy(() => new DbBackup())
Lazy<IJob> lazyJob = container.Resolve<Lazy<IJob>>();
IJob job = lazyJob.Value;
```
<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
### Func
When requesting `Func<>`, the container implicitly creates a factory delegate used to instantiate the underlying service.

It's possible to request a delegate that expects some or all of the dependencies as delegate parameters.
Parameters are used for sub-dependencies as well, like: `(arg) => new A(new B(arg))`

When a dependency is not available as a parameter, it will be resolved from the container directly.

<!-- div:right-panel -->

<!-- tabs:start -->
##### **Default**
```cs
container.Register<IJob, DbBackup>();

// () => new DbBackup()
Func<IJob> funcOfJob = container.Resolve<Func<IJob>>();
IJob job = funcOfJob();
```

##### **Custom parameters**
```cs
container.Register<IJob, DbBackup>();

// (conn, logger) => new DbBackup(conn, logger)
Func<string, ILogger, IJob> funcOfJob = container
    .Resolve<Func<string, ILogger, IJob>>();
    
IJob job = funcOfJob(config["connectionString"], new ConsoleLogger());
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
### Metadata & Tuple
With the `.WithMetadata()` registration option, you can attach additional information to a service.
To gather this information, you can request the service wrapped in either `Metadata<,>`, `ValueTuple<,>`, or `Tuple<,>`.

`Metadata<,>` is a type from the `Stashbox` package, so you might prefer using `ValueTuple<,>` or `Tuple<,>` if you want to avoid referencing Stashbox in certain parts of your project.

You can also filter a collection of services by their metadata. Requesting `IEnumerable<ValueTuple<,>>` will yield only those services that have the given type of metadata.
<!-- div:right-panel -->

<!-- tabs:start -->
##### **Single service**
```cs
container.Register<IJob, DbBackup>(options => options
    .WithMetadata("connection-string-to-db"));

var jobWithConnectionString = container.Resolve<Metadata<IJob, string>>();
// prints: "connection-string-to-db"
Console.WriteLine(jobWithConnectionString.Data);

var alsoJobWithConnectionString = container.Resolve<ValueTuple<IJob, string>>();
// prints: "connection-string-to-db"
Console.WriteLine(alsoJobWithConnectionString.Item2);

var stillJobWithConnectionString = container.Resolve<Tuple<IJob, string>>();
// prints: "connection-string-to-db"
Console.WriteLine(stillJobWithConnectionString.Item2);
```

##### **Collection filtering**
```cs
container.Register<IService, Service1>(options => options
    .WithMetadata("meta-1"));
container.Register<IService, Service2>(options => options
    .WithMetadata("meta-2"));
container.Register<IService, Service3>(options => options
    .WithMetadata(5));

// the result is: [Service1, Service2]
var servicesWithStringMetadata = container.Resolve<ValueTuple<IService, string>[]>();

// the result is: [Service3]
var servicesWithIntMetadata = container.Resolve<ValueTuple<IService, int>[]>();
```

<!-- tabs:end -->

<!-- panels:end -->

?> Metadata can also be a complex type e.g., an `IDictionary<,>`.

!> When no service found for a particular metadata type, the container throws a [ResolutionFailedException](diagnostics/validation?id=resolution-validation). In case of an `IEnumerable<>` request, an empty collection will be returned for a non-existing metadata.

<!-- panels:start -->

<!-- div:left-panel -->
### KeyValuePair & ReadOnlyKeyValue
With named registration, you can give your service unique identifiers. Requesting a service wrapped in a `KeyValuePair<object, TYourService>` or `ReadOnlyKeyValue<object, TYourService>` returns the requested service with its identifier as key.

`ReadOnlyKeyValue<,>` is a type from the `Stashbox` package, so you might prefer using `KeyValuePair<,>` if you want to avoid referencing Stashbox in certain parts of your project.

Requesting an `IEnumerable<KeyValuePair<,>>` will return all services of the requested type along their identifiers. When a service don't have an identifier the `Key` will be set to `null`.
<!-- div:right-panel -->
```cs
container.Register<IService, Service1>("FirstServiceId");
container.Register<IService, Service2>("SecondServiceId");
container.Register<IService, Service3>();

var serviceKeyValue1 = container
    .Resolve<KeyValuePair<object, IService>>("FirstServiceId");
// prints: "FirstServiceId"
Console.WriteLine(serviceKeyValue1.Key);

var serviceKeyValue2 = container
    .Resolve<ReadOnlyKeyValue<object, IService>>("SecondServiceId");
// prints: "SecondServiceId"
Console.WriteLine(serviceKeyValue2.Key);

// ["FirstServiceId": Service1, "SecondServiceId": Service2, null: Service3 ]
var servicesWithKeys = container.Resolve<KeyValuePair<object, IService>[]>();
```
<!-- panels:end -->

?> Wrappers can be composed e.g., `IEnumerable<Func<ILogger, Tuple<Lazy<IJob>, string>>>`.

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