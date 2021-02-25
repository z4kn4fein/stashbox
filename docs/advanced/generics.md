# Generics
This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, [generic decorators](advanced/decorators?id=generic-decorators), conditions based on generic constraints, and variance.

<!-- panels:start -->

<!-- div:title-panel -->
## Closed-Generics

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


## Open-Generics

The registration of an open-generic type differs from registering a closed-generic one as C# doesn't allow the usage of open-generic types in generic method parameters. We have to get a runtime type from the open-generic type first with `typeof()`.

<!-- panels:start -->

<!-- div:left-panel -->
Open-generic types could help in such scenarios where you have a generic interface-implementation pair with numerous generic parameters. The registration of the different versions would look like this: 

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
Rather than doing that, you can register your type's open-generic version and let Stashbox create the closed ones for you. When a matching closed service type is requested, the container constructs an equivalent closed-generic implementation.

<!-- div:right-panel -->

```cs
container.Register(typeof(IValidator<>), typeof(Validator<>));
// Validator<User> will be returned.
IValidator<User> validator = container.Resolve<IValidator<User>>();
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


## Generic Constraints
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

 This section will cover those pre-defined wrapper types that Stashbox supports. The collection wrapper is used to support resolution requests with all those collection types described in the [Multiple implementations](usage/advanced-registration?id=multiple-implementations) section.

<!-- panels:start -->

<!-- div:left-panel -->
### Lazy
With requesting `Lazy<>`, the container will implicitly construct a new `Lazy<>` instance with a factory delegate as constructor argument used to instantiate the wrapped service. 

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
With requesting `Func<>`, the container will implicitly create a factory delegate used to instantiate the wrapped service. 

You have the option to request a delegate that allows supplying some or all of the dependencies as delegate parameters.
Parameters are used for sub-dependencies as well, like: `(arg) => new A(new B(arg))`

When a dependency is not covered with a parameter, the container will resolve it.

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
### Tuple
With requesting `Tuple<>`, the container will implicitly wrap the requested service types into a tuple. 

<!-- div:right-panel -->
```cs
container.Register<IJob, DbBackup>();
container.Register<ITask, EmailSending>();
// tuple is holding DbBackup and EmailSending.
Tuple<IJob, ITask> tuple = container.Resolve<Tuple<IJob, ITask>>();
```
<!-- panels:end -->

?> Wrappers can be composed like: `IEnumerable<Func<ILogger, Lazy<IJob>>>`.