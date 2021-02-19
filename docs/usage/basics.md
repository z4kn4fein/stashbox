# Basic usage
This section is about the basics of Stashbox's API. It will give you a good starting point for more advanced topics described in the following sections. 
Stashbox provides several methods that enable registering services, and we'll go through the most common scenarios with code examples.

<!-- panels:start -->

<!-- div:title-panel -->
## Default registration

<!-- div:left-panel -->
Stashbox allows registration operations via `Register()` methods. 

During registration, the container checks whether the service type is assignable from the implementation type and if not, the container throws an [exception](diagnostics/exceptions?id=invalidregistrationexception). 

Also, when the implementation is not resolvable (when it's an interface or abstract class), the container throws the same [exception](diagnostics/exceptions?id=invalidregistrationexception).

The example registers `DbBackup` to be returned when `IJob` is requested.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
container.Register<IJob, DbBackup>();
IJob job = container.Resolve<IJob>();
// throws an exception because ConsoleLogger doesn't implement IJob.
container.Register<IJob, ConsoleLogger>();
// throws an exception because IJob is not a valid implementation.
container.Register<IJob, IJob>();
```
#### **Runtime type API**
```cs
container.Register(typeof(IJob), typeof(DbBackup));
object job = container.Resolve(typeof(IJob));
// throws an exception because ConsoleLogger doesn't implement IJob.
container.Register(typeof(IJob), typeof(ConsoleLogger));
// throws an exception because IJob is not a valid implementation.
container.Register(typeof(IJob), typeof(IJob));
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
You can also register a service to itself without specifying a service type, only the implementation type. 

In this case, the given implementation is considered as the service type and must be used for requesting the service (`DbBackup` in the example).

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
container.Register<DbBackup>();
DbBackup backup = container.Resolve<DbBackup>();
```
#### **Runtime type API**
```cs
container.Register(typeof(DbBackup));
object backup = container.Resolve(typeof(DbBackup));
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
The container's API is fluent, which means you can chain the calls on its methods after each other.

<!-- div:right-panel -->
```cs
var job = container.Register<IJob, DbBackup>()
    .Register<ILogger, ConsoleLogger>()
    .Resolve<IJob>();
```

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Named registration

<!-- div:left-panel -->
The example shows how you can bind more implementations to a service type using names for identification. 

The same name must be used to resolve the named service.

?> The name is an `object` type.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
container.Register<IJob, DbBackup>("DbBackup")
container.Register<IJob, StorageCleanup>("StorageCleanup");
IJob cleanup = container.Resolve<IJob>("StorageCleanup");
```
#### **Runtime type API**
```cs
container.Register(typeof(IJob), typeof(DbBackup), "DbBackup")
container.Register(typeof(IJob), typeof(StorageCleanup), "StorageCleanup");
object cleanup = container.Resolve(typeof(IJob), "StorageCleanup");
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Instance registration

<!-- div:left-panel -->
With instance registration, you can provide an already created external instance to use when the given service type is requested.

Stashbox automatically handles the [disposal](usage/scopes?id=disposal) of the registered instances, but you can turn this feature off with the `withoutDisposalTracking` parameter.

When an `IJob` is requested, the container will always return the external instance.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
var job = new DbBackup();
container.RegisterInstance<IJob>(job);

// resolvedJob and job are the same.
IJob resolvedJob = container.Resolve<IJob>();
```
#### **Runtime type API**
```cs
var job = new DbBackup();
container.RegisterInstance(job, typeof(IJob));

// resolvedJob and job are the same.
object resolvedJob = container.Resolve(typeof(IJob));
```
#### **Named**
```cs
var job = new DbBackup();
container.RegisterInstance<IJob>(job, "DbBackup");

// resolvedJob and job are the same.
IJob resolvedJob = container.Resolve<IJob>("DbBackup");
```

#### **No dispose**
```cs
var job = new DbBackup();
container.RegisterInstance<IJob>(job, withoutDisposalTracking: true);

// resolvedJob and job are the same.
IJob resolvedJob = container.Resolve<IJob>();
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->

The instance registration API allows the batched registration of different instances.

<!-- div:right-panel -->

```cs
container.RegisterInstances<IJob>(new DbBackup(), new StorageCleanup());
IEnumerable<IJob> jobs = container.ResolveAll<IJob>();
```

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Re-mapping

<!-- div:left-panel -->
With re-map, you can replace all implementations registered to a service type. 

!> When there are multiple registrations mapped to a service type, `.ReMap()` will replace all of them with the given implementation type. If you want to replace only one specified service, use the `.ReplaceExisting()` [configuration option](configuration/registration-configuration?id=replace).

<!-- div:right-panel -->


<!-- tabs:start -->
#### **Generic API**
```cs
container.Register<IJob, DbBackup>();
container.ReMap<IJob, StorageCleanup>();
// jobs contain all two jobs
IEnumerable<IJob> jobs = container.ResolveAll<IJob>();

container.ReMap<IJob, SlackMessageSender>();
// jobs contains only the SlackMessageSender
jobs = container.ResolveAll<IJob>();
```
#### **Runtime type API**
```cs
container.Register(typeof(IJob), typeof(DbBackup));
container.Register(typeof(IJob), typeof(StorageCleanup));
// jobs contain all two jobs
IEnumerable<object> jobs = container.ResolveAll(typeof(IJob));

container.ReMap(typeof(IJob), typeof(SlackMessageSender));
// jobs contains only the SlackMessageSender
jobs = container.ResolveAll(typeof(IJob));
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Wiring up

<!-- div:left-panel -->
Wiring up is similar to the [Instance registration](#instance-registration) except that the container will perform property/field injection (if configured so and applicable) on the registered instance during resolution.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Generic API**
```cs
container.WireUp<IJob>(new DbBackup());
IJob job = container.Resolve<IJob>();
```
#### **Runtime type API**
```cs
container.WireUp(new DbBackup(), typeof(IJob));
object job = container.Resolve(typeof(IJob));
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Lifetime shortcuts

<!-- div:left-panel -->
The service's lifetime indicates how long the service's instance will live and the re-using policy applied when it gets injected.

This example shows how you can use the registration API's shortcuts for lifetimes. These are just sugars, and there are more ways explained in the [lifetimes](usage/lifetimes) section.

?> The `DefaultLifetime` is [configurable](usage/lifetimes?id=default-lifetime).
<!-- div:right-panel -->

<!-- tabs:start -->
#### **Default**
When no lifetime is specified, the service will use the container's `DefaultLifetime`, which is `Transient` by default.

```cs
container.Register<IJob, DbBackup>();
IJob job = container.Resolve<IJob>();
```
#### **Singleton**
A service with `Singleton` lifetime will be instantiated once and re-used during the container's lifetime.
```cs
container.RegisterSingleton<IJob, DbBackup>();
IJob job = container.Resolve<IJob>();
```
#### **Scoped**
The `Scoped` lifetime behaves like a `Singleton` within a [scope](usage/scopes). 
The scoped service is instantiated once and re-used during the scope's lifetime.
```cs
container.RegisterScoped<IJob, DbBackup>();
IJob job = container.Resolve<IJob>();
```
<!-- tabs:end -->

<!-- panels:end -->