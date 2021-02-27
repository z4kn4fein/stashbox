# Registration Configuration

<!-- panels:start -->

<!-- div:left-panel -->
Most of the registration methods have an `Action<TOptions>` parameter, enabling several customization options on the given registration.

Here are three examples that show how the API's usage looks like.
They cover the same functionalities you read about in the [basics](registration/basics) section, 
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
In the [Lifetime shortcuts](registration/basics?id=lifetime-shortcuts) section, I mentioned that those methods are only sugars; under the curtain, they are also using this API:
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

## General Options
- **WithName(object name)** - Sets the name of the registration. 
  ```cs
  container.Register<ILogger, ConsoleLogger>(config => config.WithName("Console"));
  ```

  ?> Allows to resolve a service by its name like: `container.Resolve<ILogger>("Console")`

- **WithInstance(object instance, bool wireUp = false)** - Sets an existing instance for the registration.
  ```cs
  container.Register<ILogger>(options => options.WithInstance(new ConsoleLogger()));
  ```

  ?> Passing true for the `wireUp` parameter means that the container will perform member and method injection on the registered instance.
    `options.WithInstance(new ConsoleLogger(), wireUp: true)`

- **WithoutDisposalTracking()** - Force disables the disposal tracking on the registration.
  ```cs
  container.Register<ILogger, ConsoleLogger>(options => options.WithoutDisposalTracking());
  ```

## Initializer / Finalizer
- **WithFinalizer(Action<TService> finalizer)** - Sets a custom cleanup delegate that will be invoked when the scope holding the instance is being disposed.
  ```cs
  container.Register<ILogger, FileLogger>(options => options.WithFinalizer(logger => logger.CloseFile()));
  ```
- **WithInitializer(Action<TService, IDependencyResolver> initializer)** - Sets a custom initializer delegate that will be invoked when the given service is being resolved.
  ```cs
  container.Register<ILogger, FileLogger>(options => options.WithInitializer((logger, resolver) => logger.OpenFile()));
  ```

## Replace
- **ReplaceExisting()** - Indicates that the container should replace an existing registration with the current one (based on implementation type and name). If there's no existing registration in place, the actual one will be added to the registration list.
  ```cs
  container.Register<ILogger, ConsoleLogger>(options => options.ReplaceExisting());
  ```
- **ReplaceOnlyIfExists()** - The same as `ReplaceExisting()` except that the container will do the replace only when there's an already registered service with the same type or name.
  ```cs
  container.Register<ILogger, ConsoleLogger>(options => options.ReplaceOnlyIfExists());
  ```

## Multiple services
- **AsImplementedTypes()** - The service will be mapped to all of its implemented interfaces and base types.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options.AsImplementedTypes());
  ```
- **AsServiceAlso()** - Binds the currently configured registration to an additional service type.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options.AsServiceAlso<IRepository>());
  ```

  ?> The registered type must implement or extend the additional service type.

## Dependency Configuration
- **WithDependencyBinding(Type dependencyType, object dependencyName)** - Binds a constructor/method parameter or a property/field to a named registration by the parameter's type. The container will perform a named resolution on the bound dependency.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options.WithDependencyBinding(typeof(ILogger), "FileLogger"));
  ```

- **WithDependencyBinding(string parameterName, object dependencyName)** - Binds a constructor/method parameter or a property/field to a named registration by the parameter's name. The container will perform a named resolution on the bound dependency.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options.WithDependencyBinding("logger", "FileLogger"));
  ```

