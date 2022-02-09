# Generics
This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, [generic decorators](advanced/decorators?id=generic-decorators), conditions based on generic constraints, and variance.

<!-- panels:start -->

<!-- div:title-panel -->
## Closed-generics

<!-- div:left-panel -->
The registration of a closed-generic type does not differ from registering a simple non-generic service.

You have all options available that you saw at the [basic](usage/basics) and [advanced registration](usage/advanced-registration) flows.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
container.Register<IValidator<User>, UserValidator>();
IValidator<User> validator = container.Resolve<IValidator<User>>();
```
#### **Runtime type API**
```cs
container.Register(typeof(IValidator<User>), typeof(UserValidator));
object validator = container.Resolve(typeof(IValidator<User>));
```
<!-- tabs:end -->

<!-- panels:end -->


## Open-generics

The registration of an open-generic type differs from registering a closed-generic one as C# doesn't allow the usage of open-generic types in generic method parameters. We have to get a runtime type from the open-generic type first with `typeof()`.

<!-- panels:start -->

<!-- div:left-panel -->
Open-generic types could help in such scenarios where you have generic interface-implementation pairs with numerous generic parameter variations. The registration of those different versions would look like this: 

<!-- div:right-panel -->
```cs
container.Register<IValidator<User>, Validator<User>>();
container.Register<IValidator<Role>, Validator<Role>>();
container.Register<IValidator<Company>, Validator<Company>>();
// and so on...
```
<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
Rather than doing that, you can register your type's generic definition and let Stashbox bind the type parameters for you. When a matching closed service type is requested, the container will construct an equivalent closed-generic implementation.

<!-- div:right-panel -->

```cs
container.Register(typeof(IValidator<>), typeof(Validator<>));
// Validator<User> will be returned.
IValidator<User> userValidator = container.Resolve<IValidator<User>>();
// Validator<Role> will be returned.
IValidator<Role> roleValidator = container.Resolve<IValidator<Role>>();
```

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
A registered closed-generic type always has priority over an open-generic type at service selection.

<!-- div:right-panel -->
```cs
container.Register<IValidator<User>, UserValidator>();
container.Register(typeof(IValidator<>), typeof(Validator<>));
// UserValidator will be returned.
IValidator<User> validator = container.Resolve<IValidator<User>>();
```
<!-- panels:end -->


## Generic constraints
In the following examples, you can see how the container handles generic constraints during service resolution. Constraints can be used for [conditional resolution](usage/service-resolution?id=conditional-resolution) including collection filters. 

<!-- tabs:start -->
#### **Conditional resolution**
The container chooses `UpdatedEventHandler` because it is the only one that has a constraint satisfied by the requested `UserUpdatedEvent` generic parameter as it's implementing `IUpdatedEvent`.
```cs
interface IEventHandler<TEvent> { }

// event interfaces
interface IUpdatedEvent { }
interface ICreatedEvent { }

// event handlers
class UpdatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IUpdatedEvent { }
class CreatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : ICreatedEvent { }

// event implementation
class UserUpdatedEvent : IUpdatedEvent { }

using var container = new StashboxContainer();

container.RegisterTypesAs(typeof(IEventHandler<>), new[] 
    { 
        typeof(UpdateEventHandler<>), 
        typeof(CreateEventHandler<>) 
    });

// eventHandler will be UpdatedEventHandler<ConstraintArgument>
IEventHandler<UserUpdatedEvent> eventHandler = container.Resolve<IEventHandler<UserUpdatedEvent>>();
```

#### **Collection filter**
This example shows how the container is filtering out those services from the returned collection that does not satisfy the given generic constraint needed to create the closed generic type.
```cs
interface IEventHandler<TEvent> { }

// event interfaces
interface IUpdatedEvent { }
interface ICreatedEvent { }

// event handlers
class UpdatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IUpdatedEvent { }
class CreatedEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : ICreatedEvent { }

// event implementation
class UserUpdatedEvent : IUpdatedEvent { }

using var container = new StashboxContainer();

container.RegisterTypesAs(typeof(IEventHandler<>), new[] 
    { 
        typeof(UpdateEventHandler<>), 
        typeof(CreateEventHandler<>) 
    });

// eventHandlers will contain only UpdatedEventHandler<ConstraintArgument>
IEnumerable<IEventHandler<UserUpdatedEvent>> eventHandlers = container.ResolveAll<IEventHandler<UserUpdatedEvent>>();
```
<!-- tabs:end -->

## Variance
Since .NET Framework 4.0, C# supports [covariance and contravariance](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/) in generic interfaces and delegates and allows implicit conversion of generic type parameters. In this section, we'll focus on variance in generic interfaces. 

[Here](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/creating-variant-generic-interfaces) you can read more about how to create variant generic interfaces, and the following example will show how you can use them with Stashbox.

<!-- tabs:start -->
#### **Contravariance**
**Contravariance** only allows argument types that are less derived than that defined by the generic parameters. You can declare a generic type parameter contravariant by using the `in` keyword.

```cs
// contravariant generic event handler interface
interface IEventHandler<in TEvent> { } 

