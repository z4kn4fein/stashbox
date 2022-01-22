# Scopes
A scope is Stashbox's implementation of the unit-of-work pattern; it encapsulates a given unit used to resolve and store instances required for a given work. When a scoped service is resolved or injected, the scope ensures that it gets instantiated only once within the scope's lifetime. When the work is finished, the scope cleans up the resources by disposing every tracked disposable instance.

A web application is a fair usage example for scopes as it has a well-defined execution unit that can be bound to a scope - the HTTP request. Every request could have its unique scope attached to the request's lifetime. When a request ends, the scope gets closed, and all the scoped instances will be disposed.

<!-- panels:start -->
## Creating a scope
<!-- div:left-panel -->
You can create a scope from the container by calling its `.BeginScope()` method.

Scopes can be **nested**, which means you can begin sub-scopes from existing ones with their `.BeginScope()` method. 

Scoped service instances are not shared across parent and sub-scope relations.

Nested scopes can be **attached to their parent's lifetime**, which means when the parent gets disposed all child scopes attached to it will be disposed.

Scopes are `IDisposable`; they track all `IDisposable` instances they resolved, so calling their `Dispose()` method or wrapping them in `using` statements is a crucial part of their service's lifetime management.
<!-- div:right-panel -->

<!-- tabs:start -->
#### **Create**
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

#### **Nested**
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

#### **Nested attached**
```cs
container.RegisterScoped<IJob, DbBackup>();

var parent = container.BeginScope();
var sub = parent.BeginScope(attachToParent: true);

// sub will also be disposed with the scope.
scope.Dispose(); 
```
<!-- tabs:end -->

<!-- panels:end -->

## Named scopes

<!-- panels:start -->

<!-- div:left-panel -->
There might be cases where you don't want to use a service globally across every scope, only in specific ones. 

For this reason, there is an option to identify a scope with a **name**. With this, you can differentiate specific scope groups from other scopes.

To mark those services that only a named scope should use you can set the service's lifetime to [named scope lifetime](usage/lifetimes?id=named-scope-lifetime) initialized with the **scope's name**:

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

Service instances with named scope lifetime are **shared across parent and sub-scope relations** when the resolution request is initiated either from the named parent scope or from one of its sub-scopes.

If you request a name-scoped service from an un-named scope, you'll get an error or no result (depending on the configuration) because those services are selectable only by named scopes with a matching name.

<!-- div:right-panel -->
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
<!-- panels:end -->

## Service as scope

<!-- panels:start -->

<!-- div:left-panel -->

You can configure a service to behave like a nested named scope. It means that with the service's resolution, a new dedicated named scope will be created implicitly for managing the service's dependencies. 

With this feature, you can organize your dependencies around logical groups (named scopes) instead of individual services.

You can also bind services to a defined scope without giving it a name explicitly by using `InScopeDefinedBy()`. In this case, the defining service's implementation type will be used for naming the scope.

?> The lifetime of the defined scope will be attached to the current scope that was used to create the service.

<!-- div:right-panel -->

<!-- tabs:start -->

#### **Define named**
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

#### **Define typed**
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
<!-- tabs:end -->

<!-- panels:end -->

## Put instance to a scope

<!-- panels:start -->

<!-- div:left-panel -->
You have the option to add an already instantiated service to a scope. It means that the instance's lifetime will be tracked by the given scope.
<!-- div:right-panel -->
```cs
using var scope = container.BeginScope();
scope.PutInstanceInScope<IJob>(new DbBackup());
```

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
You can disable the tracking by passing `true` for the `withoutDisposalTracking` parameter. In this case, only the strong reference to the instance will be dropped when the scope is disposed.
<!-- div:right-panel -->
```cs
using var scope = container.BeginScope();
scope.PutInstanceInScope<IJob>(new DbBackup(), withoutDisposalTracking: true);
```
<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
You can also give your instance a name to use it like a [named registration](usage/basics?id=named-registration):
<!-- div:right-panel -->
```cs
using var scope = container.BeginScope();
scope.PutInstanceInScope<IDrow>(new DbBackup(), false, name: "DbBackup");
```
<!-- panels:end -->

!> Instances added to a scope this way would override existing registrations with the same service type.

## Disposal

<!-- panels:start -->

<!-- div:left-panel -->

Services that implement either `IDisposable` or `IAsyncDisposable` are tracked by the currently resolving scope. This means that when the scope is being disposed, all the tracked disposable instances will be disposed with it.

?> Disposing the container will dispose all the singleton instances.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Using**
```cs
using (var scope = container.BeginScope())
{
    var disposable = scope.Resolve<DisposableService>();
} // 'disposable' will be disposed when the using statement ends.
```

#### **Dispose**
```cs
var scope = container.BeginScope();
var disposable = scope.Resolve<DisposableService>();

// 'disposable' will be disposed with the scope.
scope.Dispose();
```

<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->
You can disable the tracking for disposal on a service registration with the `.WithoutDisposalTracking()` option.

<!-- div:right-panel -->
```cs
container.Register<IJob, DbBackup>(options => options.WithoutDisposalTracking());
```

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->

### Async disposal
As either the container and its scopes are implementing the `IAsyncDisposable` interface, you have the option to dispose them asynchronously when they are used in an `async` context.

Calling `DisposeAsync` disposes both `IDisposable` and `IAsyncDisposable` instances; however, calling `Dispose` only disposes the `IDisposable` instances.

<!-- div:right-panel -->

<!-- tabs:start -->
#### **Using**
```cs
await using (var scope = container.BeginScope())
{
    var disposable = scope.Resolve<DisposableService>();
} // 'disposable' will be disposed asynchronously when the using statement ends.
```

#### **Dispose**
```cs
var scope = container.BeginScope();
var disposable = scope.Resolve<DisposableService>();

// 'disposable' will be disposed asynchronously with the scope.
await scope.DisposeAsync();
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:left-panel -->

### Finalizer delegate
During service registration, you can set a custom finalizer delegate invoked at the service's disposal.
<!-- div:right-panel -->
```cs
container.Register<IJob, DbBackup>(options => 
    options.WithFinalizer(backup => backup.CloseDbConnection()));
```
<!-- panels:end -->