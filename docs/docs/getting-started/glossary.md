import CodeDescPanel from '@site/src/components/CodeDescPanel'; 

# Glossary

The following terms and definitions are used in this documentation.

## Service type | Implementation type
The *Service type* is usually an interface or an abstract class type used for service resolution or dependency injection. The *Implementation type* is the actual type registered to the *Service type*. A registration maps the *Service type* to an *Implementation type*. The *Implementation type* must implement or extend the *Service type*. 

<CodeDescPanel>
<div>

Example where a *Service type* is mapped to an *Implementation type*:

</div>
<div>

```cs
container.Register<IService, Implementation>();
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

The *Service type* used for requesting a service from the container:

</div>
<div>

```cs
container.Resolve<IService>(); // returns Implementation
```

</div>
</CodeDescPanel>

## Service registration | Registered service
It's an entity created by Stashbox when a service is registered. The service registration stores required information about how to instantiate the service, e.g., reflected type information, name, lifetime, conditions, and more.

<CodeDescPanel>
<div>

In this example, we are registering a named service. The container will create a service registration entity to store the type mapping and the name. During resolution, the container will find the registration by checking for the *Service type* and the *name*.

</div>
<div>

```cs
// the registration entity will look like: 
// IService => Implementation, name: Example
container.Register<IService, Implementation>("Example");
var service = container.Resolve<IService>("Example");
```

</div>
</CodeDescPanel>

## Injectable dependency

<CodeDescPanel>
<div>

It's a constructor/method argument or a property/field of a registered *Implementation type* that gets evaluated (*injected*) by Stashbox during the service's construction.

In this example, `Implementation` has an `IDependency` *injectable dependency* in its constructor.

</div>
<div>

```cs
class Implementation : IService
{
    public Implementation(IDependency dependency) 
    { }
}
```

</div>
</CodeDescPanel>

## Resolution tree
It's the structural representation of a service's resolution process. It describes the instantiation order of the dependencies required to resolve the desired type.

Let's see through an example:
```cs
class A
{
    public A(B b, C c) { }
}

class B
{
    public B(C c, D d) { }
}

class C { }
class D { }
```
When we request the service `A`, the container constructs the following resolution tree based on the dependencies and sub-dependencies.
```
        A
      /   \
     B     C
   /   \
  C     D
```
The container instantiates those services first that don't have any dependencies. `C` and `D` will be injected into `B`. Then, a new `C` is instantiated (if it's [transient](/docs/guides/lifetimes#transient-lifetime)) and injected into `A` along with the previously created `B`.

## Dependency resolver
It's the container itself or the [current scope](/docs/guides/scopes), depending on which was asked to resolve a particular service. They are both implementing Stashbox's `IDependencyResolver` and the .NET framework's `IServiceProvider` interface and can be used for service resolution.

:::info
Stashbox implicitly injects the [current scope](/docs/guides/scopes) wherever `IDependencyResolver` or `IServiceProvider` is requested.
:::

## Root scope
It's the [main scope](/docs/guides/scopes) created inside every container instance. It stores and handles the lifetime of all singletons. It's the base of each subsequent scope created by the container with the `.BeginScope()` method.

:::caution
[Scoped services](/docs/guides/lifetimes#scoped-lifetime) requested from the container (and not from a [scope](/docs/guides/scopes)) are managed by the root scope. This can lead to issues because their lifetime will effectively switch to singleton. Always be sure that you don't resolve scoped services directly from the container, only from a [scope](/docs/guides/scopes). This case is monitored by the [lifetime](/docs/diagnostics/validation#lifetime-validation) validation rule when it's [enabled](/docs/configuration/container-configuration#lifetime-validation). 
:::

## Named resolution

<CodeDescPanel>
<div>

It's a resolution request for a named service. The same applies, when the container sees a dependency in the resolution tree with a name (set by [attributes](/docs/guides/service-resolution#attributes) or [bindings](/docs/guides/service-resolution#dependency-binding)); it will search for a matching [Named registration](/docs/guides/basics#named-registration) to inject.

</div>
<div>

```cs
container.Register<IService, Implementation>("Example");
// the named request.
var service = container.Resolve<IService>("Example");
```

</div>
</CodeDescPanel>

## Self registration

<CodeDescPanel>
<div>

It's a service registration that's mapped to itself. This means its service and implementation type is the same.

</div>
<div>

```cs
// equivalent to container.Register<Implementation, Implementation>();
container.Register<Implementation>();
```

</div>
</CodeDescPanel>