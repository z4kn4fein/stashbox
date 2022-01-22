# Registration configuration

<!-- panels:start -->

<!-- div:left-panel -->
Most of the registration methods have an `Action<TOptions>` parameter, enabling several customization options on the given registration.

Here are three examples that show how the API's usage looks like.
They cover the same functionalities you read about in the [basics](usage/basics) section, 
but achieved with the options API.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Named**
This is how you can use the options API to set a registration's name:
```cs
container.Register<IJob, DbBackup>(options => options
    .WithName("DbBackup"));
```
#### **Lifetime**
In the [Lifetime shortcuts](usage/basics?id=lifetime-shortcuts) section, it was mentioned that those methods are only sugars; under the curtain, they are also using this API:
```cs
container.Register<IJob, DbBackup>(options => options
    .WithLifetime(Lifetimes.Singleton));
```
#### **Instance**
This is an example of how you can register an instance with the options API:
```cs
container.Register<IJob, DbBackup>(options => options
    .WithInstance(new DbBackup()));
```
<!-- tabs:end -->

<br>
<!-- div:left-panel -->
The options API is fluent, which means all the related option methods can be chained after each other. 
This nature enables an easier way of configuring complicated registrations.

<!-- div:right-panel -->

```cs
container.Register<IJob, DbBackup>(options => options
    .WithName("DbBackup")
    .WithLifetime(Lifetimes.Singleton)
    .WithoutDisposalTracking());
```

<!-- panels:end -->

## General options
<!-- panels:start -->
<!-- div:left-panel -->
### `WithName`
Sets the name of the registration.
<!-- div:right-panel -->
```cs
container.Register<ILogger, ConsoleLogger>(config => config
    .WithName("Console"));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `WithInstance`
Sets an existing instance for the registration. 

Passing true for the `wireUp` parameter means that the container performs member / method injection on the registered instance.
<!-- div:right-panel -->
<!-- tabs:start -->
#### **Instance**
```cs
container.Register<ILogger>(options => options
    .WithInstance(new ConsoleLogger()));
```
#### **WireUp**
```cs
container.Register<ILogger>(options => options
    .WithInstance(new ConsoleLogger(), wireUp: true));
```
<!-- tabs:end -->
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `WithoutDisposalTracking`
Force disables the disposal tracking on the registration.
<!-- div:right-panel -->
```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .WithoutDisposalTracking());
```
<!-- panels:end -->

## Initializer / finalizer
<!-- panels:start -->
<!-- div:left-panel -->
### `WithFinalizer`
Sets a custom cleanup delegate that will be invoked when the scope / container holding the instance is being disposed.
<!-- div:right-panel -->
```cs
container.Register<ILogger, FileLogger>(options => options
    .WithFinalizer(logger => logger
        .CloseFile()));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `WithInitializer`
Sets a custom initializer delegate that will be invoked when the given service is being instantiated.
<!-- div:right-panel -->
```cs
container.Register<ILogger, FileLogger>(options => options
    .WithInitializer((logger, resolver) => logger
        .OpenFile()));
```
<!-- panels:end -->

## Replace
<!-- tabs:start -->
#### **ReplaceExisting**
Indicates that the container should replace an existing registration with the current one (based on implementation type and name). If there's no existing registration in place, the actual one will be added to the registration list.
```cs
container.Register<ILogger, ConsoleLogger>(options => options.ReplaceExisting());
```
#### **ReplaceOnlyIfExists**
The same as `ReplaceExisting()` except that the container will do the replace only when there's an already registered service with the same type or name.
```cs
container.Register<ILogger, ConsoleLogger>(options => options.ReplaceOnlyIfExists());
```
<!-- tabs:end -->

## Multiple services
You can read more about binding a registration to multiple services [here](usage/advanced-registration?id=binding-to-multiple-services).
<!-- panels:start -->
<!-- div:left-panel -->
### `AsImplementedTypes`
The service will be mapped to all of its implemented interfaces and base types.
<!-- div:right-panel -->
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .AsImplementedTypes());
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `AsServiceAlso`
Binds the currently configured registration to an additional service type. The registered type must implement or extend the additional service type.
<!-- div:right-panel -->
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .AsServiceAlso<IRepository>()
    // or
    .AsServiceAlso(typeof(IRepository)));
