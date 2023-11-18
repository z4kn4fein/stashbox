import CodeDescPanel from '@site/src/components/CodeDescPanel';
import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Registration configuration

<CodeDescPanel>
<div>

Most of the registration methods have an `Action<TOptions>` parameter, enabling several customization options on the given registration.

Here are three examples that show how the API's usage looks like. 
They cover the exact functionalities you've read about in the [basics](/docs/guides/basics) section but are achieved with the options API.

</div>
<div>

<Tabs>
<TabItem value="Named" label="Named">

This is how you can use the options API to set a registration's name:
```cs
container.Register<IJob, DbBackup>(options => options
    .WithName("DbBackup"));
```

</TabItem>
<TabItem value="Lifetime" label="Lifetime">

It was mentioned in the [Lifetime shortcuts](/docs/guides/basics#lifetime-shortcuts) section, that those methods are only sugars; under the curtain, they are also using this API:
```cs
container.Register<IJob, DbBackup>(options => options
    .WithLifetime(Lifetimes.Singleton));
```

</TabItem>
<TabItem value="Instance" label="Instance">

An example of how you can register an instance with the options API:
```cs
container.Register<IJob, DbBackup>(options => options
    .WithInstance(new DbBackup()));
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

The registration configuration API is fluent, which means all option methods can be chained after each other. 
This provides an easier way to configure complicated registrations.

</div>
<div>

```cs
container.Register<IJob, DbBackup>(options => options
    .WithName("DbBackup")
    .WithLifetime(Lifetimes.Singleton)
    .WithoutDisposalTracking());
```

</div>
</CodeDescPanel>

## General options

<CodeDescPanel>
<div>

### `WithName`
Sets the name identifier of the registration.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(config => config
    .WithName("Console"));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithInstance`
Sets an existing instance for the registration. 

Passing true for the `wireUp` parameter means that the container performs member / method injection on the registered instance.

</div>
<div>

<Tabs>
<TabItem value="Instance" label="Instance">

```cs
container.Register<ILogger>(options => options
    .WithInstance(new ConsoleLogger()));
```

</TabItem>
<TabItem value="WireUp" label="WireUp">

```cs
container.Register<ILogger>(options => options
    .WithInstance(new ConsoleLogger(), wireUp: true));
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithoutDisposalTracking`
Force disables the disposal tracking on the registration.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .WithoutDisposalTracking());
```

</div>
</CodeDescPanel>


<CodeDescPanel>
<div>

### `WithMetadata`
Sets additional metadata for the registration. It's attached to the service upon its resolution through `ValueTuple<,>`, `Tuple<,>`, or `Metadata<,>` wrappers.

</div>
<div>

```cs
container.Register<IJob, DbBackup>(options => options
    .WithMetadata(connectionString));

var jobWithConnectionString = container.Resolve<ValueTuple<IJob, string>>();
Console.WriteLine(jobWithConnectionString.Item2); // prints the connection string.

```

</div>
</CodeDescPanel>


<CodeDescPanel>
<div>

### `WithDynamicResolution`
Indicates that the service's resolution should be handled by a dynamic `Resolve()` call on the current `IDependencyResolver` instead of a pre-built instantiation expression.

</div>
<div>

```cs
container.Register<IJob, DbBackup>();
container.Register<ILogger, ConsoleLogger>(options => options
    .WithDynamicResolution());

// new DbBackup(currentScope.Resolve<ILogger>());
var job = container.Resolve<IJob>();

```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `HasServiceType`
Used to build conditions based on service type in batch/assembly registrations.
It determines whether the registration is mapped to the given service type.

</div>
<div>

```cs
container.RegisterAssemblyContaining<IService1>(configurator: options =>
    {
        if (options.HasServiceType<IService2>())
            options.WithScopedLifetime();
    });

```

</div>
</CodeDescPanel>

## Initializer / finalizer
<CodeDescPanel>
<div>

### `WithFinalizer`
Sets a custom cleanup delegate that will be invoked when the scope / container holding the instance is being disposed.

</div>
<div>

```cs
container.Register<ILogger, FileLogger>(options => options
    .WithFinalizer(logger => logger
        .CloseFile()));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithInitializer`
Sets a custom initializer delegate that will be invoked when the given service is being instantiated.

</div>
<div>

```cs
container.Register<ILogger, FileLogger>(options => options
    .WithInitializer((logger, resolver) => logger
        .OpenFile()));
