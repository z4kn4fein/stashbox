import CodeDescPanel from '@site/src/components/CodeDescPanel';
import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Scopes
A scope is Stashbox's implementation of the unit-of-work pattern; it encapsulates a given unit used to resolve and store instances required for a given work. When a scoped service is resolved or injected, the scope ensures that it gets instantiated only once within the scope's lifetime. When the work is finished, the scope cleans up the resources by disposing each tracked disposable instance.

A web application is a fair usage example for scopes as it has a well-defined execution unit that can be bound to a scope - the HTTP request. Every request could have its unique scope attached to the request's lifetime. When a request ends, the scope gets closed, and all the scoped instances will be disposed.

## Creating a scope

<CodeDescPanel>
<div>

You can create a scope from the container by calling its `.BeginScope()` method.

Scopes can be **nested**, which means you can create sub-scopes from existing ones with their `.BeginScope()` method. 

Scoped service instances are not shared across parent and sub-scope relations.

Nested scopes can be **attached to their parent's lifetime**, which means when a parent gets disposed all child scopes attached to it will be disposed.

Scopes are `IDisposable`; they track all `IDisposable` instances they resolved. Calling their `Dispose()` method or wrapping them in `using` statements is a crucial part of their service's lifetime management.

</div>
<div>

<Tabs>
<TabItem value="Create" label="Create">

```cs
container.RegisterScoped<IJob, DbBackup>();

// create the scope with using so it'll be auto disposed.
using (var scope = container.BeginScope())
{
    IJob job = scope.Resolve<IJob>();
    IJob jobAgain = scope.Resolve<IJob>();
    // job and jobAgain are created in the 
    // same scope, so they are the same instance.
}
```

</TabItem>
<TabItem value="Nested" label="Nested">

```cs
container.RegisterScoped<IJob, DbBackup>();

using (var parent = container.BeginScope())
{
    IJob job = parent.Resolve<IJob>();
    IJob jobAgain = parent.Resolve<IJob>();
    // job and jobAgain are created in the 
    // same scope, so they are the same instance.

    // create a sub-scope.
    using var sub = parent.BeginScope();

    IJob subJob = sub.Resolve<IJob>();
    // subJob is a new instance created in the sub-scope, 
    // differs from either job and jobAgain.
}
```

</TabItem>
<TabItem value="Nested attached" label="Nested attached">

```cs
container.RegisterScoped<IJob, DbBackup>();

var parent = container.BeginScope();
var sub = parent.BeginScope(attachToParent: true);

// sub will also be disposed with the scope.
scope.Dispose(); 
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Named scopes

<CodeDescPanel>
<div>

There might be cases where you don't want to use a service globally across every scope, only in specific ones. 

For this reason, you can differentiate specific scope groups from other scopes with a **name**.

You can set a service's lifetime to [named scope lifetime](/docs/guides/lifetimes#named-scope-lifetime) initialized with the **scope's name** to mark it usable only for that named scope.

```cs
container.Register<IJob, DbBackup>(options => 
    options.InNamedScope("DbScope"));

container.Register<IJob, DbCleanup>(options => 
    options.InNamedScope("DbScope"));

container.Register<IJob, DbIndexRebuild>(options => 
    options.InNamedScope("DbSubScope"));

container.Register<IJob, StorageCleanup>(options => 
    options.InNamedScope("StorageScope"));
```

:::note
Services with named scope lifetime are **shared across parent and sub-scope relations**.
:::

If you request a name-scoped service from an un-named scope, you'll get an error or no result (depending on the configuration) because those services are selectable only by named scopes with a matching name.

</div>
<div>

```cs
using (var dbScope = container.BeginScope("DbScope"))
{ 
    // DbBackup and DbCleanup will be returned.
    IEnumerable<IJob> jobs = dbScope.ResolveAll<IJob>();

    // create a sub-scope of dbScope.
    using var sub = dbScope.BeginScope();

    // DbBackup and DbCleanup will be returned from the named parent scope.
    IEnumerable<IJob> jobs = sub.ResolveAll<IJob>();

    // create a named sub-scope.
    using var namedSub = dbScope.BeginScope("DbSubScope");
    // DbIndexRebuild will be returned from the named sub-scope.
    IEnumerable<IJob> jobs = namedSub.ResolveAll<IJob>();
}

using (var storageScope = container.BeginScope("StorageScope"))
{
    // StorageCleanup will be returned.
    IJob job = storageScope.Resolve<IJob>();
}

// create a common scope without a name.
using (var unNamed = container.BeginScope())
{
    // empty result as there's no service registered without named scope.
    IEnumerable<IJob> jobs = unNamed.ResolveAll<IJob>();

    // throws an exception because there's no unnamed service registered.
    IJob job = unNamed.Resolve<IJob>();
}
```

</div>
</CodeDescPanel>

## Service as scope

<CodeDescPanel>
<div>

You can configure a service to behave like a nested named scope. At the resolution of this kind of service, a new dedicated named scope is created implicitly for managing the service's dependencies. 

With this feature, you can organize your dependencies around logical groups (named scopes) instead of individual services.

Using `InScopeDefinedBy()`, you can bind services to a defined scope without giving it a name. In this case, the defining service's [implementation type](/docs/getting-started/glossary#service-type--implementation-type) is used for naming the scope.

:::note
The lifetime of the defined scope is attached to the current scope that was used to create the service.
:::

</div>
<div>

<Tabs>
<TabItem value="Define named" label="Define named">

```cs
container.Register<IJob, DbBackup>(options => options
    .DefinesScope("DbBackupScope"));
