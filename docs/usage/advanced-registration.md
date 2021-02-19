# Advanced registration
This section is about Stashbox's further configuration options, including the registration configuration API, the registration of factory delegates, multiple implementations, batch registration, the concept of the [Composition Root](https://blog.ploeh.dk/2011/07/28/CompositionRoot/) and many more.

?> This section won't cover all the available options of the registrations API, but you can find them [here](configuration/registration-configuration).

<!-- panels:start -->

<!-- div:title-panel -->
## Factory registration

<!-- div:left-panel -->
You have the option to bind a factory delegate to a registration that will be invoked by the container directly to instantiate your service. 

You can choose between a delegate that gets the current dependency resolver as an argument (used to resolve further services inside the factory) and a parameterless delegate.

<!-- div:right-panel -->

<!-- tabs:start -->

#### **With resolver parameter**
```cs
container.Register<IJob, DbBackup>(options => options
    .WithFactory(resolver => new DbBackup(resolver.Resolve<ILogger>()));
// The container uses the factory for instantiation.
IJob job = container.Resolve<IJob>();
```

#### **Parameter-less**
```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .WithFactory(() => new ConsoleLogger());
// The container uses the factory for instantiation.
IJob job = container.Resolve<ILogger>();
```
<!-- tabs:end -->

<!-- div:left-panel -->
Delegate factories are useful when your service's instantiation is not straight-forward for the container, like when it depends on something that is not available at resolution time. E.g., a connection string.

<!-- div:right-panel -->
```cs
container.Register<IJob, DbBackup>(options => options
    .WithFactory(resolver => 
        new DbBackup(Configuration["ConnectionString"], 
            resolver.Resolve<ILogger>()));
```

<!-- div:left-panel -->
### Factory with custom parameters
Suppose you'd want to use custom parameters for your service's instantiation rather than captured variables in lambda closures. In that case, you can register a `Func<>` delegate that you can use with parameters at resolution time.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
container.RegisterFunc<string, IJob>((connectionString, resolver) => 
    new DbBackup(connectionString, resolver.Resolve<ILogger>()));

Func<string, IJob> backupFactory = container.ResolveFactory<string, IJob>();
IJob dbBackup = backupFactory(Configuration["ConnectionString"]);
```

#### **Runtime type API**
```cs
container.RegisterFunc<string, IJob>((connectionString, resolver) => 
    new DbBackup(connectionString, resolver.Resolve<ILogger>()));

Delegate backupFactory = container.ResolveFactory(typeof(IJob), 
    parameterTypes: new[] { typeof(string) });
IJob dbBackup = backupFactory.DynamicInvoke(Configuration["ConnectionString"]);
```

<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Multiple implementations

<!-- div:left-panel -->
As we previously saw in the [Named registration](usage/basics?id=named-registration) topic, Stashbox allows you to have multiple implementations bound to a particular service type. You can use names to distinguish them, but you can also access them by requesting a typed collection using the service type.

?> The returned collection is in the same order as the services were registered.
Also, to request a collection, you can use any interface implemented by an array.

<!-- div:right-panel -->

```cs
container.Register<IJob, DbBackup>();
container.Register<IJob, StorageCleanup>();
container.Register<IJob, ImageProcess>();
```

<!-- tabs:start -->

#### **ResolveAll**
```cs
// jobs contain all three services in registration order.
IEnumerable<IJob> jobs = container.ResolveAll<IJob>();
```

#### **Array**
```cs
// jobs contain all three services in registration order.
IJob[] jobs = container.Resolve<IJob[]>();
```

#### **IEnumerable**
```cs
// jobs contain all three services in registration order.
IEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>();
```

#### **IList**
```cs
// jobs contain all three services in registration order.
IList<IJob> jobs = container.Resolve<IList<IJob>>();
```

#### **ICollection**
```cs
// jobs contain all three services in registration order.
ICollection<IJob> jobs = container.Resolve<ICollection<IJob>>();
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
When you have multiple implementations registered to a service, a request to the service type without a name will return the **last registered implementation**.

?> Not only names can be used to distinguish registrations, [conditions](usage/service-resolution?id=conditional-resolution) and [named scopes](usage/scopes?id=named-scopes) can also influence the results.
<!-- div:right-panel -->

```cs
container.Register<IJob, DbBackup>();
container.Register<IJob, StorageCleanup>();
container.Register<IJob, ImageProcess>();

// job will be the ImageProcess.
IJob job = container.Resolve<IJob>();
```

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Map to multiple services

<!-- div:left-panel -->
When you have an implementation that implements multiple interfaces, you have the option to bind its registration to all or some of those interfaces/base types.