```

</div>
</CodeDescPanel>


## Replace

<Tabs>
<TabItem value="ReplaceExisting" label="ReplaceExisting">

Indicates whether the container should replace an existing registration with the current one (based on [implementation type](/docs/getting-started/glossary#service-type--implementation-type) and name). If there's no existing registration in place, the actual one will be added to the registration list.
```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .ReplaceExisting());
```

</TabItem>
<TabItem value="ReplaceOnlyIfExists" label="ReplaceOnlyIfExists">

The same as `ReplaceExisting()` except that the container will do the replace only when there's an already [registered service](/docs/getting-started/glossary#service-registration--registered-service) with the same type or name.
```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .ReplaceOnlyIfExists());
```

</TabItem>
</Tabs>

## Multiple services
You can read more about binding a registration to multiple services [here](/docs/guides/advanced-registration#binding-to-multiple-services).
<CodeDescPanel>
<div>

### `AsImplementedTypes`
The service will be mapped to all of its implemented interfaces and base types.

</div>
<div>

```cs
container.Register<IUserRepository, UserRepository>(options => options
    .AsImplementedTypes());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `AsServiceAlso`
Binds the currently configured registration to an additional [service type](/docs/getting-started/glossary#service-type--implementation-type). The registered type must implement or extend the additional [service type](/docs/getting-started/glossary#service-type--implementation-type).

</div>
<div>

```cs
container.Register<IUserRepository, UserRepository>(options => options
    .AsServiceAlso<IRepository>()
    // or
    .AsServiceAlso(typeof(IRepository)));
```

</div>
</CodeDescPanel>

## Dependency configuration
These options allows the same configuration functionality as the [dependency attribute](/docs/guides/service-resolution#attributes). 

<Tabs>
<TabItem value="By parameter type" label="By parameter type">

Binds a constructor / method parameter or a property / field to a named registration by the parameter's type. The container will perform a [named resolution](/docs/getting-started/glossary#named-resolution) on the bound dependency. The second parameter used to set the name of the dependency.
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithDependencyBinding(typeof(ILogger), "FileLogger"));
```

</TabItem>
<TabItem value="By parameter name" label="By parameter name">

Binds a constructor / method parameter or a property / field to a named registration by the parameter's name. The container will perform a [named resolution](/docs/getting-started/glossary#named-resolution) on the bound dependency. The second parameter used to set the name of the dependency.
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithDependencyBinding("logger", "FileLogger"));
```

</TabItem>
<TabItem value="By expression" label="By expression">

Marks a member (property / field) as a dependency that should be filled by the container. The second parameter used to set the name of the dependency.
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithDependencyBinding(logger => logger.Logger, "ConsoleLogger"));
```

</TabItem>
</Tabs>

## Lifetime
You can read more about lifetimes [here](/docs/guides/lifetimes).

<CodeDescPanel>
<div>

### `WithSingletonLifetime`
Sets a singleton lifetime for the registration.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(config => config
    .WithSingletonLifetime());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithScopedLifetime`
Sets a scoped lifetime for the registration.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(config => config
    .WithScopedLifetime());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithPerScopedRequestLifetime`
Sets the lifetime to `PerScopedRequestLifetime`. This lifetime will create a new instance between scoped services. This means that every scoped service will get a different instance but within their dependency tree it will behave as a singleton.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .WithPerScopedRequestLifetime());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithPerRequestLifetime`
Sets the lifetime to `PerRequestLifetime`. This lifetime will create a new instance between resolution requests. Within the request the same instance will be re-used.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .WithPerRequestLifetime());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithAutoLifetime`
Sets the lifetime to auto lifetime. This lifetime aligns to the lifetime of the resolved service's dependencies. When the underlying service has a dependency with a higher lifespan, this lifetime will inherit that lifespan up to a given boundary.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .WithAutoLifetime(Lifetimes.Scoped /* boundary lifetime */));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithLifetime`
Sets a custom lifetime for the registration.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(config => config
    .WithLifetime(new CustomLifetime()));
```

</div>
</CodeDescPanel>

## Conditions
You can read more about the concept of conditional resolution [here](/docs/guides/service-resolution#conditional-resolution).

<CodeDescPanel>
<div>

### `WhenHas`
Sets an attribute condition for the registration.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(config => config
    .WhenHas<ConsoleAttribute>());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WhenResolutionPathHas`
