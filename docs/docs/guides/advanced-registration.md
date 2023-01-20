import CodeDescPanel from '@site/src/components/CodeDescPanel';
import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Advanced registration
This section is about Stashbox's further configuration options, including the registration configuration API, the registration of factory delegates, multiple implementations, batch registrations, the concept of the [Composition Root](https://blog.ploeh.dk/2011/07/28/CompositionRoot/), and many more.

:::info
This section won't cover all the available options of the registrations API, but you can find them [here](/docs/configuration/registration-configuration).
:::

## Factory registration

<CodeDescPanel>
<div>

You can bind a factory delegate to a registration that the container will invoke directly to instantiate your service. 

You can use parameter-less and custom parameterized delegates as a factory. [Here](/docs/configuration/registration-configuration#factory) is the list of all available options.

You can also get the current [dependency resolver](/docs/getting-started/glossary#dependency-resolver) as a delegate parameter to resolve any additional dependencies required for the service construction.

</div>
<div>

<Tabs>
<TabItem value="Parameter-less" label="Parameter-less">

```cs
container.Register<ILogger, ConsoleLogger>(options => options
    .WithFactory(() => new ConsoleLogger());

// the container uses the factory for instantiation.
IJob job = container.Resolve<ILogger>();
```

</TabItem>
<TabItem value="Parameterized" label="Parameterized">

```cs
container.Register<IJob, DbBackup>(options => options
    .WithFactory<ILogger>(logger => new DbBackup(logger));

// the container uses the factory for instantiation.
IJob job = container.Resolve<IJob>();
```

</TabItem>
<TabItem value="Resolver parameter" label="Resolver parameter">

```cs
container.Register<IJob, DbBackup>(options => options
    .WithFactory(resolver => new DbBackup(resolver.Resolve<ILogger>()));
    
// the container uses the factory for instantiation.
IJob job = container.Resolve<IJob>();
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

Delegate factories are useful when your service's instantiation is not straight-forward for the container, like when it depends on something that is not available at resolution time. E.g., a connection string.

</div>
<div>

```cs
container.Register<IJob, DbBackup>(options => options
    .WithFactory<ILogger>(logger => 
        new DbBackup(Configuration["DbConnectionString"], logger));
```

</div>
</CodeDescPanel>


<CodeDescPanel>
<div>

### Factories with parameter overrides
Stashbox can implicitly [wrap](/docs/advanced/wrappers-resolvers#delegate) your service in a `Delegate` and lets you pass parameters that can override your service's dependencies. Moreover, you can register your own custom delegate that the container will resolve when you request your service wrapped in a `Delegate`.

</div>
<div>

<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

```cs
container.RegisterFunc<string, IJob>((connectionString, resolver) => 
    new DbBackup(connectionString, resolver.Resolve<ILogger>()));

Func<string, IJob> backupFactory = container.Resolve<Func<string, IJob>>();
IJob dbBackup = backupFactory(Configuration["ConnectionString"]);
```

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

```cs
container.RegisterFunc<string, IJob>((connectionString, resolver) => 
    new DbBackup(connectionString, resolver.Resolve<ILogger>()));

Delegate backupFactory = container.ResolveFactory(typeof(IJob), 
    parameterTypes: new[] { typeof(string) });
IJob dbBackup = backupFactory.DynamicInvoke(Configuration["ConnectionString"]);
```
<!-- tabs:end -->

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

If a service has multiple constructors, the container visits those first, that has matching parameters passed to the factory, with respecting the additional [constructor selection rules](/docs/configuration/registration-configuration#constructor-selection).

</div>
<div>

```cs
class Service
{
    public Service(int number) { }
    public Service(string text) { }
}

container.Register<Service>();

// create the factory with an int input parameter.
var func = constainer.Resolve<Func<int, Service>>();

// the constructor with the int param 
// is used for instantiation.
var service = func(2);
```
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### Consider this before using the resolver parameter inside a factory
Delegate factories are a black-box for the container. It doesn't have control over what's happening inside a delegate, which means when you resolve additional dependencies with the [dependency resolver](/docs/getting-started/glossary#dependency-resolver) parameter, they could easily bypass the [lifetime](/docs/diagnostics/validation#lifetime-validation) and [circular dependency](/docs/diagnostics/validation#circular-dependency) validations. Fortunately, you have the option to keep them validated anyway with parameterized factory delegates.

#### Delegates with dependencies passed as parameters
Rather than using the [dependency resolver](/docs/getting-started/glossary#dependency-resolver) parameter inside the factory, let the container inject the dependencies into the delegate as parameters. This way, the [resolution tree's](/docs/getting-started/glossary#resolution-tree) integrity remains stable because no service resolution happens inside the black-box, and each parameter is validated.

</div>
<div>


```cs
interface IEventProcessor { }

class EventProcessor : IEventProcessor
{
    public EventProcessor(ILogger logger, IEventValidator validator)
    { }
}

container.Register<ILogger, ConsoleLogger>();
container.Register<IEventValidator, EventValidator>();

container.Register<IEventProcessor, EventProcessor>(options => options
    // Ilogger and IEventValidator instances are injected
    // by the container at resolution time, so they will be
    // validated against circular and captive dependencies.
    .WithFactory<ILogger, IEventValidator>((logger, validator) => 
        new EventProcessor(logger, validator));

// the container resolves ILogger and IEventValidator first, then
// it passes them to the factory as delegate parameters.
IEventProcessor processor = container.Resolve<IEventProcessor>();
```

</div>
</CodeDescPanel>


## Multiple implementations

<CodeDescPanel>
<div>

As we previously saw in the [Named registration](/docs/guides/basics#named-registration) topic, Stashbox allows you to have multiple implementations bound to a particular [service type](/docs/getting-started/glossary#service-type--implementation-type). You can use names to distinguish them, but you can also access them by requesting a typed collection using the [service type](/docs/getting-started/glossary#service-type--implementation-type).

:::note 
The returned collection is in the same order as the services were registered.
Also, to request a collection, you can use any interface implemented by an array.
:::

</div>
<div>

```cs
container.Register<IJob, DbBackup>();
container.Register<IJob, StorageCleanup>();
container.Register<IJob, ImageProcess>();
```

<Tabs>
<TabItem value="ResolveAll" label="ResolveAll">

```cs
// jobs contain all three services in registration order.
IEnumerable<IJob> jobs = container.ResolveAll<IJob>();
```

</TabItem>
<TabItem value="Array" label="Array">

```cs
// jobs contain all three services in registration order.
IJob[] jobs = container.Resolve<IJob[]>();
```

</TabItem>
<TabItem value="IEnumerable" label="IEnumerable">

```cs
// jobs contain all three services in registration order.
IEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>();
```

</TabItem>
<TabItem value="IList" label="IList">

```cs
// jobs contain all three services in registration order.
IList<IJob> jobs = container.Resolve<IList<IJob>>();
```

</TabItem>
<TabItem value="ICollection" label="ICollection">

```cs
// jobs contain all three services in registration order.
ICollection<IJob> jobs = container.Resolve<ICollection<IJob>>();
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

When you have multiple implementations registered to a service, a request to the [service type](/docs/getting-started/glossary#service-type--implementation-type) without a name will return the **last registered implementation**.

:::info
Not only names can be used to distinguish registrations, [conditions](/docs/guides/service-resolution#conditional-resolution), [named scopes](/docs/guides/scopes#named-scopes), and [metadata](/docs/advanced/wrappers-resolvers#metadata--tuple) can also influence the results.
:::

</div>
<div>

```cs
container.Register<IJob, DbBackup>();
container.Register<IJob, StorageCleanup>();
container.Register<IJob, ImageProcess>();

// job will be the ImageProcess.
IJob job = container.Resolve<IJob>();
```

</div>
</CodeDescPanel>

## Binding to multiple services

<CodeDescPanel>
<div>

When you have a service that implements multiple interfaces, you have the option to bind its registration to all or some of those additional interfaces or base types.

Suppose we have the following class declaration:
```cs
class DbBackup : IJob, IScheduledJob
{ 
    public DbBackup() { }
}
```

</div>
<div>

<Tabs>
<TabItem value="To another type" label="To another type">

```cs
container.Register<IJob, DbBackup>(options => options
    .AsServiceAlso<IScheduledJob>());

IJob job = container.Resolve<IJob>(); // DbBackup
IScheduledJob job = container.Resolve<IScheduledJob>(); // DbBackup
DbBackup job = container.Resolve<DbBackup>(); // error, not found
```

</TabItem>
<TabItem value="To all implemented types" label="To all implemented types">

```cs
container.Register<DbBackup>(options => options
    .AsImplementedTypes());

IJob job = container.Resolve<IJob>(); // DbBackup
IScheduledJob job = container.Resolve<IScheduledJob>(); // DbBackup
DbBackup job = container.Resolve<DbBackup>(); // DbBackup
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Batch registration

<CodeDescPanel>
<div>

You have the option to register multiple services in a single registration operation. 

**Filters (optional):**
First, the container will use the *implementation filter* action to select only those types from the collection we want to register. When we have those, the container will execute the *service filter* on their implemented interfaces and base classes to select which [service type](/docs/getting-started/glossary#service-type--implementation-type) they should be mapped to.

:::note
Framework types like `IDisposable` are excluded from being considered as a [service type](/docs/getting-started/glossary#service-type--implementation-type) by default.
:::

:::tip 
You can use the registration configuration API to configure individual registrations.
:::

</div>
<div>

<Tabs>
<TabItem value="Default" label="Default">

This example will register three types to all their implemented interfaces, extended base classes, and to themselves ([self registration](/docs/getting-started/glossary#self-registration)) without any filter:
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

</TabItem>
<TabItem value="Filters" label="Filters">

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

</TabItem>
<TabItem value="Without self" label="Without self">

This example ignores the [self registrations](/docs/getting-started/glossary#self-registration) completely:
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

</TabItem>
<TabItem value="Registration options" label="Registration options">

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

</TabItem>
</Tabs>

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

Another type of service filter is the `.RegisterTypesAs<T>()` method, which registers only those types that implements the `T` [service type](/docs/getting-started/glossary#service-type--implementation-type).

:::note 
This method also accepts an implementation filter and a registration configurator action like `.RegisterTypes()`.
:::

:::caution
`.RegisterTypesAs<T>()` doesn't create [self registrations](/docs/getting-started/glossary#self-registration) as it only maps the implementations to the given `T` [service type](/docs/getting-started/glossary#service-type--implementation-type).
:::

</div>
<div>

<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

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

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

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

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Assembly registration

<CodeDescPanel>
<div>

The batch registration API *(filters, registration configuration action, self-registration)* is also usable for registering services from given assemblies.

In this example, we assume that the same three services we used in the [batch registration](#batch-registration) section are in the same assembly.

:::info
The container also detects and registers open-generic definitions (when applicable) from the supplied type collection. You can read about [open-generics here](/docs/advanced/generics#open-generics).
:::

</div>
<div>

<Tabs>
<TabItem value="Single assembly" label="Single assembly">

```cs
container.RegisterAssembly(typeof(DbBackup).Assembly,
    // service filter, register to interfaces only
    serviceTypeSelector: (impl, service) => info.IsInterface,
    registerSelf: false,
    configurator: options => options.WithoutDisposalTracking()
);

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
IEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items
ILogger logger = container.Resolve<ILogger>(); // ConsoleLogger
DbBackup backup = container.Resolve<DbBackup>(); // error, not found
```

</TabItem>
<TabItem value="Multiple assemblies" label="Multiple assemblies">

```cs
container.RegisterAssemblies(new[] 
    { 
        typeof(DbBackup).Assembly, 
        typeof(JobFromAnotherAssembly).Assembly 
    },
    // service filter, register to interfaces only
    serviceTypeSelector: (impl, service) => info.IsInterface,
    registerSelf: false,
    configurator: options => options.WithoutDisposalTracking()
);

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
IEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items
ILogger logger = container.Resolve<ILogger>(); // ConsoleLogger
DbBackup backup = container.Resolve<DbBackup>(); // error, not found
```

</TabItem>
<TabItem value="Containing type" label="Containing type">

```cs
container.RegisterAssemblyContaining<DbBackup>(
    // service filter, register to interfaces only
    serviceTypeSelector: (impl, service) => service.IsInterface,
    registerSelf: false,
    configurator: options => options.WithoutDisposalTracking()
);

IEnumerable<IJob> jobs = container.ResolveAll<IJob>(); // 2 items
IEnumerable<JobBase> jobs = container.ResolveAll<JobBase>(); // 0 items
ILogger logger = container.Resolve<ILogger>(); // ConsoleLogger
DbBackup backup = container.Resolve<DbBackup>(); // error, not found
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Composition root

<CodeDescPanel>
<div>

The [Composition Root](https://blog.ploeh.dk/2011/07/28/CompositionRoot/) is an entry point where all services required to make a component functional are wired together.

Stashbox provides an `ICompositionRoot` interface that can be used to define an entry point for a given component or even for an entire assembly. 

You can wire up your *composition root* implementation with `ComposeBy<TRoot>()`, or you can let the container find and execute all available *composition root* implementations within an assembly.

:::note
Your `ICompositionRoot` implementation also can have dependencies that the container will resolve.
:::

</div>
<div>


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

<Tabs>
<TabItem value="Single" label="Single">

```cs
// compose a single root.
container.ComposeBy<ExampleRoot>();
```

</TabItem>
<TabItem value="Assembly" label="Assembly">

```cs
// compose every root in the given assembly.
container.ComposeAssembly(typeof(IServiceA).Assembly);
```

</TabItem>
<TabItem value="Override" label="Override">

```cs
// compose a single root with dependency override.
container.ComposeBy<ExampleRoot>(new CustomRootDependency());
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Injection parameters

<CodeDescPanel>
<div>

If you have pre-evaluated dependencies you'd like to inject at resolution time, you can set them as injection parameters during registration. 

:::note
Injection parameter names are matched to constructor arguments or field/property names.
:::

</div>
<div>

```cs
container.Register<IJob, DbBackup>(options => options
    .WithInjectionParameter("logger", new ConsoleLogger())
    .WithInjectionParameter("eventBroadcaster", new MessageBus());

// the injection parameters will be passed to DbBackup's constructor.
IJob backup = container.Resolve<IJob>();
```
</div>
</CodeDescPanel>

## Initializer / finalizer

<CodeDescPanel>
<div>

The container provides specific extension points to let you react to lifetime events of an instantiated service. 

For this reason, you can specify *Initializer* and *Finalizer* delegates. The *finalizer* is called upon the service's [disposal](/docs/guides/scopes#disposal), and the *initializer* is called upon the service's construction.

</div>
<div>

```cs
container.Register<ILogger, FileLogger>(options => options
    // delegate that called right after instantiation.
    .WithInitializer((logger, resolver) => logger.OpenFile())
    // delegate that called right before the instance's disposal.
    .WithFinalizer(logger => logger.CloseFile()));
```
</div>
</CodeDescPanel>