In these examples, we assume that `DbBackup` implements `IScheduledJob` alongside `IJob`.
<!-- div:right-panel -->

<!-- tabs:start -->
#### **To another type**
```cs
container.Register<IJob, DbBackup>(options => options
    .AsServiceAlso<IScheduledJob>());

IJob job = container.Resolve<IJob>(); // DbBackup
IScheduledJob job = container.Resolve<IScheduledJob>(); // DbBackup
DbBackup job = container.Resolve<DbBackup>(); // error, not found
```

#### **To all implemented types**
```cs
container.Register<DbBackup>(options => options
    .AsImplementedTypes());

IJob job = container.Resolve<IJob>(); // DbBackup
IScheduledJob job = container.Resolve<IScheduledJob>(); // DbBackup
DbBackup job = container.Resolve<DbBackup>(); // DbBackup
```

<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Batch registration

<!-- div:left-panel -->
You have the option to register multiple services in a single registration operation. 

**Filters (optional):**
First, the container will use the *implementation filter* action to select only those types from the given collection that we want to register. When we have those, the container will execute the *service filter* on their implemented interfaces and base classes to select which service type they should be mapped to.

?> Framework types like `IDisposable` are excluded from being considered as a service type by default.

?> You can use the registration configuration API to configure the individual registrations.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Default**
This example will register three types to all their implemented interfaces, extended base classes, and to themselves without any filter:
```cs
container.RegisterTypes(new[] 
    { 
        typeof(DbBackup), 
        typeof(ConsoleLogger), 
        typeof(StorageCleanup) 
    });

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
ILogger logger = container.Resolve<ILogger>(); // ConsoleLogger
IJob job = container.Resolve<IJob>(); // StorageCleanup
DbBackup backup = container.Resolve<DbBackup>(); // DbBackup
```

#### **Filters**
In this example, we assume that `DbBackup` and `StorageCleanup` are implementing `IDisposable` besides `IJob` and also extending a `JobBase` abstract class.
```cs
container.RegisterTypes(new[] 
    { typeof(DbBackup), typeof(ConsoleLogger), typeof(StorageCleanup) },
    // implementation filter, only those implementations that implements IDisposable
    impl => typeof(IDisposable).IsAssignableFrom(impl),
    // service filter, register them to base classes only
    (impl, service) => service.IsAbstract && !service.IsInterface);

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 0 items
IEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 2 items
ILogger logger = container.Resolve<ILogger>(); // error, not found
DbBackup backup = container.Resolve<DbBackup>(); // DbBackup
```

#### **Without self**
This example will ignore the mapping of implementation types to themselves completely:
```cs
container.RegisterTypes(new[] 
    { 
        typeof(DbBackup), 
        typeof(ConsoleLogger), 
        typeof(StorageCleanup)
    },
    registerSelf: false);

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
ILogger logger = container.Resolve<ILogger>(); // ConsoleLogger
DbBackup backup = container.Resolve<DbBackup>(); // error, not found
ConsoleLogger logger = container.Resolve<ConsoleLogger>(); // error, not found
```

#### **Registration options**
This example will configure all registrations mapped to `ILogger` as `Singleton`:
```cs
container.RegisterTypes(new[] 
    { 
        typeof(DbBackup), 
        typeof(ConsoleLogger), 
        typeof(StorageCleanup)
    },
    configurator: options => 
    {
        if (options.ServiceType == typeof(ILogger))
            options.WithSingletonLifetime();
    });

ILogger logger = container.Resolve<ILogger>(); // ConsoleLogger
ILogger newLogger = container.Resolve<ILogger>(); // the same ConsoleLogger
IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
```

<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
Another type of service filter is the `.RegisterTypesAs<T>()` method, which registers only those types that implements the `T` service type.

?> This method also accepts an implementation filter and registration configurator action as the `.RegisterTypes()`.

!> `.RegisterTypesAs<T>()` doesn't create self registrations as it only maps the implementations to the given `T` service type.
<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
container.RegisterTypesAs<IJob>(new[] 
    { 
        typeof(DbBackup), 
        typeof(ConsoleLogger), 
        typeof(StorageCleanup) 
    });

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
ILogger logger = container.Resolve<ILogger>(); // error, not found
IJob job = container.Resolve<IJob>(); // StorageCleanup
DbBackup backup = container.Resolve<DbBackup>(); // error, not found
```
#### **Runtime type API**
```cs
container.RegisterTypesAs(typeof(IJob), new[] 
    { 
        typeof(DbBackup), 
        typeof(ConsoleLogger), 
        typeof(StorageCleanup) 
    });

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
ILogger logger = container.Resolve<ILogger>(); // error, not found
IJob job = container.Resolve<IJob>(); // StorageCleanup
DbBackup backup = container.Resolve<DbBackup>(); // error, not found
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Assembly registration