Sets a resolution path condition for the registration. The service will be selected only in the resolution path of the target that has the given attribute.
This means that only the direct and sub-dependencies of the target type that has the given attribute will get the configured service.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(config => config
    // Each direct and sub-dependency of any service that has
    // a ConsoleAttribute will get FileLogger wherever they 
    // depend on ILogger. 
    .WhenResolutionPathHas<ConsoleAttribute>());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WhenDependantIs`
Sets a parent target condition for the registration.

</div>
<div>

```cs
container.Register<ILogger, FileLogger>(config => config
    .WhenDependantIs<UserRepository>());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WhenInResolutionPathOf`
Sets a resolution path condition for the registration. The service will be selected only in the resolution path of the given target.
This means that only the direct and sub-dependencies of the target type will get the configured service.

</div>
<div>

```cs
container.Register<ILogger, FileLogger>(config => config
    // Each direct and sub-dependency of UserRepository
    // will get FileLogger wherever they depend on ILogger. 
    .WhenInResolutionPathOf<UserRepository>());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `When`
Sets a custom user-defined condition for the registration.

</div>
<div>

```cs
container.Register<ILogger, FileLogger>(config => config
    .When(typeInfo => typeInfo.ParentType == typeof(UserRepository)));
```

</div>
</CodeDescPanel>


## Constructor selection
<CodeDescPanel>
<div>

### `WithConstructorSelectionRule`
Sets the constructor selection rule for the registration.

</div>
<div>

```cs
container.Register<ILogger>(options => options
    .WithConstructorSelectionRule(...));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

#### PreferMostParameters
Selects the constructor which has the longest parameter list.

</div>
<div>

```cs
options.WithConstructorSelectionRule(
    Rules.ConstructorSelection.PreferMostParameters)
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

#### PreferLeastParameters
Selects the constructor which has the shortest parameter list.

</div>
<div>

```cs
options.WithConstructorSelectionRule(
    Rules.ConstructorSelection.PreferLeastParameters)
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

#### Custom
You can set your own custom constructor ordering logic.

</div>
<div>

```cs
options.WithConstructorSelectionRule(
    constructors => { /* custom constructor sorting logic */ })
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithConstructorByArgumentTypes`
Selects a constructor by its argument types.

</div>
<div>

```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithConstructorByArgumentTypes(typeof(ILogger)));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithConstructorByArguments`
Selects a constructor by its arguments to use during resolution. These arguments are used to invoke the selected constructor.

</div>
<div>

```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithConstructorByArguments(new ConsoleLogger()));
```

</div>
</CodeDescPanel>



## Property / field Injection
<CodeDescPanel>
<div>

### `WithAutoMemberInjection`
Enables the auto member injection and sets the rule for it.

</div>
<div>

```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithAutoMemberInjection(...));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

#### PropertiesWithPublicSetter
With this flag, the container will perform auto-injection on properties with a public setter.

</div>
<div>

```cs
options.WithAutoMemberInjection(
    Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter)
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

#### PropertiesWithLimitedAccess
With this flag, the container will perform auto-injection on properties which has a non-public setter as well.

</div>
<div>

```cs
options.WithAutoMemberInjection(
    Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess)
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

#### PrivateFields
With this flag, the container will perform auto-injection on private fields too.

</div>
<div>

```cs
options.WithAutoMemberInjection(
    Rules.AutoMemberInjectionRules.PrivateFields)
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

#### Combined rules
As these rules are [bit flags](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum#enumeration-types-as-bit-flags), you can use them combined together with bitwise logical operators.

</div>
<div>

```cs
options.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields | 
    Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter)
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

