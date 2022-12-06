# Service resolution
When you have all your components registered and configured adequately, you can resolve them from the container or a [scope](usage/scopes) by requesting their service type.

During a service's resolution, the container walks through the entire resolution tree and instantiates all dependencies required for the service construction.
When the container encounters any violations of [these rules](diagnostics/validation?id=resolution-validation) *(circular dependencies, missing required services, lifetime misconfigurations)* during the walkthrough, it lets you know that something is wrong by throwing the appropriate exception.

<!-- panels:start -->

<!-- div:title-panel -->
## Injection patterns

<!-- div:left-panel -->
**Constructor injection** is the *primary dependency injection pattern*. It encourages the organization of the dependencies to a single place - the constructor.

Stashbox, by default, uses the constructor that has the most parameters it knows how to resolve. This behavior is configurable through *[constructor selection](configuration/registration-configuration?id=constructor-selection)*.

*[Property / field injection](configuration/registration-configuration?id=property-field-injection)* is also supported in cases where constructor injection is not applicable.

!> It's a common mistake to use the *property / field injection* only to disencumber the constructor from having too many parameters. That's a code smell and also a violation of the [Single-responsibility principle](https://en.wikipedia.org/wiki/Single-responsibility_principle). If you recognize these conditions, you might consider not adding that extra property-injected dependency into your class but instead split it into multiple smaller units. 

?> [Constructor selection](configuration/container-configuration?id=constructor-selection) and [property / field injection](configuration/container-configuration?id=auto-member-injection) is also configurable container-wide.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Constructor injection**
```cs
class DbBackup : IJob
{
    private readonly ILogger logger;
    private readonly IEventBroadcaster eventBroadcaster;

    public DbBackup(ILogger logger, IEventBroadcaster eventBroadcaster)
    {
        this.logger = logger;
        this.eventBroadcaster = eventBroadcaster;
    }
}

container.Register<ILogger, ConsoleLogger>();
container.Register<IEventBroadcaster, MessageBus>();

container.Register<IJob, DbBackup>();

// resolution using the available constructor.
IJob job = container.Resolve<IJob>();
```
#### **Property / field injection**
```cs
class DbBackup : IJob
{
    public ILogger Logger { get; set; }
    public IEventBroadcaster EventBroadcaster { get; set; }

    public DbBackup() 
    { }
}

container.Register<ILogger, ConsoleLogger>();
container.Register<IEventBroadcaster, MessageBus>();

// registration of service with auto member injection.
container.Register<IJob, DbBackup>(options => 
    options.WithAutoMemberInjection());

// resolution will inject the properties.
IJob job = container.Resolve<IJob>();
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Attributes

<!-- div:left-panel -->
Attributes give you control over how Stashbox selects dependencies for a service's resolution.

**Dependency attribute**: 
- **On a constructor / method parameter**: used with the *name* property, it works as a marker for named resolution.

- **On a property / field**: first, it enables the *auto-injection* of the marked property / field (even if it wasn't configured at registration), and just as with the method parameter, it allows named resolution.

**InjectionMethod attribute**: marks a method to be called when the requested service is being instantiated.

!> Attributes provide a more straightforward configuration, but using them also tightens the bond between your application and Stashbox. If that's an issue for you, the same functionality is available on the *registration API* as [dependency binding](usage/service-resolution?id=dependency-binding).

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Constructor**
```cs
class DbBackup : IJob
{
    private readonly ILogger logger;

    public DbBackup([Dependency("Console")]ILogger logger)
    {
        this.logger = logger;
    }
}

container.Register<ILogger, ConsoleLogger>("Console");
container.Register<ILogger, FileLogger>("File");

container.Register<IJob, DbBackup>();

// the container will resolve DbBackup with ConsoleLogger.
IJob job = container.Resolve<IJob>();
```

#### **Property / field**
```cs
class DbBackup : IJob
{
    [Dependency("Console")]
    public ILogger Logger { get; set; }

    public DbBackup() 
    { }
}

container.Register<ILogger, ConsoleLogger>("Console");
container.Register<ILogger, FileLogger>("File");

container.Register<IJob, DbBackup>();

// the container will resolve DbBackup with ConsoleLogger.
IJob job = container.Resolve<IJob>();
```

#### **Method**
```cs
class DbBackup : IJob
{
    [InjectionMethod]
    public void Initialize([Dependency("Console")]ILogger logger)
    {
        this.logger.Log("Initializing.");
    }
}

container.Register<ILogger, ConsoleLogger>("Console");
container.Register<ILogger, FileLogger>("File");

container.Register<IJob, DbBackup>();

// the container will call DbBackup's Initialize method.
IJob job = container.Resolve<IJob>();
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Dependency binding

<!-- div:left-panel -->
The same dependency configuration as attributes have is available using the registration configuration API.

**Bind to parameter**: it has the same functionality as the [Dependency attribute](usage/service-resolution?id=attributes) on a constructor or method parameter, enables the named resolution.

**Bind to property / field**: it has the same functionality as the [Dependency attribute](usage/service-resolution?id=attributes); it enables the injection of the given property / field.

?> There are more overloads [available](configuration/registration-configuration?id=dependency-configuration).

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Bind to parameter**
```cs
class DbBackup : IJob
{
    public DbBackup(ILogger logger)
    { }
}

container.Register<ILogger, ConsoleLogger>("Console");
container.Register<ILogger, FileLogger>("File");

// registration of service with the dependency binding.
container.Register<IJob, DbBackup>(options => options
    .WithDependencyBinding("logger", "Console"));

// the container will resolve DbBackup with ConsoleLogger.
IJob job = container.Resolve<IJob>();
```

#### **Bind to property / field**
```cs
class DbBackup : IJob
{
    public ILogger Logger { get; set; }
}

container.Register<ILogger, ConsoleLogger>("Console");
container.Register<ILogger, FileLogger>("File");

// registration of service with the member injection.
container.Register<IJob, DbBackup>(options => options
    .WithDependencyBinding("Logger", "Console"));

// the container will resolve DbBackup with ConsoleLogger.
IJob job = container.Resolve<IJob>();
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Conventional resolution

<!-- div:left-panel -->
When you enable the conventional resolution, the container treats the member and method parameter names as their dependency identifier. 

It's like an implicit dependency binding on every class member.

First, you have to enable the conventional resolution through the configuration of the container:  
```cs
new StashboxContainer(options => options
    .TreatParameterAndMemberNameAsDependencyName());
```

?> The container will attempt named resolution on every dependency based on parameter or property / field name.

<!-- div:right-panel -->


<!-- tabs:start -->
#### **Parameters**
```cs
class DbBackup : IJob
{
    public DbBackup(
        // the parameter name identifies the dependency.
        ILogger consoleLogger)
    { }
}

container.Register<ILogger, ConsoleLogger>("consoleLogger");
container.Register<ILogger, FileLogger>("fileLogger");

container.Register<IJob, DbBackup>();

// the container will resolve DbBackup with ConsoleLogger.
IJob job = container.Resolve<IJob>();
```

#### **Properties / fields**
```cs
class DbBackup : IJob
{
    // the property name identifies the dependency.
    public ILogger ConsoleLogger { get; set; }
}

container.Register<ILogger, ConsoleLogger>("ConsoleLogger");
container.Register<ILogger, FileLogger>("FileLogger");

// registration of service with auto member injection.
container.Register<IJob, DbBackup>(options => options
    .WithAutoMemberInjection());

// the container will resolve DbBackup with ConsoleLogger.
IJob job = container.Resolve<IJob>();
```
<!-- tabs:end -->

<!-- panels:end -->


<!-- panels:start -->

<!-- div:title-panel -->
## Conditional resolution

<!-- div:left-panel -->
Stashbox can resolve a particular dependency based on its context. This context is typically the reflected type information of the dependency, its usage, and the type it gets injected into.

- **Attribute**: you can filter on constructor, method, property, or field attributes to select the desired dependency for your service. In contrast to the `Dependency` attribute, this configuration method doesn't tie your application to Stashbox because you can use your attributes.

- **Parent type**: you can filter on what type the given service is injected into.

- **Resolution path**: similar to the parent type and attribute condition but extended with inheritance. You can set that the given service is only usable in a type's resolution path. This means that each direct and sub-dependency of the selected type must use the given service as dependency.

- **Custom**: with this, you can build your own selection logic based on the passed contextual type information.

The specified conditions are behaving like filters when a **collection** is requested.

When you use the same conditional option multiple times, the container will evaluate them **combined with OR** logical operator.

?> [Here](configuration/registration-configuration?id=conditions) you can find each condition related registration option.

<!-- div:right-panel -->


<!-- tabs:start -->
#### **Attribute**
```cs
class ConsoleAttribute : Attribute { }

class DbBackup : IJob
{
    public DbBackup([Console]ILogger logger)
    { }
}

container.Register<ILogger, ConsoleLogger>(options => options
    // resolve only when the injected parameter, 
    // property or field has the 'Console' attribute
    .WhenHas<ConsoleAttribute>());

container.Register<IJob, DbBackup>();

// the container will resolve DbBackup with ConsoleLogger.
IJob job = container.Resolve<IJob>();
```

#### **Parent**
```cs
class DbBackup : IJob
{
    public DbBackup(ILogger logger)
    { }
}

container.Register<ILogger, ConsoleLogger>(options => options
    // inject only when we are 
    // currently resolving DbBackup
    .WhenDependantIs<DbBackup>());

container.Register<IJob, DbBackup>();

// the container will resolve DbBackup with ConsoleLogger.
IJob job = container.Resolve<IJob>();
```

#### **Resolution path**
```cs
class DbBackup : IJob
{
    public DbBackup(IStorage storage)
    { }
}

class FileStorage : IStorage
{
    public FileStorage(ILogger logger) 
    { }
}

container.Register<ILogger, ConsoleLogger>(options => options
    // inject only when we are in the
    // resolution path of DbBackup
    .WhenInResolutionPathOf<DbBackup>());

container.Register<IStorage, FileStorage>();
container.Register<IJob, DbBackup>();

// the container will select ConsoleLogger for FileStorage
// because they are injected into DbBackup.
IJob job = container.Resolve<IJob>();
```

#### **Custom**
```cs
class DbBackup : IJob
{
    public DbBackup(ILogger logger)
    { }
}

container.Register<ILogger, ConsoleLogger>(options => options
    // inject only when we are 
    // currently resolving DbBackup.
    .When(typeInfo => typeInfo.ParentType.Equals(typeof(DbBackup))));

container.Register<IJob, DbBackup>();

// the container will resolve DbBackup with ConsoleLogger.
IJob job = container.Resolve<IJob>();
```

#### **Collection**
```cs
class DbJobsExecutor : IJobsExecutor
{
    public DbBackup(IEnumerable<IJob> jobs)
    { }
}

container.Register<IJob, DbBackup>(options => options
    .WhenDependantIs<DbJobsExecutor>());
container.Register<IJob, DbCleanup>(options => options
    .WhenDependantIs<DbJobsExecutor>());
ontainer.Register<IJob, StorageCleanup>();

container.Register<IJobsExecutor, DbJobsExecutor>();

// jobsExecutor will get DbBackup and DbCleanup within a collection.
IJobsExecutor jobsExecutor = container.Resolve<IJobsExecutor>();
```

#### **Combine**
```cs
class DbBackup : IJob
{
    public DbBackup(ILogger logger)
    { }
}

class StorageCleanup : IJob
{
    public DbBackup(ILogger logger)
    { }
}

container.Register<ILogger, ConsoleLogger>(options => options
    // inject only when we are 
    // currently resolving DbBackup OR StorageCleanup.
    .WhenDependantIs<DbBackup>()
    .WhenDependantIs<StorageCleanup>());

container.Register<IJob, DbBackup>();
container.Register<IJob, StorageCleanup>();

// the collection will contain DbBackup and StorageCleanup 
// constructed with ConsoleLogger.
IEnumerable<IJob> jobs = container.ResolveAll<IJob>();
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Optional resolution

<!-- div:left-panel -->
In cases where it's not guaranteed that a service is resolvable, either because it's not registered or any of its dependencies are missing, you can attempt an optional resolution by using the `ResolveOrDefault()` method. 

In this case, the resolution request will return with `null` (or `default` in case of type values) when the attempt fails.
<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
// returns null when the resolution fails.
IJob job = container.ResolveOrDefault<IJob>();

// throws ResolutionFailedException when the resolution fails.
IJob job = container.Resolve<IJob>();
```
#### **Runtime type API**
```cs
// returns null when the resolution fails.
object job = container.ResolveOrDefault(typeof(IJob));

// throws ResolutionFailedException when the resolution fails.
object job = container.Resolve(typeof(IJob));
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Dependency overrides

<!-- div:left-panel -->

At resolution time, you have the option to override the dependencies of your resolved service by passing them as an `object[]` to the `Resolve()` method.

```cs
class DbBackup : IJob
{
    public DbBackup(ILogger logger)
    { }
}
```
<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
DbBackup backup = container.Resolve<DbBackup>( 
    dependencyOverrides: new object[] 
    { 
        new ConsoleLogger() 
    });
```
#### **Runtime type API**
```cs
object backup = container.Resolve(typeof(DbBackup),
    dependencyOverrides: new object[] 
    { 
        new ConsoleLogger() 
    });
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Activation

<!-- div:left-panel -->
When you only want to build up an instance from a type on the fly without a registration, you can use the container's `.Activate()` method. 

It also allows dependency overriding with `object` arguments and performs member injection on the created instance (when configured).

?> It works as `Activator.CreateInstance()` except that Stashbox supplies the dependencies.
<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
// use dependency injected by container.
DbBackup backup = container.Activate<DbBackup>();

// override the injected dependency.
DbBackup backup = container.Activate<DbBackup>(new ConsoleLogger());
```
#### **Runtime type API**
```cs
// use dependency injected by container.
object backup = container.Activate(typeof(DbBackup));

// override the injected dependency.
object backup = container.Activate(typeof(DbBackup), new ConsoleLogger());
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
### Build-up

You can also do the same *on the fly* activation post-processing (member/method injection) on already constructed instances with the `.BuildUp()` method. 

!> `.BuildUp()` won't register the given instance into the container.
<!-- div:right-panel -->

```cs
class DbBackup : IJob
{
    public ILogger Logger { get; set; }
}

DbBackup backup = new DbBackup();
// the container fills the Logger property.
container.BuildUp(backup); 
```

<!-- panels:end -->