## Installation

Stashbox and its extensions are distributed via [NuGet](https://www.nuget.org/packages?q=stashbox) packages.

<!-- tabs:start -->

#### **Package Manager**

You can install the package by typing the following into the Package Manager Console:
```
Install-Package Stashbox -Version 3.6.0
```

#### **dotnet CLI**

You can install the package by using the dotnet cli:
```
dotnet add package Stashbox --version 3.6.0
```

#### **PackageReference**

You can add the package into the package references of your `.csproj`:
```
<PackageReference Include="Stashbox" Version="3.6.0" />
```

<!-- tabs:end -->

?> All Stashbox DLLs are signed with a strong name key stored in the [GitHub repository](https://github.com/z4kn4fein/stashbox/).

## Usage
The general idea behind using Stashbox is that you structure your code from loosely coupled components with the [Dependency Inversion Principle](https://en.wikipedia.org/wiki/Dependency_inversion_principle), [Inversion Of Control](https://en.wikipedia.org/wiki/Inversion_of_control) and [Dependency Injection](https://martinfowler.com/articles/injection.html) in mind. 

Rather than letting the services instantiate their dependencies inside themselves, inject the dependencies on construction. Also, rather than creating the object hierarchy manually, you can use a Dependency Injection framework that does the work for you. That's why you are here, I suppose. 🙂

To achieve the most efficient usage of Stashbox, you should follow these steps:
- At the startup of your application, instantiate a `StashboxContainer`.
- Register your services into the container.
- [Validate](diagnostics/validation) the state of the container and the registrations with the `.Validate()` method. *(Optional)*
- During the lifetime of the application, use the container to resolve your services.
- Create [scopes](usage/scopes) and use them to resolve your services. *(Optional)*
- On application exit, call the container's `.Dispose()` method to clean up the resources. *(Optional)*

?> You should create only a single instance from `StashboxContainer` (plus child containers if you use them) per application domain. `StashboxContainer` instances are thread-safe.

!> Don't create new `StashboxContainer` instances continuously. Such action will bypass the container's internal delegate cache and could lead to poor performance. 

## How It Works?
Stashbox builds and maintains a collection of registered services. At first, when a service is requested, Stashbox looks for a service registration that has a matching service type. Then, it scans the found registration's implementation type for all available constructors and selects one with the most arguments it knows how to resolve by matching their types to other registrations.

When every constructor argument has its matching registration, Stashbox jumps to the first argument and does the same scanning on its type. 

This process is repeated until every injectable dependency in the resolution tree has a matching registration. As a final step, Stashbox instantiates them by calling their constructors and builds up the hierarchical object structure. 

## Example
Let's see a quick example. We have three services `DbBackup`, `MessageBus` and `ConsoleLogger`. `DbBackup` has a dependency on `IEventBroadcaster` (implemented by `MessageBus`) and `ILogger` (implemented by `ConsoleLogger`), `MessageBus` also depending on an `ILogger`:
```cs
public interface IJob { void DoTheJob(); }

public interface ILogger { void Log(string message); }

public interface IEventBroadcaster { void Broadcast(IEvent @event); }


public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine(message);
}

public class MessageBus : IEventBroadcaster
{
    private readonly ILogger logger;

    public MessageBus(ILogger logger)
    {
        this.logger = logger;
    }

    void Broadcast(IEvent @event) 
    {
        this.logger.Log($"Sending event to bus: {@event.Name}");
        // Do the actual event broadcast.
    }
}

public class DbBackup : IJob
{
    private readonly ILogger logger;
    private readonly IEventBroadcaster eventBroadcaster;

    public DbBackup(ILogger logger, IEventBroadcaster eventBroadcaster)
    {
        this.logger = logger;
        this.eventBroadcaster = eventBroadcaster;
    }

    public void DoTheJob() 
    {
        this.logger.Log("Backing up!");
        // Do the actual backup.
        this.eventBroadcaster.Broadcast(new DbBackupCompleted());
    } 
}
```

?> By depending only on interfaces, you decouple your services from concrete implementations. This gives you the flexibility of a more comfortable implementation replacement and isolates your components from each other. For example, unit testing benefits a lot from the possibility of replacing a real implementation with mock objects.

The example services above used with Stashbox in a Console Application:

```cs
using Stashbox;
using System;

namespace Example
{
    public class Program
    {
        private static readonly IStashboxContainer container;

        static Program()
        {
            // 1. Create container
            container = new StashboxContainer();

            // 2. Register your services
            container.RegisterSingleton<ILogger, ConsoleLogger>();
            container.Register<IEventBroadcaster, MessageBus>();
            container.Register<IJob, DbBackup>();

            // 3. Validate the configuration.
            container.Validate();
        }

        static void Main(string[] args)
        {
            // 4. Resolve and use your service
            var job = container.Resolve<IJob>();
            job.DoTheJob();
        }
    }
}
```