<!-- div:left-panel -->

The batch registration API's signature *(filters, registration configuration action, self-registration)* is also usable for registering services from given assemblies.

In this example, we assume that the same three services used at the batch registration are in the same assembly.

?> The container also detects and registers open-generic definitions (when applicable) from the supplied type collection. You can read about [open-generics here](advanced/generics?id=open-generic).

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Single assembly**
```cs
container.RegisterAssembly(typeof(DbBackup).Assembly,
    // service filter, register to interfaces only
    serviceTypeSelector: (impl, service) => info.IsInterface,
    registerSelf: false,
    configurator: options => options.WithoutDisposalTracking());

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
IEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items
ILogger logger = container.Resolve<ILogger>(); // ConsoleLogger
DbBackup backup = container.Resolve<DbBackup>(); // error, not found
```

#### **Multiple assemblies**
```cs
container.RegisterAssembly(new[] 
    { 
        typeof(DbBackup).Assembly, 
        typeof(JobFromAnotherAssembly).Assembly 
    },
    // service filter, register to interfaces only
    serviceTypeSelector: (impl, service) => info.IsInterface,
    registerSelf: false,
    configurator: options => options.WithoutDisposalTracking());

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
IEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items
ILogger logger = container.Resolve<ILogger>(); // ConsoleLogger
DbBackup backup = container.Resolve<DbBackup>(); // error, not found
```

#### **Containing type**
```cs
container.RegisterAssemblyContaining<DbBackup>(
    // service filter, register to interfaces only
    serviceTypeSelector: (impl, service) => service.IsInterface,
    registerSelf: false,
    configurator: options => options.WithoutDisposalTracking());

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
IEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items
ILogger logger = container.Resolve<ILogger>(); // ConsoleLogger
DbBackup backup = container.Resolve<DbBackup>(); // error, not found
```

<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Composition root

<!-- div:left-panel -->
The entry point of components, where all the services are wired together, is often referred to as [Composition Root](https://blog.ploeh.dk/2011/07/28/CompositionRoot/).

Stashbox provides an `ICompositionRoot` interface used to detect user-defined entry points in given components or assemblies. 

You can wire up your *composition root* implementation with `ComposeBy<TRoot>()`, or you can let the container find and execute all available implementations within an assembly.

?> Your `ICompositionRoot` implementations also can have their dependencies that would be resolved by the container.

<!-- div:right-panel -->

```cs
class ExampleRoot : ICompositionRoot
{
    public ExampleRoot(IDependency rootDependency)
    { }

    public void Compose(IStashboxContainer container)
    {
       container.Register<IServiceA, ServiceA>();
       container.Register<IServiceB, ServiceB>();
    }
}
```

<!-- tabs:start -->
#### **Single**
```cs
// compose a single root.
container.ComposeBy<ExampleRoot>();
```

#### **Assembly**
```cs
// compose every root in the given assembly.
container.ComposeAssembly(typeof(IServiceA).Assembly);
```

#### **Override**
```cs
// compose a single root with overridden dependency.
container.ComposeBy<ExampleRoot>(new CustomRootDependency());
```

<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Injection parameters

<!-- div:left-panel -->
If you have some pre-evaluated dependencies you'd like to inject at resolution time, you can set them as an injection parameter during registration. 

?> Injection parameter names are matched to constructor argument and field/property names.

<!-- div:right-panel -->
```cs
container.Register<IJob, DbBackup>(options => options
    .WithInjectionParameter("logger", new ConsoleLogger())
    .WithInjectionParameter("eventBroadcaster", new MessageBus());

// the injection parameters will be passed to DbBackup's constructor.
IJob backup = container.Resolve<IJob>();
```
<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Initializer / Finalizer

<!-- div:left-panel -->
The container provides specific extension points that could be used as hooks to react to the instantiated service's lifetime events. 

For this reason, you can specify your own *Initializer* and *Finalizer* delegates. The finalizer is called on the service's [disposal](usage/scopes?id=disposal).
<!-- div:right-panel -->
```cs
container.Register<ILogger, FileLogger>(options => options
    // delegate that called right after instantiation.
    .WithInitializer((logger, resolver) => logger.OpenFile())
    // delegate that called right before the instance's disposal.
    .WithFinalizer(logger => logger.CloseFile()));
```
<!-- panels:end -->