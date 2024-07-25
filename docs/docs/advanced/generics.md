import CodeDescPanel from '@site/src/components/CodeDescPanel';
import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Generics
This section is about how Stashbox handles various usage scenarios that involve .NET Generic types. Including the registration of open-generic and closed-generic types, [generic decorators](/docs/advanced/decorators#generic-decorators), conditions based on generic constraints, and variance.

## Closed-generics

<CodeDescPanel>
<div>

The registration of a closed-generic type does not differ from registering a simple non-generic service.

You have all options available that you saw at the [basic](/docs/guides/basics) and [advanced registration](/docs/guides/advanced-registration) flows.

</div>
<div>

<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

```cs
container.Register<IValidator<User>, UserValidator>();
IValidator<User> validator = container.Resolve<IValidator<User>>();
```

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

```cs
container.Register(typeof(IValidator<User>), typeof(UserValidator));
object validator = container.Resolve(typeof(IValidator<User>));
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Open-generics

The registration of an open-generic type differs from registering a closed-generic one as C# doesn't allow the usage of open-generic types in generic method parameters. We have to get a runtime type from the open-generic type first with `typeof()`.

<CodeDescPanel>
<div>

Open-generic types could help in such scenarios where you have generic interface-implementation pairs with numerous generic parameter variations. The registration of those different versions would look like this: 

</div>
<div>

```cs
container.Register<IValidator<User>, Validator<User>>();
container.Register<IValidator<Role>, Validator<Role>>();
container.Register<IValidator<Company>, Validator<Company>>();
// and so on...
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

Rather than doing that, you can register your type's generic definition and let Stashbox bind the type parameters for you. When a matching closed [service type](/docs/getting-started/glossary#service-type--implementation-type) is requested, the container will construct an equivalent closed-generic implementation.

</div>
<div>

```cs
container.Register(typeof(IValidator<>), typeof(Validator<>));
// Validator<User> will be returned.
IValidator<User> userValidator = container.Resolve<IValidator<User>>();
// Validator<Role> will be returned.
IValidator<Role> roleValidator = container.Resolve<IValidator<Role>>();
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

A registered closed-generic type always has priority over an open-generic type at service selection.

</div>
<div>

```cs
container.Register<IValidator<User>, UserValidator>();
container.Register(typeof(IValidator<>), typeof(Validator<>));
// UserValidator will be returned.
IValidator<User> validator = container.Resolve<IValidator<User>>();
```
</div>
</CodeDescPanel>


## Generic constraints
In the following examples, you can see how the container handles generic constraints during service resolution. Constraints can be used for [conditional resolution](/docs/guides/service-resolution#conditional-resolution) including collection filters. 


<Tabs>
<TabItem value="Conditional resolution" label="Conditional resolution">

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

</TabItem>
<TabItem value="Collection filter" label="Collection filter">

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

</TabItem>
</Tabs>

## Variance
Since .NET Framework 4.0, C# supports [covariance and contravariance](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/) in generic interfaces and delegates and allows implicit conversion of generic type parameters. In this section, we'll focus on variance in generic interfaces. 

[Here](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/covariance-contravariance/creating-variant-generic-interfaces) you can read more about how to create variant generic interfaces, and the following example will show how you can use them with Stashbox.

<Tabs>
<TabItem value="Contravariance" label="Contravariance">

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

</TabItem>
<TabItem value="Covariance" label="Covariance">

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

</TabItem>
</Tabs>

:::info
The check for variant generic types is enabled by default, but it can be turned off via a [container configuration option](/docs/configuration/container-configuration#generic-variance).
:::