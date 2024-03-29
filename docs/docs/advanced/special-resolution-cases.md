import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Special resolution cases

## Unknown type resolution
When this [feature](/docs/configuration/container-configuration#unknown-type-resolution) is enabled, the container will try to resolve unregistered types by registering them using a pre-defined configuration delegate.

<Tabs>
<TabItem value="Default" label="Default">

Without a registration configuration, the container can resolve only non-interface and non-abstract unknown types. In the following example,
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

</TabItem>
<TabItem value="With registration configuration" label="With registration configuration">

With a registration configuration, you can control how an unknown type's individual registration should behave. You can also react to a service resolution request. In the following example, we tell the container that if it finds an unregistered `IDependency` for the first time, that should be mapped to `Dependency` and have a singleton lifetime. Next time, when the container comes across this service, it will use the registration created at the first request.

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

</TabItem>
</Tabs>

## Default value injection
When this [feature](/docs/configuration/container-configuration#default-value-injection) is enabled, the container will resolve unknown primitive dependencies with their default value.
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

:::note
Unknown reference types are resolved to `null` only in properties and fields.
:::

## Optional value injection
Stashbox respects the optional value of each constructor and method argument.

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