# Child container
With child containers, you can build up parent-child relationships between containers. It means you can have a different subset of services present in child and parent containers. When something is missing from a child container during a resolution request, the parent will be asked to resolve the missing service.

## Example

Here are the actions of an example case:
1. Resolution request on *child* container for `A`.
2. `A` not found in the *child*, go up to the *parent* and check there.
3. `A` found in the *parent*, resolve.
4. `A` is depending on `B`, go back to the *child* and search `B`.
5. `B` found in the *child*, resolve.
6. All dependencies are resolved; return `A`.

Let's see this with code:
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
    // register 'A' into the main container.
    container.Register<A>();

    // register 'B' as a dependency into the main container.
    container.Register<IDependency, B>();

    using (var child = container.CreateChildContainer())
    {
        // register 'C' as a dependency into the child container.
        child.Register<IDependency, C>();

        // 'A' is resolved from the parent and gets
        // 'C' as IDependency because the resolution
        // request was initiated on the child.
        child.Resolve<A>();
    }

    // 'A' gets 'B' as IDependency because the 
    // resolution request was initiated on the main.
    container.Resolve<A>();
}
```
?> You can [re-configure](configuration/container-configuration) child containers with the `.Configure()` method. It doesn't affect the parent container's configuration.

## Re-building singletons
By default, singletons are instantiated and stored only in those containers that registered them. However, you can enable the re-instantiation of singletons in child containers with the `.WithReBuildSingletonsInChildContainer()` [container configuration option](configuration/container-configuration?id=re-build-singletons-in-child-containers). 

If it's enabled, all singletons will be re-created within those containers that initiated the resolution request. It means that re-built singletons can use overridden dependencies from child containers. 

It does not affect the singletons instantiated in the parent container.

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
    // register 'A' as a singleton into the main container.
    container.RegisterSingleton<A>();

    // register 'B' as a dependency into the main container.
    container.Register<IDependency, B>();

    // 'A' gets 'B' as IDependency and will be stored
    // in the main container as a singleton.
    container.Resolve<A>();

    using (var child = container.CreateChildContainer())
    {
        // register 'C' as a dependency into the child container.
        child.Register<IDependency, C>();

        // a new 'A' singleton will be created in 
        // the child container with 'C' as IDependency
        child.Resolve<A>();
    } 
    // using will dispose the child and the 
    // newly created singleton instance of 'A'
}
// using will dispose the parent and the 
// original singleton instance of 'A'
```

## Nested child containers

<!-- panels:start -->

<!-- div:left-panel -->
You can build up a hierarchical tree structure from containers as a child can create other child containers with the `.CreateChildContainer()` method.

?> This feature is the core of the [multi-tenant package](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection#multitenant).
<!-- div:right-panel -->
```cs
using(var child1 = container.CreateChildContainer())
{
    using(var child2 = child1.CreateChildContainer())
    { }
}
```
<!-- panels:end -->