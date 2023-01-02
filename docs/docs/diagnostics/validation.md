import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Validation

Stashbox has validation routines that help you detect and solve common misconfiguration issues. You can verify the container's actual state with its `.Validate()` method. This method walks through the whole [resolution tree](/docs/getting-started/glossary#resolution-tree) and collects all the issues into an `AggregateException`.

## Registration validation
The container validates the given types during registration and throws the following exceptions when the validation fails.
### InvalidRegistrationException
1. *When the [implementation type](/docs/getting-started/glossary#service-type--implementation-type) is not resolvable:* (it's an interface or abstract class registered like: `Register<IService>()`):
  ```
  The type Namespace.IService could not be resolved. It's probably an interface, abstract class, or primitive type.
  ```
2. *When the [implementation type](/docs/getting-started/glossary#service-type--implementation-type) does not implement the [service type](/docs/getting-started/glossary#service-type--implementation-type)*:
  ```
  The type Namespace.MotorCycle does not implement the '[service type](/docs/getting-started/glossary#service-type--implementation-type)' Namespace.ICar.
  ```

### ServiceAlreadyRegisteredException
*When the given [implementation type](/docs/getting-started/glossary#service-type--implementation-type) is already registered* and the `RegistrationBehavior` [container configuration option](configuration/container-configuration?id=registration-behavior) is set to `ThrowException`:
```
The type Namespace.Service is already registered.
```

## Resolution validation
During the construction of the [resolution tree](/docs/getting-started/glossary#resolution-tree), the container continuously checks its actual state to ensure stability. When any of the following issues occur, the container throws a `ResolutionFailedException`.

1. *When a dependency is missing from the [resolution tree](/docs/getting-started/glossary#resolution-tree)*:

    <Tabs>
    <TabItem value="Parameter" label="Parameter">

    ```cs
    class Service
    {
        public Service(Dependency dep) { }

        public Service(Dependency2 dep2) { }
    }

    container.Register<Service>();
    var service = container.Resolve<Service>();
    ```
    This will result in the following exception message:

    ```
    Could not resolve type Namespace.Service.
    Constructor Void .ctor(Dependency) found with unresolvable parameter: (Namespace.Dependency)dep.
    Constructor Void .ctor(Dependency2) found with unresolvable parameter: (Namespace.Dependency2)dep2.
    ```

    </TabItem>
    <TabItem value="Property / field" label="Property / field">

    ```cs
    class Service
    {
        public Dependency Dep { get; set; }
    }

    container.Register<Service>(options => options.WithDependencyBinding(s => s.Dep));
    var service = container.Resolve<Service>();
    ```
    This will show the following message:
    ```
    Could not resolve type Namespace.Service.
    Unresolvable property: (Namespace.Dependency)Dep.
    ```

    </TabItem>
    </Tabs>

2. *When the requested type is unresolvable:* (e.g., it doesn't have a public constructor)

    ```
    Could not resolve type Namespace.Service.
    Service is not registered or unresolvable type requested.
    ```

## Lifetime validation
This validation enforces the following rules, and when they are being violated, the container throws a `LifetimeValidationFailedException`.
1. *When a scoped service is requested from the [root scope](/docs/getting-started/glossary#root-scope)*. As the [root scope's](/docs/getting-started/glossary#root-scope) lifetime is bound to the container's lifetime, this action would unintentionally promote the scoped service's lifetime to singleton:
  ```
  Resolution of Namespace.Service (ScopedLifetime) from the '[root scope](/docs/getting-started/glossary#root-scope)' is not allowed, 
  that would promote the service's lifetime to a singleton.
  ```

2. *When the life-span of a dependency is shorter than its parent's*. It's called [captive dependency](https://blog.ploeh.dk/2014/06/02/captive-dependency/). Every lifetime has a `LifeSpan` value, which determines how long the related service lives. The main rule is that services may not contain dependencies with shorter life-spans like singletons should not depend on scoped services. The only exception is the life-span value `0`, which indicates that the related service is state-less and could be injected into any service. 

    These are the `LifeSpan` values of the pre-defined lifetimes: 
     - **Singleton**: 20
     - **Scoped**: 10
     - **NamedScope**: 10
     - **PerRequest**: 0
     - **PerScopedRequest**: 0
     - **Transient**: 0

  In case of a failed validation the exception message would be:
  ```
  The life-span of Namespace.Service (ScopedLifetime|10) is shorter than 
  its direct or indirect parent's Namespace.Dependency (Singleton|20). 
  This could lead to incidental lifetime promotions with longer life-span, 
  it's recommended to double-check your lifetime configurations.
  ```

## Circular dependency
When the container encounters a circular dependency loop in the [resolution tree](/docs/getting-started/glossary#resolution-tree), it throws a `CircularDependencyException` with every diagnostic detail included.

```cs
class Service1
{
    public Service1(Service2 service2) { }
}

class Service2
{
    public Service2(Service1 service1) { }
}

container.Register<Service1>();
container.Register<Service2>();
var service = container.Resolve<Service1>();
```
The exception message is:  
```
Circular dependency detected during the resolution of Namespace.Service1.
```

## Other exceptions
### CompositionRootNotFoundException
This exception pops up when we try to compose an assembly, but it doesn't contain an `ICompositionRoot` implementation.
```cs
container.ComposeAssembly(typeof(Service).Assembly);
```
The exception message is:  
```
No ICompositionRoot found in the given assembly: {your-assembly-name}
```

### ConstructorNotFoundException
During the registration phase, when you are using the `WithConstructorByArgumentTypes()` or `WithConstructorByArguments()` options, you can accidentally point to a non-existing constructor and in that case, the container throws a `ConstructorNotFoundException`.

```cs
class Service
{
    public Service(Dependency dep) { }
}

container.Register<Service>(options => options.WithConstructorByArgumentTypes(typeof(string), typeof(int)));
```
The exception message is:  
```
Constructor not found for Namespace.Service with the given argument types: System.String, System.Int32.
```