- **WithDependencyBinding(Expression propertyAccessor, object dependencyName = null)** - Marks a member (property/field) as a dependency that should be filled by the container.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options.WithDependencyBinding(logger => logger.Logger, "ConsoleLogger"));
  ```

  ?> The second parameter used to set the name of the dependency. 


## Lifetime
- **WithLifetime(ILifetime lifetime)** - Sets a custom lifetime for the registration.
  ```cs
  container.Register<ILogger, ConsoleLogger>(config => config.WithLifetime(new CustomLifetime()));
  ```
- **WithSingletonLifetime()** - Sets a singleton lifetime for the registration.
  ```cs
  container.Register<ILogger, ConsoleLogger>(config => config.WithSingletonLifetime());
  ```
- **WithScopedLifetime()** - Sets a scoped lifetime for the registration.
  ```cs
  container.Register<ILogger, ConsoleLogger>(config => config.WithScopedLifetime());
  ```
- **WithPerScopedRequestLifetime()** - Sets the lifetime to `PerScopedRequestLifetime`. That means this registration will behave like a singleton within every scoped resolution request.
  ```cs
  container.Register<ILogger, ConsoleLogger>(options => options.WithPerScopedRequestLifetime());
  ```

## Conditions
- **WhenHas(Type attributeType)** - Sets an attribute condition for the registration.
  ```cs
  container.Register<ILogger, ConsoleLogger>(config => config.WhenHas(typeof(ConsoleAttribute)));
  ```
- **WhenHas<TAttribute>()** - Sets an attribute condition for the registration.
  ```cs
  container.Register<ILogger, ConsoleLogger>(config => config.WhenHas<ConsoleAttribute>());
  ```
- **WhenDependantIs(Type targetType)** - Sets a parent target condition for the registration.
  ```cs
  container.Register<ILogger, FileLogger>(config => config.WhenDependantIs(typeof(UserRepository)));
  ```
- **WhenDependantIs<TTarget>()** - Sets a parent target condition for the registration.
  ```cs
  container.Register<ILogger, FileLogger>(config => config.WhenDependantIs<UserRepository>());
  ```
- **When(Func<TypeInformation, bool> resolutionCondition)** - Sets a custom user-defined condition for the registration.  
  ```cs
  container.Register<ILogger, FileLogger>(config => config
      .When(typeInfo => typeInfo.ParentType == typeof(UserRepository)));
  ```

## Constructor Selection
- **WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> rule)** - Sets the constructor selection rule for the registration.
  ```cs
  container.Register<ILogger, ConsoleLogger>(options => options.WithConstructorSelectionRule());
  ```
  Custom constructor selection rules:
  - *PreferMostParameters* - Prefers the constructor which has the most extended parameter list.
    ```cs
    options.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters)
    ```
  - *PreferLeastParameters* - Prefers the constructor which has the shortest parameter list.
    ```cs
    options.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters)
    ```
  - *Custom*
    ```cs
    options.WithConstructorSelectionRule(constructors => { /* custom constructor sorting logic */ })
    ```
- **WithConstructorByArgumentTypes(params Type[] argumentTypes)** - Selects a constructor by its argument types to use during the resolution.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options
      .WithConstructorByArgumentTypes(typeof(ILogger)));
  ```
- **WithConstructorByArguments(params object[] arguments)** - Selects a constructor by its arguments to use during resolution. These arguments will be used to invoke the selected constructor.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options
      .WithConstructorByArguments(new ConsoleLogger()));
  ```

## Property/Field Injection
- **WithAutoMemberInjection(AutoMemberInjectionRules rule)** - Enables the auto member injection and sets the rule for it.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options.WithAutoMemberInjection(...));
  ```
  Custom member injection rules:
  - *PropertiesWithPublicSetter* - With this flag, the container will perform auto-injection on properties with a public setter.
    ```cs
    options.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter)
    ```
  - *PropertiesWithLimitedAccess* - With this flag, the container will perform auto-injection on properties which has a non-public setter as well.
    ```cs
    options.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess)
    ```
  - *PrivateFields* - With this flag, the container will perform auto-injection on private fields too.
    ```cs
    options.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields)
    ```
  - Combined rules:
    ```cs
    options.WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields | 
        Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter)
    ```
  
  Member selection filter:
  ```cs
  container.Register<ILogger, ConsoleLogger>(options => options
        .WithAutoMemberInjection(filter: member => member.Type != typeof(ILogger)));
  ```

  ?> With the filter above, the container will exclude all the class members with the type `ILogger` from auto-injection.

## Injection Parameters
- **WithInjectionParameters(params KeyValuePair<string, object>[] injectionParameters)** - Sets injection parameters for the registration.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => 
      options.WithInjectionParameters(new KeyValuePair<string, object>("logger", new ConsoleLogger()));
  ```
- **WithInjectionParameter(string name, object value)** - Sets an injection parameter for the registration.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options.WithInjectionParameter("logger", new ConsoleLogger());
  ```