container.Register<ILogger, ConsoleLogger>(options => options
    .InNamedScope("DbBackupScope"));
container.Register<ILogger, FileLogger>();

var scope = container.BeginScope();

// DbBackup will create a named scope with the name "DbBackupScope".
// the named scope will select ConsoleLogger as it's 
// bound to the named scope's identifier.
IJob job = scope.Resolve<IJob>();

// this will dispose the implicitly created named scope by DbBackup.
scope.Dispose(); 
```

</TabItem>
<TabItem value="Define typed" label="Define typed">

```cs
container.Register<IJob, DbBackup>(options => options
    .DefinesScope());
container.Register<ILogger, ConsoleLogger>(options => options
    .InScopeDefinedBy<DbBackup>());
container.Register<ILogger, FileLogger>();

var scope = container.BeginScope();

// DbBackup will create a named scope with the name typeof(DbBackup).
// the named scope will select ConsoleLogger as it's 
// bound to the named scope's identifier.
IJob job = scope.Resolve<IJob>();

// this will dispose the implicitly created named scope by DbBackup.
scope.Dispose(); 
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Put instance to a scope

<CodeDescPanel>
<div>

You can add an already instantiated service to a scope. The instance's lifetime will be tracked by the given scope.

</div>
<div>

```cs
using var scope = container.BeginScope();
scope.PutInstanceInScope<IJob>(new DbBackup());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

You can disable the tracking by passing `true` for the `withoutDisposalTracking` parameter. In this case, only the strong reference to the instance is dropped when the scope is disposed.

</div>
<div>

```cs
using var scope = container.BeginScope();
scope.PutInstanceInScope<IJob>(new DbBackup(), withoutDisposalTracking: true);
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

You can also give your instance a name to use it like a [named registration](/docs/guides/basics#named-registration):

</div>
<div>

```cs
using var scope = container.BeginScope();
scope.PutInstanceInScope<IDrow>(new DbBackup(), false, name: "DbBackup");
```

</div>
</CodeDescPanel>

:::note
Instances put to a scope will take precedence over existing registrations with the same [service type](/docs/getting-started/glossary#service-type--implementation-type).
:::

## Disposal

<CodeDescPanel>
<div>

The currently resolving scope tracks services that implement either `IDisposable` or `IAsyncDisposable`. This means that when the scope is disposed, all the tracked disposable instances will be disposed with it.

:::note
Disposing the container will dispose all the singleton instances and their dependencies.
:::

</div>
<div>

<Tabs groupId="lifetime-dispose">
<TabItem value="Using" label="Using">

```cs
using (var scope = container.BeginScope())
{
    var disposable = scope.Resolve<DisposableService>();
} // 'disposable' will be disposed when 
  // the using statement ends.
```

</TabItem>
<TabItem value="Dispose" label="Dispose">

```cs
var scope = container.BeginScope();
var disposable = scope.Resolve<DisposableService>();

// 'disposable' will be disposed with the scope.
scope.Dispose();
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

You can disable the disposal tracking on a [service registration](/docs/getting-started/glossary#service-registration--registered-service) with the `.WithoutDisposalTracking()` option.

</div>
<div>

```cs
container.Register<IJob, DbBackup>(options => 
    options.WithoutDisposalTracking());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### Async disposal
As the container and its scopes implement the `IAsyncDisposable` interface, you can dispose them asynchronously when they are used in an `async` context.

Calling `DisposeAsync` disposes both `IDisposable` and `IAsyncDisposable` instances; however, calling `Dispose` only disposes `IDisposable` instances.

</div>
<div>

<Tabs groupId="lifetime-dispose">
<TabItem value="Using" label="Using">

```cs
await using (var scope = container.BeginScope())
{
    var disposable = scope.Resolve<DisposableService>();
} // 'disposable' will be disposed asynchronously 
  // when the using statement ends.
```

</TabItem>
<TabItem value="Dispose" label="Dispose">

```cs
var scope = container.BeginScope();
var disposable = scope.Resolve<DisposableService>();

// 'disposable' will be disposed asynchronously with the scope.
await scope.DisposeAsync();
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### Finalizer delegate
During [service registration](/docs/getting-started/glossary#service-registration--registered-service), you can set a custom finalizer delegate that will be invoked at the service's disposal.

</div>
<div>

```cs
container.Register<IJob, DbBackup>(options => 
    options.WithFinalizer(backup => 
        backup.CloseDbConnection()));
```

</div>
</CodeDescPanel>
