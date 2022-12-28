import CodeDescPanel from '@site/src/components/CodeDescPanel'; 

# Glossary

The following terms and definitions are used in this documentation.

## Service Type | Implementation Type
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

## Service Registration | Registered Service
It's an entity created by the container when a service is registered. The service registration stores required information about how to instantiate the service, e.g., reflected type information, name, lifetime, conditions, and more.

<CodeDescPanel>
<div>

In this example, we are registering a named service. The container will create a service registration entity to store the type mapping and the name. Later, the container will find the registration by combining the *Service type* and the *name*.

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

## Injectable Dependency

<CodeDescPanel>
<div>

It's a constructor / method argument or a property / field of a registered *Implementation type* that gets evaluated (*injected*) by the container during the service's construction.

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

## Resolution Tree
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
The container instantiates those services first that don't have any dependencies. `C` and `D` will be injected into `B`. Then, a new `C` is instantiated (if it's [transient](usage/lifetimes?id=transient-lifetime)) and injected into `A` along with the previously created `B`.

## Dependency Resolver
It's the container itself or the [current scope](usage/scopes), depending on which was asked to resolve a particular service. They are both implementing Stashbox's `IDependencyResolver` and the .NET framework's `IServiceProvider` interface and can be used for service resolution.

:::info
Stashbox implicitly injects the [current scope](usage/scopes) wherever `IDependencyResolver` or `IServiceProvider` is requested.
:::

## Root Scope
It's the [main scope](usage/scopes) created inside every container instance. It stores and handles the lifetime of all singletons. It's the base of each subsequent scope created by the container with the `.BeginScope()` method.

:::caution
[Scoped services](usage/lifetimes?id=scoped-lifetime) requested from the container (and not from a [scope](usage/scopes)) are managed by the root scope. This can lead to issues because their lifetime will effectively switch to singleton. Always be sure that you are not resolving scoped services directly from the container, only from a [scope](usage/scopes). This case is monitored by the [lifetime](diagnostics/validation?id=lifetime-validation) validation rule when it's [enabled](configuration/container-configuration?id=lifetime-validation). 
:::

## Named Resolution

<!-- panels:start -->
<!-- div:left-panel -->
It's a resolution request for a named service. The same applies, when the container sees that a dependency in the resolution tree has a name (set by [attributes](usage/service-resolution?id=attributes) or [bindings](usage/service-resolution?id=dependency-binding)), it will search for a matching [Named registration](usage/basics?id=named-registration) to inject.
<!-- div:right-panel -->
```cs
container.Register<IService, Implementation>("Example");
// the named request.
var service = container.Resolve<IService>("Example");
```
<!-- tabs:end -->
<!-- panels:end -->

## Self registration

<!-- panels:start -->
<!-- div:left-panel -->
It's a registration that's mapped to itself, which means its service and implementation type is the same.
<!-- div:right-panel -->
```cs
// equivalent to container.Register<Implementation, Implementation>();
container.Register<Implementation>();
```
<!-- tabs:end -->
<!-- panels:end -->