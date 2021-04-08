## Unknown Type Resolution
When this [feature](configuration/container-configuration?id=unknown-type-resolution) is enabled, the container will try to resolve unregistered types by registering them using a pre-defined configuration delegate.

<!-- tabs:start -->
#### **Default**
Without a registration configuration, the container can resolve only non-interface and non-abstract unknown types. In this case,
the container creates an implicit registration for `Dependency` and injects its instance into `Service`.
```cs
class Dependency { }

class Service 
{
    public Service(Dependency dependency)
    { }     
}

var container = new StashboxContainer(config => config
    .WithUnknownTypeResolution());

container.Register<Service>();

var service = container.Resolve<Service>();
```

#### **With registration configuration**
With a registration configuration, you can control how the individual registrations of the unknown types should behave. You also have the option to react to a service resolution request. In this case, we tell the container that if it finds an unregistered `IDependency` service for the first time, it should be mapped to the `Dependency` implementation and have a singleton lifetime. Next time, when the container is coming across with this service, it will use the registration created at the first request.
```cs
interface IDependency { }

class Dependency : IDependency { }

class Service 
{
    public Service(IDependency dependency)
    { }     
}

var container = new StashboxContainer(config => config
    .WithUnknownTypeResolution(options => 
    {
        if(options.ServiceType == typeof(IDependency))
        {
            options.SetImplementationType(typeof(Dependency))
                .WithLifetime(Lifetimes.Singleton);
        }
    }));

container.Register<Service>();

var service = container.Resolve<Service>();
```
<!-- tabs:end -->

## Default Value Injection
When this [feature](configuration/container-configuration?id=default-value-injection) is enabled, the container will resolve unknown primitive dependencies with their default value.
```cs
class Person 
{
    public Person(string name, int age) { }
}

var container = new StashboxContainer(config => config
    .WithDefaultValueInjection());
// the name parameter will be null and the age will be 0.
var person = container.Resolve<Person>();
```

?> Unknown reference types will be resolved to `null` only in properties and fields.

## Optional Value Injection
Stashbox respects the optional value of constructor and method arguments.

```cs
class Person 
{
    public Person(string name = null, int age = 54, IContact contact = null) { }
}

// the name will be null 
// the age will be 54.
// the contact will be null.
var person = container.Resolve<Person>();
```