// event interfaces
interface IGeneralEvent { }
interface IUpdatedEvent : IGeneralEvent { }

// event handlers
class GeneralEventHandler : IEventHandler<IGeneralEvent> { }
class UpdatedEventHandler : IEventHandler<IUpdatedEvent> { }

container.Register<IEventHandler<IGeneralEvent>, GeneralEventHandler>();
container.Register<IEventHandler<IUpdatedEvent>, UpdatedEventHandler>();

// eventHandlers contain both GeneralEventHandler and UpdatedEventHandler
IEnumerable<IEventHandler<IUpdatedEvent>> eventHandlers = container.ResolveAll<IEventHandler<IUpdatedEvent>>();
```
Despite the fact that only `IEventHandler<IUpdatedEvent>` implementations were requested, the result contains both `GeneralEventHandler` and `UpdatedEventHandler`. As `TEvent` is declared **contravariant** with the `in` keyword, and `IGeneralEvent` is less derived than `IUpdatedEvent`, `IEventHandler<IGeneralEvent>` implementations can be part of `IEventHandler<IUpdatedEvent>` collections.

If we request `IEventHandler<IGeneralEvent>`, only `GeneralEventHandler` would be returned, because `IUpdatedEvent` is more derived, so `IEventHandler<IUpdatedEvent>` implementations are not fit into `IEventHandler<IGeneralEvent>` collections. 


#### **Covariance**
**Covariance** only allows argument types that are more derived than that defined by the generic parameters. You can declare a generic type parameter covariant by using the `out` keyword.
```cs
// covariant generic event handler interface
interface IEventHandler<out TEvent> { } 

// event interfaces
interface IGeneralEvent { }
interface IUpdatedEvent : IGeneralEvent { }

// event handlers
class GeneralEventHandler : IEventHandler<IGeneralEvent> { }
class UpdatedEventHandler : IEventHandler<IUpdatedEvent> { }

container.Register<IEventHandler<IGeneralEvent>, GeneralEventHandler>();
container.Register<IEventHandler<IUpdatedEvent>, UpdatedEventHandler>();

// eventHandlers contain both GeneralEventHandler and UpdatedEventHandler
IEnumerable<IEventHandler<IGeneralEvent>> eventHandlers = container.ResolveAll<IEventHandler<IGeneralEvent>>();
```

Despite the fact that only `IEventHandler<IGeneralEvent>` implementations were requested, the result contains both `GeneralEventHandler` and `UpdatedEventHandler`. As `TEvent` is declared **covariant** with the `out` keyword, and `IUpdatedEvent` is more derived than `IGeneralEvent`, `IEventHandler<IUpdatedEvent>` implementations can be part of `IEventHandler<IGeneralEvent>` collections.

If we request `IEventHandler<IUpdatedEvent>`, only `UpdatedEventHandler` would be returned, because `IGeneralEvent` is less derived, so `IEventHandler<IGeneralEvent>` implementations are not fit into `IEventHandler<IUpdatedEvent>` collections.

<!-- tabs:end -->

## Wrappers
Stashbox can implicitly wrap your services into different data structures. All functionalities covered in the [service resolution](usage/service-resolution) are applied to the wrappers. Every wrapper request starts as a standard resolution; only the result is wrapped in the requested structure.

 This section will cover those pre-defined wrapper types that Stashbox supports. The enumerable wrapper (not listed here) is used to support resolution requests of all those collection types described in the [Multiple implementations](usage/advanced-registration?id=multiple-implementations) section.

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
To gather this information, you can request the service wrapped in either `Metadata<,>` or `Tuple<,>`.

`Metadata<,>` is a type from the `Stashbox` package, so you might prefer using `Tuple<,>` if you want to avoid referencing Stashbox in certain parts of your project.

You can also filter a collection of services by their metadata. Requesting `IEnumerable<Tuple<,>>` will yield only those services that have the given type of metadata.
<!-- div:right-panel -->

<!-- tabs:start -->
##### **Single service**
```cs
container.Register<IJob, DbBackup>(options => options
    .WithMetadata("connection-string-to-db"));

var jobWithConnectionString = container.Resolve<Metadata<IJob, string>>();
// prints: "connection-string-to-db"
Console.WriteLine(jobWithConnectionString.Data);

var alsoJobWithConnectionString = container.Resolve<Tuple<IJob, string>>();
// prints: "connection-string-to-db"
Console.WriteLine(alsoJobWithConnectionString.Item2);
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
var servicesWithStringMetadata = container.Resolve<Tuple<IService, string>[]>();

// the result is: [Service3]
var servicesWithIntMetadata = container.Resolve<Tuple<IService, int>[]>();
```

<!-- tabs:end -->

<!-- panels:end -->

?> Metadata can also be a complex type e.g., an `IDictionary<>`.

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