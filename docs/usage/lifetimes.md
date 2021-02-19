# Lifetimes

Lifetime management is the concept of controlling how long a service's instances will live (from instantiation to [disposal](usage/scopes?id=disposal)) and how they will be reused between resolution requests.

!> Choosing the right lifetime helps you avoid [captive dependencies](diagnostics/validation?id=lifetime-validation).


<!-- panels:start -->

<!-- div:title-panel -->
## Default lifetime

<!-- div:left-panel -->
When you are not specifying a lifetime during registration, Stashbox will use the default lifetime. By default, it's set to **Transient**, but you can override it with the `.WithDefaultLifetime()` [container configuration option](configuration/container-configuration?id=default-lifetime). 

You can choose either from the pre-defined lifetimes in the `Lifetimes` static class or use a [custom one](#custom-lifetime).
<!-- div:right-panel -->

<!-- tabs:start -->
#### **Transient (default)**
```cs
var container = new StashboxContainer(options => options
    .WithDefaultLifetime(Lifetimes.Transient));
```

#### **Singleton**
```cs
var container = new StashboxContainer(options => options
    .WithDefaultLifetime(Lifetimes.Singleton));
```

#### **Scoped**
```cs
var container = new StashboxContainer(options => options
    .WithDefaultLifetime(Lifetimes.Scoped));
```
<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Transient lifetime

<!-- div:left-panel -->
A new instance will be created for every resolution request. If a transient is referred by multiple consumers in the same resolution tree, each of them will get its instance.

<!-- div:right-panel -->
```cs
container.Register<IJob, DbBackup>(options => options
    .WithLifetime(Lifetimes.Transient));
```

<!-- panels:end -->

!> Transient services are not tracked for disposal by default but this feature can be turned on with `.WithDisposableTransientTracking()` [option](configuration/configuration/container-configuration?id=tracking-disposable-transients).

?> When the tracking of disposable transients is enabled, they will be tracked and disposed by the actual [scope](usage/scopes) on which the resolution request was initiated or by the root scope when the resolution request was made on the container.

<!-- panels:start -->

<!-- div:title-panel -->
## Singleton lifetime

<!-- div:left-panel -->
A single instance will be created and reused for every resolution request and injected into every consumer.

?> Singleton services will be disposed when the container (root scope) is disposed.

<!-- div:right-panel -->

<!-- tabs:start -->

#### **Longer form**
```cs
container.Register<IJob, DbBackup>(options => options
    .WithLifetime(Lifetimes.Singleton));
```

#### **Shorter form**
```cs
container.RegisterSingleton<IJob, DbBackup>();
```

<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Scoped lifetime

<!-- div:left-panel -->
A new instance is created for each [scope](usage/scopes), and that instance will be returned (and reused) for every resolution request initiated on the given scope. It's like the singleton lifetime within a scope. 

?> Scoped services will be disposed when their scope is disposed.

<!-- div:right-panel -->

<!-- tabs:start -->

#### **Longer form**
```cs
container.Register<IJob, DbBackup>(options => options
    .WithLifetime(Lifetimes.Scoped));

using var scope = container.BeginScope();
IJob job = scope.Resolve<IJob>();
```

#### **Shorter form**
```cs
container.RegisterScoped<IJob, DbBackup>();

using var scope = container.BeginScope();
IJob job = scope.Resolve<IJob>();
```

<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Named scope lifetime

<!-- div:left-panel -->
It is the same as the scoped lifetime, except that the given service will be selected only when the resolution request is initiated on a scope with the same name.

You can also let a service [define](usage/scopes?id=service-as-scope) its own named scope within itself. Later this scope can be referred to in named scope

?> Services with named scope lifetime will be disposed when the related named scope is disposed.
<!-- div:right-panel -->

<!-- tabs:start -->

#### **Named scope**
```cs
container.Register<IJob, DbBackup>(options => options
    .InNamedScope("DbScope"));

using var scope = container.BeginScope("DbScope");
IJob job = scope.Resolve<IJob>();
```

#### **Defined scope**
```cs
container.Register<DbJobExecutor>(options => options
    .DefinesScope());

ontainer.Register<IJob, DbBackup>(options => options
    .InScopeDefinedBy<DbJobExecutor>());

// the executor will begin a new scope within itself
// when it gets resolved and DbBackup will be selected
// and attached to that scope instead.
using var scope = container.BeginScope();
DbJobExecutor executor = scope.Resolve<DbJobExecutor>();
```

#### **Defined scope with name**
```cs
container.Register<DbJobExecutor>(options => options
    .DefinesScope("DbScope"));

ontainer.Register<IJob, DbBackup>(options => options
    .InNamedScope("DbScope"));

// the executor will begin a new scope within itself
// when it gets resolved and DbBackup will be selected
// and attached to that scope instead.
using var scope = container.BeginScope();
DbJobExecutor executor = scope.Resolve<DbJobExecutor>();
```

<!-- tabs:end -->

<!-- panels:end -->

<!-- panels:start -->

<!-- div:title-panel -->
## Per scoped request lifetime

<!-- div:left-panel -->
The requested service will behave like a singleton with this lifetime, but only within a scoped dependency request. That means every scoped service will get a new exclusive instance that will be used by its sub-dependencies as well.

<!-- div:right-panel -->

```cs
container.Register<IJob, DbBackup>(options => options
    .WithPerScopedRequestLifetime());
```

<!-- panels:end -->

## Custom lifetime
Suppose you'd like to use a custom lifetime. In that case, you can create your implementation by inheriting either from `FactoryLifetimeDescriptor` or from `ExpressionLifetimeDescriptor`, depending on how do you want to manage the given service instances. Then you can pass it to the `WithLifetime()` configuration method.

- **ExpressionLifetimeDescriptor**: With this, you can build your lifetime with the expression form of the service instantiation.
  ```cs
  class CustomLifetime : ExpressionLifetimeDescriptor
  {
      protected override Expression ApplyLifetime(
          Expression expression, // The expression which describes the service creation
          ServiceRegistration serviceRegistration, 
          ResolutionContext resolutionContext, 
          Type requestedType)
      {
          // Lifetime managing functionality
      }
  }
  ```

- **FactoryLifetimeDescriptor**: With this, you can build your lifetime based on a pre-compiled factory delegate used for service instantiation.
  ```cs
  class CustomLifetime : FactoryLifetimeDescriptor
  {
      protected override Expression ApplyLifetime(
          Func<IResolutionScope, object> factory, // The factory used for service creation
          ServiceRegistration serviceRegistration, 
          ResolutionContext resolutionContext, 
          Type requestedType)
      {
          // Lifetime managing functionality
      }
  }
  ```
Then you can use your lifetime like this:
```cs
container.Register<IJob, DbBackup>(options => options.WithLifetime(new CustomLifetime()));
```