## Factory
- **WithFactory(Func&lt;object&gt; factory)** - Sets a delegate factory that returns an object used to instantiate the service.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options
      .WithFactory(resolver => new UserRepository(resolver.Resolve<ILogger>()));
  ```

- **WithFactory(Func&lt;TImplementation&gt; factory)** - Sets a delegate factory that returns an object with the registered implementation type used to instantiate the service.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options
      .WithFactory(resolver => new UserRepository(resolver.Resolve<ILogger>()));
  ```

- **WithFactory(Func&lt;IDependencyResolver, object&gt; factory)** - Sets a delegate factory that gets a dependency resolver parameter and returns an object used to instantiate the service.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options
      .WithFactory(resolver => new UserRepository(resolver.Resolve<ILogger>()));
  ```

- **WithFactory(Func&lt;IDependencyResolver, TImplementation&gt; factory)** - Sets a delegate factory that gets a dependency resolver parameter and returns an object with the registered implementation type used to instantiate the service.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options
      .WithFactory(resolver => new UserRepository(resolver.Resolve<ILogger>()));
  ```

- **WithFactory&lt;TParam1&gt;(Func&lt;TParam1, TImplementation&gt; factory)** - Sets a delegate factory that gets a pre-resolved parameter and returns an object with the registered implementation type used to instantiate the service.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options
      .WithFactory<ILogger>(logger => new UserRepository(logger));
  ```

- **WithFactory&lt;TParam1, TParam2&gt;(Func&lt;TParam1, TParam2, TImplementation&gt; factory)** - Sets a delegate factory that gets two pre-resolved parameters and returns an object with the registered implementation type used to instantiate the service.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options
      .WithFactory<ILogger, IDbContext>((logger, context) => new UserRepository(logger, context));
  ```

- **WithFactory&lt;TParam1, TParam2, TParam3&gt;(Func&lt;TParam1, TParam2, TParam3, TImplementation&gt; factory)** - Sets a delegate factory that gets three pre-resolved parameters and returns an object with the registered implementation type used to instantiate the service.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options
      .WithFactory<ILogger, IDbContext, IOptions>((logger, context, options) => 
          new UserRepository(logger, context, options));
  ```

- **WithFactory&lt;TParam1, TParam2, TParam3, TParam4&gt;(Func&lt;TParam1, TParam2, TParam3, TParam4, TImplementation&gt; factory)** - Sets a delegate factory that gets four pre-resolved parameters and returns an object with the registered implementation type used to instantiate the service.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options
      .WithFactory<ILogger, IDbConnection, IOptions, IUserValidator>((logger, connection, options, validator) => 
          new UserRepository(logger, connection, options, validator));
  ```

- **WithFactory&lt;TParam1, TParam2, TParam3, TParam4, TParam5&gt;(Func&lt;TParam1, TParam2, TParam3, TParam4, TParam5, TImplementation&gt; factory)** - Sets a delegate factory that gets five pre-resolved parameters and returns an object with the registered implementation type used to instantiate the service.
  ```cs
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

?> All factory configuration method has an `isCompiledLambda` parameter which should be set to `true` if the passed delegate is compiled from an `Expression` tree.

## Scope Definition
- **InNamedScope(object scopeName)** - Sets a scope name condition for the registration; it will be used only when a scope with the same name requests it.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options.InNamedScope("UserRepo"));
  ```
- **DefinesScope(object scopeName)** - This registration would be used as a logical scope for its dependencies. The dependencies registered with the `InNamedScope()` setting with the same name are preferred during the resolution.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options.DefinesScope("UserRepo"));
  ```
- **InScopeDefinedBy<IScopeDefiner>()** - Sets a condition for the registration that it will be used only within the scope defined by the given type.
  ```cs
  container.Register<ILogger, ConsoleLogger>(options => options.InScopeDefinedBy<UserRepository>());
  container.Register<IUserRepository, UserRepository>(options => options.DefinesScope());
  ```
- **DefinesScope()** - This registration would be used as a logical scope for its dependencies, the dependencies bound to the current registration by the `InScopeDefinedBy()` are preferred during the resolution.
  ```cs
  container.Register<IUserRepository, UserRepository>(options => options.DefinesScope());
  ```