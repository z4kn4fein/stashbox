# Exceptions

When something goes wrong with the internal state of the container, it throws a specific exception to let you know where the issue lies.

## ResolutionFailedException
This exception could pop up:
- **When a dependency is missing from the resolution tree**:
    ```cs
    class Service
    {
        public Service(Dependency dep) { }

        public Service(Dependency2 dep2) { }
    }

    container.Register<Service>().Resolve<Service>();
    ```
    This will result a `ResolutionFailedException` with the following message:

    ```
    Could not resolve type Namespace.Service.
    Constructor Void .ctor(Dependency) found with unresolvable parameter: (Namespace.Dependency)dep.
    Constructor Void .ctor(Dependency2) found with unresolvable parameter: (Namespace.Dependency2)dep2.
    ```

- **When the requested type is unresolvable**:  
    When your top-level resolution request points to a service which is not resolvable (e.g. not registered), the following exception message shows:

    ```
    Could not resolve type Namespace.Service.
    Service is not registered or unresolvable type requested.
    ```

- **When a member is unresolvable**:  
    Let's see what happens when we are facing this issue with the following type:
    ```cs
    class Service
    {
        public Dependency Dep { get; set; }
    }

    container.Register<Service>(c => c.InjectMember(s => s.Dep)).Resolve<Service>();
    ```
    In this case, we'll get the following message:

    ```
    Could not resolve type Namespace.Service.
    Unresolvable property: (Namespace.Dependency)Dep.
    ```

## CircularDependencyException
When this exception occurs that means the container noticed an infinite dependency loop in the resolution tree.
```cs
class Service1
{
    public Service1(Service2 service2) { }
}

class Service2
{
    public Service2(Service1 service1) { }
}

container.Register<Service1>().Register<Service2>().Resolve<Service1>();
```
The exception message is:  
```
Circular dependency detected during the resolution of Namespace.Service1.
```

## CompositionRootNotFoundException
This exception pops up when we try to compose an assembly but it doesn't contain an `ICompositionRoot` implementation.
```cs
container.ComposeAssembly(typeof(Service).Assembly);
```
The exception message is:  
```
No ICompositionRoot found in the given assembly: {your-assembly-name}
```

## ConstructorNotFoundException
During the registration phase when you are using the `WithConstructorByArgumentTypes()` or `WithConstructorByArguments()` options, you can accidentally point to a non-existing constructor and in that case, the container throws a `ConstructorNotFoundException`.

```cs
class Service
{
    public Service(Dependency dep) { }
}

container.Register<Service>(c => c.WithConstructorByArgumentTypes(typeof(string), typeof(int)));
```
The exception message is:  
```
Constructor not found for Namespace.Service with the given argument types: System.String, System.Int32.
```

## ServiceAlreadyRegisteredException
When you are registering a service that's already registered and the `RegistrationBehavior` [option of the container](configuration/container-configuration?id=registration-behavior) is set to `ThrowException` it'll throw a `ServiceAlreadyRegisteredException`.
```cs
var container = new StashboxContainer(c => c.WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));
container.Register<Service>().Register<Service>();
```
The exception message is:  
```
The type Namespace.Service is already registered.
```

## InvalidRegistrationException
During the service registration, the container executes validation rules on the service types and if the validation fails, it throws an `InvalidRegistrationException`.
It could show the following messages:
- When the implementation does not implement the service type like `Register(typeof(ICar), typeof(MotorCycle))`:  
  ```
  The type Namespace.MotorCycle does not implement the 'service type' Namespace.ICar.
  ```

- The given type is not resolvable like `Register<IService>()`:  
  ```
  The type Namespace.IService could not be resolved. 
  It's probably an interface, abstract class, or primitive type.
  ```

## LifetimeValidationFailedException
This exception pops up when the lifetime validation feature of the container is turned on and the validation fails at some point. Depending on which phase of the validation failed, the exception message could be one of the following:
- When the life-span of a dependency is shorter than its parent's:  
  ```
  The life-span of Namespace.Service (ScopedLifetime|10) is shorter than 
  its direct or indirect parent's Namespace.Dependency (Singleton|20). 
  This could lead to incidental lifetime promotions with longer life-span, 
  it's recommended to double-check your lifetime configurations.
  ```

- When a `Scoped` service is requested from the root scope:  
  ```
  Resolution of Namespace.Service (ScopedLifetime) from the 'root scope' is not allowed, 
  that would promote the service's lifetime to a singleton.
  ```

## AggregateException
This exception can occur only when the `.Validate()` method of the container is called. This function goes through the whole resolution tree and collects the issues into a collection. If it finds any, it puts the result into an `AggregateException` and throws it.