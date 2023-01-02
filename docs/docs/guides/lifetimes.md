import CodeDescPanel from '@site/src/components/CodeDescPanel';
import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Lifetimes

Lifetime management is the concept of controlling how long a service's instances will live (from instantiation to [disposal](/docs/guides/scopes#disposal)) and how they will be reused between resolution requests.

:::info
Choosing the right lifetime helps you avoid [captive dependencies](/docs/diagnostics/validation#lifetime-validation).
:::

## Default lifetime

<CodeDescPanel>
<div>

When you are not specifying a lifetime during registration, Stashbox will use the default lifetime. By default, it's set to **Transient**, but you can override it with the `.WithDefaultLifetime()` [container configuration option](/docs/configuration/container-configuration#default-lifetime). 

You can choose either from the pre-defined lifetimes defined in the `Lifetimes` static class or use a [custom one](#custom-lifetime).

</div>
<div>

<Tabs>
<TabItem value="Transient (default)" label="Transient (default)">

```cs
var container = new StashboxContainer(options => options
    .WithDefaultLifetime(Lifetimes.Transient));
```

</TabItem>
<TabItem value="Singleton" label="Singleton">

```cs
var container = new StashboxContainer(options => options
    .WithDefaultLifetime(Lifetimes.Singleton));
```

</TabItem>
<TabItem value="Scoped" label="Scoped">

```cs
var container = new StashboxContainer(options => options
    .WithDefaultLifetime(Lifetimes.Scoped));
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Transient lifetime

<CodeDescPanel>
<div>

A new instance will be created for every resolution request. If a transient is referred by multiple consumers in the same [resolution tree](/docs/getting-started/glossary#resolution-tree), each of them will get a new instance.

</div>
<div>

```cs
container.Register<IJob, DbBackup>(options => options
    .WithLifetime(Lifetimes.Transient));
```

</div>
</CodeDescPanel>

:::info
Transient services are not tracked for disposal by default, but this feature can be turned on with the `.WithDisposableTransientTracking()` [container configuration option](/docs/configuration/container-configuration#tracking-disposable-transients). When it's enabled, the current [scope](/docs/guides/scopes) on which the resolution request was initiated takes the responsibility to track and dispose the transient services.
:::

## Singleton lifetime

<CodeDescPanel>
<div>

A single instance will be created and reused for every resolution request and injected into every consumer.

:::note
Singleton services are disposed when the container ([root scope](/docs/getting-started/glossary#root-scope)) is being disposed.
:::

</div>
<div>

<Tabs groupId="lifetime-forms">
<TabItem value="Longer form" label="Longer form">

```cs
container.Register<IJob, DbBackup>(options => options
    .WithLifetime(Lifetimes.Singleton));
```

</TabItem>
<TabItem value="Shorter form" label="Shorter form">

```cs
container.RegisterSingleton<IJob, DbBackup>();
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Scoped lifetime

<CodeDescPanel>
<div>

A new instance is created for each [scope](/docs/guides/scopes), and that instance will be returned for every resolution request initiated on the given scope. It's like a singleton lifetime within the scope. 

:::note
Scoped services are disposed when their scope is being disposed.
:::

</div>
<div>

<Tabs groupId="lifetime-forms">
<TabItem value="Longer form" label="Longer form">

```cs
container.Register<IJob, DbBackup>(options => options
    .WithLifetime(Lifetimes.Scoped));

using var scope = container.BeginScope();
IJob job = scope.Resolve<IJob>();
```

</TabItem>
<TabItem value="Shorter form" label="Shorter form">

```cs
container.RegisterScoped<IJob, DbBackup>();

using var scope = container.BeginScope();
IJob job = scope.Resolve<IJob>();
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Named scope lifetime

<CodeDescPanel>
<div>

It is the same as the scoped lifetime, except that the given service will be selected only when the resolution request is initiated by a scope with the same name.

You can also let a service [define](/docs/guides/scopes#service-as-scope) its own named scope within itself. During registration, this scope can be referred to by its name upon using a named scope lifetime.

:::note
Services with named scope lifetime are disposed when the related named scope is being disposed.
:::

</div>
<div>

<Tabs>
<TabItem value="Named" label="Named">

```cs
container.Register<IJob, DbBackup>(options => options
    .InNamedScope("DbScope"));

using var scope = container.BeginScope("DbScope");
IJob job = scope.Resolve<IJob>();
```

</TabItem>
<TabItem value="Defined" label="Defined">

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

</TabItem>
<TabItem value="Defined with name" label="Defined with name">

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

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Per-request lifetime

<CodeDescPanel>
<div>

The requested service will be reused within the whole resolution request. For each individual request a new instance will be created.

</div>
<div>

```cs
container.Register<IJob, DbBackup>(options => options
    .WithPerRequestLifetime());
```

</div>
</CodeDescPanel>

## Per-scoped request lifetime

<CodeDescPanel>
<div>

The requested service will behave like a singleton, but only within a scoped dependency request. This means every scoped service will get a new exclusive instance that will be used by its sub-dependencies as well.

</div>
<div>

```cs
container.Register<IJob, DbBackup>(options => options
    .WithPerScopedRequestLifetime());
```

</div>
</CodeDescPanel>

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