```
<!-- panels:end -->

## Dependency configuration
These options allows the same configuration functionality as the [dependency attribute](usage/service-resolution?id=attributes). 
<!-- tabs:start -->
#### **By parameter type**
Binds a constructor / method parameter or a property / field to a named registration by the parameter's type. The container will perform a named resolution on the bound dependency. The second parameter used to set the name of the dependency.
```cs
container.Register<IUserRepository, UserRepository>(options => options.WithDependencyBinding(typeof(ILogger), "FileLogger"));
```
#### **By parameter name**
Binds a constructor / method parameter or a property / field to a named registration by the parameter's name. The container will perform a named resolution on the bound dependency. The second parameter used to set the name of the dependency.
```cs
container.Register<IUserRepository, UserRepository>(options => options.WithDependencyBinding("logger", "FileLogger"));
```
#### **By expression**
Marks a member (property / field) as a dependency that should be filled by the container. The second parameter used to set the name of the dependency.
```cs
container.Register<IUserRepository, UserRepository>(options => options.WithDependencyBinding(logger => logger.Logger, "ConsoleLogger"));
```
<!-- tabs:end -->

## Lifetime
You can read more about lifetimes [here](usage/lifetimes).
<!-- panels:start -->
<!-- div:left-panel -->
### `WithSingletonLifetime`
Sets a singleton lifetime for the registration.
<!-- div:right-panel -->
```cs
container.Register<ILogger, ConsoleLogger>(config => config
    .WithSingletonLifetime());
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `WithScopedLifetime`
Sets a scoped lifetime for the registration.
<!-- div:right-panel -->
```cs
container.Register<ILogger, ConsoleLogger>(config => config
    .WithScopedLifetime());
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `WithPerScopedRequestLifetime`
Sets the lifetime to `PerScopedRequestLifetime`. That means this registration will behave like a singleton within every scoped resolution request.
<!-- div:right-panel -->
```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .WithPerScopedRequestLifetime());
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `WithLifetime`
Sets a custom lifetime for the registration.
<!-- div:right-panel -->
```cs
container.Register<ILogger, ConsoleLogger>(config => config
    .WithLifetime(new CustomLifetime()));
```
<!-- panels:end -->

## Conditions
You can read more about the concept of conditional resolution [here](usage/service-resolution?id=conditional-resolution).
<!-- panels:start -->
<!-- div:left-panel -->
### `WhenHas`
Sets an attribute condition for the registration.
<!-- div:right-panel -->
```cs
container.Register<ILogger, ConsoleLogger>(config => config
    .WhenHas<ConsoleAttribute>());
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `WhenDependantIs`
Sets a parent target condition for the registration.
<!-- div:right-panel -->
```cs
container.Register<ILogger, FileLogger>(config => config
    .WhenDependantIs<UserRepository>());
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `When`
Sets a custom user-defined condition for the registration.
<!-- div:right-panel -->
```cs
container.Register<ILogger, FileLogger>(config => config
    .When(typeInfo => typeInfo.ParentType == typeof(UserRepository)));
```
<!-- panels:end -->

## Constructor selection
<!-- panels:start -->
<!-- div:left-panel -->
### `WithConstructorSelectionRule`
Sets the constructor selection rule for the registration.
<!-- div:right-panel -->
```cs
container.Register<ILogger>(options => options
    .WithConstructorSelectionRule(...));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
#### PreferMostParameters
Selects the constructor which has the longest parameter list.
<!-- div:right-panel -->
```cs
options.WithConstructorSelectionRule(
    Rules.ConstructorSelection.PreferMostParameters)
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
#### PreferLeastParameters
Selects the constructor which has the shortest parameter list.
<!-- div:right-panel -->
```cs
options.WithConstructorSelectionRule(
    Rules.ConstructorSelection.PreferLeastParameters)
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
#### Custom
You can set your own custom constructor ordering logic.
<!-- div:right-panel -->
```cs
options.WithConstructorSelectionRule(
    constructors => { /* custom constructor sorting logic */ })
```
<!-- panels:end -->

<!-- tabs:end -->
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `WithConstructorByArgumentTypes`
Selects a constructor by its argument types.
<!-- div:right-panel -->
```cs
container.Register<IUserRepository, UserRepository>(options => options
      .WithConstructorByArgumentTypes(typeof(ILogger)));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `WithConstructorByArguments`
Selects a constructor by its arguments to use during resolution. These arguments are used to invoke the selected constructor.
<!-- div:right-panel -->
```cs
container.Register<IUserRepository, UserRepository>(options => options
      .WithConstructorByArguments(new ConsoleLogger()));
```
<!-- panels:end -->


