import CodeDescPanel from '@site/src/components/CodeDescPanel';
import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Basic usage

This section is about the basics of Stashbox's API. It will give you a good starting point for more advanced topics described in the following sections. 
Stashbox provides several methods that enable registering services, and we'll go through the most common scenarios with code examples.

## Default registration

<CodeDescPanel>
<div>

Stashbox allows registration operations via the `Register()` methods. 

During registration, the container checks whether the [service type](/docs/getting-started/glossary#service-type--implementation-type) is assignable from the [implementation type](/docs/getting-started/glossary#service-type--implementation-type) and if not, the container throws an [exception](/docs/diagnostics/validation#registration-validation). 

Also, when the implementation is not resolvable, the container throws the same [exception](/docs/diagnostics/validation#registration-validation).

The example registers `DbBackup` to be returned when `IJob` is requested.

</div>

<div>

<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

```cs
container.Register<IJob, DbBackup>();
IJob job = container.Resolve<IJob>();
// throws an exception because ConsoleLogger doesn't implement IJob.
container.Register<IJob, ConsoleLogger>();
// throws an exception because IJob is not a valid implementation.
container.Register<IJob, IJob>();
```

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

```cs
container.Register(typeof(IJob), typeof(DbBackup));
object job = container.Resolve(typeof(IJob));
// throws an exception because ConsoleLogger doesn't implement IJob.
container.Register(typeof(IJob), typeof(ConsoleLogger));
// throws an exception because IJob is not a valid implementation.
container.Register(typeof(IJob), typeof(IJob));
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

You can register a service to itself without specifying a [service type](/docs/getting-started/glossary#service-type--implementation-type), only the implementation ([self registration](/docs/getting-started/glossary#self-registration)). 

In this case, the given implementation is considered the [service type](/docs/getting-started/glossary#service-type--implementation-type) and must be used to request the service (`DbBackup` in the example).

</div>
<div>
<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

```cs
container.Register<DbBackup>();
DbBackup backup = container.Resolve<DbBackup>();
```

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

```cs
container.Register(typeof(DbBackup));
object backup = container.Resolve(typeof(DbBackup));
```
</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>

<div>

The container's API is fluent, which means you can chain the calls on its methods after each other.

</div>
<div>

```cs
var job = container.Register<IJob, DbBackup>()
    .Register<ILogger, ConsoleLogger>()
    .Resolve<IJob>();
```

</div>
</CodeDescPanel>

## Named registration

<CodeDescPanel>

<div>

The example shows how you can bind more implementations to a [service type](/docs/getting-started/glossary#service-type--implementation-type) using names for identification. 

The same name must be used to resolve the named service.

:::note
The name is an `object` type.
:::

</div>
<div>

<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

```cs
container.Register<IJob, DbBackup>("DbBackup");
container.Register<IJob, StorageCleanup>("StorageCleanup");
IJob cleanup = container.Resolve<IJob>("StorageCleanup");
```

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

```cs
container.Register(typeof(IJob), typeof(DbBackup), "DbBackup");
container.Register(typeof(IJob), typeof(StorageCleanup), "StorageCleanup");
object cleanup = container.Resolve(typeof(IJob), "StorageCleanup");
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

You can also get each service that share the same name by requesting an `IEnumerable<>` or using the `ResolveAll()` method with the `name` parameter.

</div>
<div>

```cs
container.Register<IJob, DbBackup>("StorageJobs");
container.Register<IJob, StorageCleanup>("StorageJobs");
container.Register<IJob, AnotherJob>();
// jobs will be [DbBackup, StorageCleanup].
IEnumerable<IJob> jobs = container.Resolve<IEnumerable<IJob>>("StorageJobs");
```

</div>
</CodeDescPanel>

## Instance registration

<CodeDescPanel>
<div>

With instance registration, you can provide an already created external instance to use when the given [service type](/docs/getting-started/glossary#service-type--implementation-type) is requested.

Stashbox automatically handles the [disposal](/docs/guides/scopes#disposal) of the registered instances, but you can turn this feature off with the `withoutDisposalTracking` parameter.

When an `IJob` is requested, the container will always return the external instance.

</div>
<div>
<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

```cs
var job = new DbBackup();
container.RegisterInstance<IJob>(job);

// resolvedJob and job are the same.
IJob resolvedJob = container.Resolve<IJob>();
```

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

```cs
var job = new DbBackup();
container.RegisterInstance(job, typeof(IJob));

// resolvedJob and job are the same.
object resolvedJob = container.Resolve(typeof(IJob));
```

</TabItem>
<TabItem value="Named" label="Named">

```cs
var job = new DbBackup();
container.RegisterInstance<IJob>(job, "DbBackup");

// resolvedJob and job are the same.
IJob resolvedJob = container.Resolve<IJob>("DbBackup");
```

</TabItem>
<TabItem value="No dispose" label="No dispose">

```cs
var job = new DbBackup();
container.RegisterInstance<IJob>(job, withoutDisposalTracking: true);

// resolvedJob and job are the same.
IJob resolvedJob = container.Resolve<IJob>();
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

The instance registration API allows the batched registration of different instances.

</div>
<div>

```cs
container.RegisterInstances<IJob>(new DbBackup(), new StorageCleanup());
IEnumerable<IJob> jobs = container.ResolveAll<IJob>();
```

</div>
</CodeDescPanel>

## Re-mapping

<CodeDescPanel>

<div>

With re-map, you can bind new implementations to a [service type](/docs/getting-started/glossary#service-type--implementation-type) and delete old registrations in one action. 

:::caution
When there are multiple registrations mapped to a [service type](/docs/getting-started/glossary#service-type--implementation-type), `.ReMap()` will replace all of them with the given [implementation type](/docs/getting-started/glossary#service-type--implementation-type). If you want to replace only one specific service, use the `.ReplaceExisting()` [configuration option](/docs/configuration/registration-configuration#replace).
:::

</div>
<div>

<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

```cs
container.Register<IJob, DbBackup>();
container.ReMap<IJob, StorageCleanup>();
// jobs contain all two jobs
IEnumerable<IJob> jobs = container.ResolveAll<IJob>();

container.ReMap<IJob, SlackMessageSender>();
// jobs contains only the SlackMessageSender
jobs = container.ResolveAll<IJob>();
```

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

```cs
container.Register(typeof(IJob), typeof(DbBackup));
container.Register(typeof(IJob), typeof(StorageCleanup));
// jobs contain all two jobs
IEnumerable<object> jobs = container.ResolveAll(typeof(IJob));

container.ReMap(typeof(IJob), typeof(SlackMessageSender));
// jobs contains only the SlackMessageSender
jobs = container.ResolveAll(typeof(IJob));
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Wiring up
<CodeDescPanel>

<div>

Wiring up is similar to [Instance registration](#instance-registration) except that the container will perform property/field injection (if configured so and applicable) on the registered instance during resolution.

</div>
<div>

<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

```cs
container.WireUp<IJob>(new DbBackup());
IJob job = container.Resolve<IJob>();
```

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

```cs
container.WireUp(new DbBackup(), typeof(IJob));
object job = container.Resolve(typeof(IJob));
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Lifetime shortcuts
<CodeDescPanel>

<div>
A service's lifetime indicates how long its instance will live and which re-using policy should be applied when it gets injected.

This example shows how you can use the registration API's shortcuts for lifetimes. These are just sugars, and there are more ways explained in the [lifetimes](/docs/guides/lifetimes) section.

:::info
The `DefaultLifetime` is [configurable](/docs/guides/lifetimes#default-lifetime).
:::

</div>
<div>

<Tabs groupId="generic-runtime-apis">
<TabItem value="Default" label="Default">

When no lifetime is specified, the service will use the container's `DefaultLifetime`, which is `Transient` by default.

```cs
container.Register<IJob, DbBackup>();
IJob job = container.Resolve<IJob>();
```

</TabItem>
<TabItem value="Singleton" label="Singleton">

A service with `Singleton` lifetime will be instantiated once and reused during the container's lifetime.
```cs
container.RegisterSingleton<IJob, DbBackup>();
IJob job = container.Resolve<IJob>();
```

</TabItem>
<TabItem value="Scoped" label="Scoped">

The `Scoped` lifetime behaves like a `Singleton` within a [scope](/docs/guides/scopes). 
A scoped service is instantiated once and reused during the scope's whole lifetime.
```cs
container.RegisterScoped<IJob, DbBackup>();
IJob job = container.Resolve<IJob>();
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>