#### Member selection filter
You can pass your own member selection logic to control which members should be auto injected.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .WithAutoMemberInjection(filter: member => member.Type != typeof(ILogger)));
```

</div>
</CodeDescPanel>

:::info
Members defined with C# 11's [`required`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required) keyword are automatically injected by the container. 
:::

## Injection parameters
<CodeDescPanel>
<div>

### `WithInjectionParameters`
Sets multiple injection parameters for the registration.

</div>
<div>

```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithInjectionParameters(new KeyValuePair<string, object>("logger", new ConsoleLogger()));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithInjectionParameter`
Sets a single injection parameter for the registration.

</div>
<div>

```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithInjectionParameter("logger", new ConsoleLogger());
```

</div>
</CodeDescPanel>


## Factory
You can read more about the concept of factory registration [here](/docs/guides/advanced-registration?id=factory-registration).

<Tabs>
<TabItem value="Parameterized" label="Parameterized">

**WithFactory** - Sets a factory delegate that could take various number of pre-resolved dependencies as parameters and returns the service instance.
```cs
// 1 parameter factory
container.Register<IUserRepository, UserRepository>(options => options
    .WithFactory<ILogger>(logger => new UserRepository(logger));

// 2 parameters factory
container.Register<IUserRepository, UserRepository>(options => options
    .WithFactory<ILogger, IDbContext>((logger, context) => new UserRepository(logger, context));

// 3 parameters factory
container.Register<IUserRepository, UserRepository>(options => options
    .WithFactory<ILogger, IDbContext, IOptions>((logger, context, options) => 
        new UserRepository(logger, context, options));

// 4 parameters factory
container.Register<IUserRepository, UserRepository>(options => options
    .WithFactory<ILogger, IDbConnection, IOptions, IUserValidator>((logger, connection, options, validator) => 
        new UserRepository(logger, connection, options, validator));

// 5 parameters factory
container.Register<IUserRepository, UserRepository>(options => options
    .WithFactory<ILogger, IDbConnection, IOptions, IUserValidator, IPermissionManage>(
        (logger, connection, options, validator, permissionManager) => 
            new UserRepository(logger, connection, options, validator, permissionManager));
```
You can also get the current [dependency resolver](/docs/getting-started/glossary#dependency-resolver) as a pre-resolved parameter:
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithFactory<ILogger, IDependencyResolver>((logger, resolver) => 
        new UserRepository(logger, resolver.Resolve<IDbConnection>())));
```

</TabItem>
<TabItem value="Parameter-less" label="Parameter-less">

**WithFactory** - Sets a parameter-less factory delegate that returns the service instance.
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithFactory(()) => new UserRepository(new ConsoleLogger()));
```

</TabItem>
<TabItem value="Resolver parameter" label="Resolver parameter">

**WithFactory** - Sets a factory delegate that takes an `IDependencyResolver` as parameter and returns the service instance.
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithFactory(resolver => new UserRepository(resolver.Resolve<ILogger>()));
```

</TabItem>
</Tabs>

:::info
All factory configuration method has an `isCompiledLambda` parameter which should be set to `true` if the passed delegate is compiled from an `Expression` tree.
:::

## Scope definition
You can read more about the concept of defined scopes [here](/docs/guides/scopes?id=service-as-scope).
<CodeDescPanel>
<div>

### `InNamedScope`
Sets a scope name condition for the registration; it will be used only when a scope with the same name requests it.

</div>
<div>

```cs
container.Register<IUserRepository, UserRepository>(options => options
    .InNamedScope("UserRepo"));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `InScopeDefinedBy`
Sets a condition for the registration; it will be used only within the scope defined by the given type.

</div>
<div>

```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .InScopeDefinedBy<UserRepository>());
container.Register<IUserRepository, UserRepository>(options => options
    .DefinesScope());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `DefinesScope`
This registration is used as a logical scope for it's dependencies. Dependencies registered with `InNamedScope()` with the same name are preferred during resolution. 
When the `name` is not set, the [service type](/docs/getting-started/glossary#service-type--implementation-type) is used as the name. Dependencies registered with `InScopeDefinedBy()` are selected.

</div>
<div>

```cs
container.Register<IUserRepository, UserRepository>(options => options
    .DefinesScope("UserRepo"));

// or
container.Register<IUserRepository, UserRepository>(options => options
    .DefinesScope());
```

</div>
</CodeDescPanel>


## Decorator specific
You can read more about decorators [here](/docs/advanced/decorators).
<CodeDescPanel>
<div>

### `WhenDecoratedServiceIs`
Sets a decorated target condition for the registration.

</div>
<div>

```cs
container.RegisterDecorator<ILogger, LoggerDecorator>(options => options
    .WhenDecoratedServiceIs<FileLogger>());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WhenDecoratedServiceHas`
Sets an attribute condition that the decorated target has to satisfy.

</div>
<div>

```cs
container.RegisterDecorator<ILogger, LoggerDecorator>(options => options
    .WhenDecoratedServiceHas<DetailedLoggingAttribute>());
```

</div>
</CodeDescPanel>


## Unknown registration specific
You can read more about unknown type resolution [here](/docs/advanced/special-resolution-cases#unknown-type-resolution).
<CodeDescPanel>
<div>

### `SetImplementationType`
Sets the current registration's [implementation type](/docs/getting-started/glossary#service-type--implementation-type).

</div>
<div>

```cs
var container = new StashboxContainer(c => c.WithUnknownTypeResolution(config =>
{
    if (config.ServiceType == typeof(IService))
        config.SetImplementationType(typeof(Service));
}));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `Skip`
Marks the current unknown type registration as skipped.

</div>
<div>

```cs
var container = new StashboxContainer(c => c.WithUnknownTypeResolution(config =>
{
    if (config.ServiceType == typeof(IService))
        config.Skip();
}));
```

</div>
</CodeDescPanel>