## Property / field Injection
<!-- panels:start -->
<!-- div:left-panel -->
### `WithAutoMemberInjection`
Enables the auto member injection and sets the rule for it.
<!-- div:right-panel -->
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithAutoMemberInjection(...));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
#### PropertiesWithPublicSetter
With this flag, the container will perform auto-injection on properties with a public setter.
<!-- div:right-panel -->
```cs
options.WithAutoMemberInjection(
    Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter)
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
#### PropertiesWithLimitedAccess
With this flag, the container will perform auto-injection on properties which has a non-public setter as well.
<!-- div:right-panel -->
```cs
options.WithAutoMemberInjection(
    Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess)
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
#### PrivateFields
With this flag, the container will perform auto-injection on private fields too.
<!-- div:right-panel -->
```cs
options.WithAutoMemberInjection(
    Rules.AutoMemberInjectionRules.PrivateFields)
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
#### Combined rules
As these rules are [bit flags](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/enum#enumeration-types-as-bit-flags), you can use them combined together with bitwise logical operators.
<!-- div:right-panel -->
```cs
options.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields | 
        Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter)
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
#### Member selection filter
You can pass your own member selection logic to control which members should be auto injected.
<!-- div:right-panel -->
```cs
container.Register<ILogger, ConsoleLogger>(options => options
        .WithAutoMemberInjection(filter: member => member.Type != typeof(ILogger)));
```
<!-- panels:end -->


## Injection parameters
<!-- panels:start -->
<!-- div:left-panel -->
### `WithInjectionParameters`
Sets multiple injection parameters for the registration.
<!-- div:right-panel -->
```cs
container.Register<IUserRepository, UserRepository>(options => 
      options.WithInjectionParameters(new KeyValuePair<string, object>("logger", new ConsoleLogger()));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `WithInjectionParameter`
Sets a single injection parameter for the registration.
<!-- div:right-panel -->
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithInjectionParameter("logger", new ConsoleLogger());
```
<!-- panels:end -->

## Factory
You can read more about the concept of factory registration [here](usage/advanced-registration?id=factory-registration).
<!-- tabs:start -->
#### **Parameterized**
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
You can also get the current dependency resolver as a pre-resolved parameter:
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .WithFactory<ILogger, IDependencyResolver>((logger, resolver) => 
          new UserRepository(logger, resolver.Resolve<IDbConnection>())));
```
#### **Parameter-less**
**WithFactory** - Sets a parameter-less factory delegate that returns the service instance.
```cs
container.Register<IUserRepository, UserRepository>(options => options
      .WithFactory(()) => new UserRepository(new ConsoleLogger()));
```
#### **Resolver parameter**
**WithFactory** - Sets a factory delegate that takes an `IDependencyResolver` as parameter and returns the service instance.
```cs
container.Register<IUserRepository, UserRepository>(options => options
      .WithFactory(resolver => new UserRepository(resolver.Resolve<ILogger>()));
```
<!-- tabs:end -->

?> All factory configuration method has an `isCompiledLambda` parameter which should be set to `true` if the passed delegate is compiled from an `Expression` tree.

## Scope definition
You can read more about the concept of defined scopes [here](usage/scopes?id=service-as-scope).
<!-- panels:start -->
<!-- div:left-panel -->
### `InNamedScope`
Sets a scope name condition for the registration; it will be used only when a scope with the same name requests it.
<!-- div:right-panel -->
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .InNamedScope("UserRepo"));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `InScopeDefinedBy`
Sets a condition for the registration; it will be used only within the scope defined by the given type.
<!-- div:right-panel -->
```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .InScopeDefinedBy<UserRepository>());
container.Register<IUserRepository, UserRepository>(options => options
    .DefinesScope());
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### `DefinesScope`
This registration is used as a logical scope for it's dependencies. Dependencies registered with `InNamedScope()` with the same name are preferred during resolution. 
When the `name` is not set, the service type is used as the name. Dependencies registered with `InScopeDefinedBy()` are selected.
<!-- div:right-panel -->
```cs
container.Register<IUserRepository, UserRepository>(options => options
    .DefinesScope("UserRepo"));

// or
container.Register<IUserRepository, UserRepository>(options => options
    .DefinesScope());
```
<!-- panels:end -->