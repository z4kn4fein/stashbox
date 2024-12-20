import CodeDescPanel from '@site/src/components/CodeDescPanel';

# Child containers

With child containers, you can build up parent-child relationships between containers. This means you can have a different subset of services present in a child than in the parent container. 

When a dependency is missing from the child container during a resolution request, the parent will be asked to resolve the missing service. If it's found there, the parent will return only the service's registration, and the resolution request will jump back to the child. Also, child registrations with the same [service type](/docs/getting-started/glossary#service-type--implementation-type) will override the parent's services.

Resolving `IEnumerable<T>` and [decorators](/docs/advanced/decorators) also considers parent containers by default. However, this behavior can be controlled with the [`ResolutionBehavior`](#resolution-behavior) parameter. 

:::info
Child containers are the foundation of the [ASP.NET Core multi-tenant extension](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection#multitenant).
:::

## Example

Here is an example case:
```cs
interface IDependency {}

class B : IDependency {}
class C : IDependency {}

class A 
{
    public A(IDependency dependency)
    { }
}

using (var container = new StashboxContainer())
{
    // register 'A' into the parent container.
    container.Register<A>();

    // register 'B' as a dependency into the parent container.
    container.Register<IDependency, B>();

    var child = container.CreateChildContainer()
    
    // register 'C' as a dependency into the child container.
    child.Register<IDependency, C>();
    
    // 'A' is resolved from the parent and gets
    // 'C' as IDependency because the resolution
    // request was initiated on the child.
    A fromChild = child.Resolve<A>();

    // 'A' gets 'B' as IDependency because the 
    // resolution request was initiated on the parent.
    A fromParent = container.Resolve<A>();
} // using will dispose the parent along with the child.
```
Let's see what's happening when we request `A` from the *child*:
1. `A` not found in the *child*, go up to the *parent* and check there.
2. `A` found in the *parent*, resolve.
3. `A` depends on `IDependency`, go back to the *child* and search `IDependency` implementations.
4. `C` found in the *child*, it does not have any dependencies, instantiate.
5. Inject the new `C` instance into `A`.
6. All dependencies are resolved; return `A`.

When we make the same request on the parent, everything will go as usual because we have all dependencies in place. `B` will be injected into `A`.

:::info
You can [re-configure](/docs/configuration/container-configuration) child containers with the `.Configure()` method. It doesn't affect the parent container's configuration.
:::

## Accessing child containers

You can identify child containers with the `identifier` parameter of `CreateChildContainer()`. Later, you can retrieve the given child container by passing its ID to `GetChildContainer()`.

```cs
using var container = new StashboxContainer();
container.CreateChildContainer("child");
// ...

var child = container.GetChildContainer("child");
```

Also, each child container created by a container is available through the `IStashboxContainer.ChildContainers` propert.

```cs
using var container = new StashboxContainer();
container.CreateChildContainer("child1");
container.CreateChildContainer("child2");
// ...

foreach (var child in container.ChildContainers)
{
    var id = child.Key;
    var childContainer = child.Value;
}
```

## Resolution behavior

You can control which level of the container hierarchy can participate in the service resolution with the `ResolutionBehavior` parameter. 

Possible values:
- `Default`: The default behavior, it's used when the parameter is not specified. Its value is `Parent | Current`, so both the current container (which initiated the resolution request) and its parents can participate in the resolution request's service selection.
- `Parent`: Indicates that parent containers (including all indirect ancestors) can participate in the resolution request's service selection.
- `Current`: Indicates that the current container (which initiated the resolution request) can participate in the service selection.
- `ParentDependency`: Indicates that parent containers (including all indirect ancestors) can only provide dependencies for services that are already selected for resolution.
- `PreferEnumerableInCurrent`: Upon enumerable resolution, when both `Current` and `Parent` behaviors are enabled, and the current container has the appropriate services, the resolution will prefer those and ignore the parent containers. When the current container doesn't have the requested services, the parent containers will serve the request.

```csharp
interface IService {}

class A : IService {}
class B : IService {}

using (var container = new StashboxContainer())
{
    // register 'A' into the parent container.
    container.Register<IService, A>();

    var child = container.CreateChildContainer()
    
    // register 'B' into the child container.
    child.Register<IService, B>();
    
    // 'A' is resolved because only parent
    // can participate in the resolution request.
    IService withParent = child.Resolve<IService>(ResolutionBehavior.Parent);

    // Only 'B' is in the collection because
    // only the caller container can take part
    // in the resolution request.
    IEnumerable<IService> allWithCurrent = child.Resolve<IEnumerable<IService>>(ResolutionBehavior.Current);
    
    // Both 'A' and 'B' is in the collection
    // because both the parent and the caller container
    // participates in the resolution request.
    IEnumerable<IService> all = child.Resolve<IEnumerable<IService>>(ResolutionBehavior.Current | ResolutionBehavior.Parent);
} // using will dispose the parent along with the child.
```

## Re-building singletons
By default, singletons are instantiated and stored only in those containers that registered them. However, you can enable the re-instantiation of singletons in child containers with the `.WithReBuildSingletonsInChildContainer()` [container configuration option](/docs/configuration/container-configuration#re-build-singletons-in-child-containers). 

If it's enabled, all singletons will be re-created in those containers that initiated the resolution request. By this, re-built singletons can use overridden dependencies from child containers. 

Re-building in child containers does not affect the singletons instantiated in the parent container.

```cs
interface IDependency {}

class B : IDependency {}
class C : IDependency {}

class A 
{
    public A(IDependency dependency)
    { }
}

using (var container = new StashboxContainer(options => options.WithReBuildSingletonsInChildContainer()))
{
    // register 'A' as a singleton into the parent container.
    container.RegisterSingleton<A>();

    // register 'B' as a dependency into the parent container.
    container.Register<IDependency, B>();

    // 'A' gets 'B' as IDependency and will be stored
    // in the parent container as a singleton.
    A fromParent = container.Resolve<A>();

    var child = container.CreateChildContainer();
    
    // register 'C' as a dependency into the child container.
    child.Register<IDependency, C>();

    // a new 'A' singleton will be created in
    // the child container with 'C' as IDependency.
    A fromChild = child.Resolve<A>();
} // using will dispose the parent along with the child.
```

## Nested child containers

<CodeDescPanel>
<div>

You can build up a hierarchical tree structure from containers by creating more child containers with the `.CreateChildContainer()` method.

</div>
<div>

```cs
using var container = new StashboxContainer();

var child1 = container.CreateChildContainer();
var child2 = child1.CreateChildContainer();
```

</div>
</CodeDescPanel>

## Dispose

By default, the parent container's disposal also disposes its child containers. You can control this behavior with the `CreateChildContainer()` method's `attachToParent` boolean parameter.

```cs
using (var container = new StashboxContainer())
{
    using (var child1 = container.CreateChildContainer(attachToParent: false))
    {
    } // child1 will be disposed only once here.
    
    var child2 = container.CreateChildContainer();
    var child3 = container.CreateChildContainer();
} // using will dispose the parent along with child2 and child3.
```

You can safely dispose a child even if it's attached to its parent, in this case the parent's disposal will not dispose the already disposed child.

```cs
using (var container = new StashboxContainer())
{
    using (var child1 = container.CreateChildContainer())
    {
    } // child1 will be disposed only once here.
    
    var child2 = container.CreateChildContainer();
} // using will dispose only the parent and child2.
```