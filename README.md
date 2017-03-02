# stashbox [![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/stashbox/master.svg?label=travis-ci)](https://travis-ci.org/z4kn4fein/stashbox) [![Coverage Status](https://coveralls.io/repos/github/z4kn4fein/stashbox/badge.svg?branch=master)](https://coveralls.io/github/z4kn4fein/stashbox?branch=master) [![NuGet Version](https://buildstats.info/nuget/Stashbox)](https://www.nuget.org/packages/Stashbox/) [![MyGet package](https://img.shields.io/myget/pcsajtai/v/Stashbox.svg?label=myget)](https://www.myget.org/feed/pcsajtai/package/nuget/Stashbox)

Stashbox is a lightweight, portable dependency injection framework for .NET based solutions.

[![Join the chat at https://gitter.im/z4kn4fein/stashbox](https://img.shields.io/badge/gitter-join%20chat-1dce73.svg)](https://gitter.im/z4kn4fein/stashbox?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Slack](https://img.shields.io/badge/slack-chat-orange.svg?style=flat)](https://stashbox-container.slack.com)

## Features

 - **Fluent interface** - for faster and easier configuration, attributes also can be used.
 - **Interface/type mapping** - single service, existing instance registration, remapping registrations also supported.
 - **Resolution via delegates** - any number of parameters can be injected, they will be reused for subdenpendency resolution as well.
 - **Registering with name** - multiple registration with the same service type, can be addressed at resolution time via attributes as well.
 - **Multiple service resolution** - all registered type of a service can be obtained as an `IEnumerable<T>` or `IEnumerable<object>` with the `ResolveAll()` method.
 - **Unknown type resolution** - unregistered services can be resolved or injected.
 - **Default and optional value injection** - primitive types or dependencies with default or optional value can be injected.
 - **Open generic type resolution** - concrete generic types can be resolved from open generic definitions, constraint checking and nested generic definitions also supported.
 - **Constructor, property and field injection** - supports attribute driven injection and attributeless injection as well, where there is no chance to decorate members with attributes.
 - **Injection method** - methods decorated with `InjectionMethod` attribute will be called at resolution time.
 - **Wiring into container** - member injection can be executed on existing instance with every resolve call.
 - **Building up existing instance** - member injection can be executed on existing instance without registering it into the container.
 - **Child scopes** - for managing scoped registrations.
 - **Lifetime management** - including `Singleton`, `Transient` and `Scoped` lifetime, custom user defined lifetimes also can be used.
 - **Conditional resolution** - attribute, parent-type and custom user defined conditions can be specified.
 - **IDisposable object tracking** - `IDisposable` objects are being disposed by the container.
 - **Circular dependency tracking** - the container checks the dependency graph for circular dependencies, specific excpetion will be thrown if found any.
 - **Special types** - generic wrappers:
  - Collections: everything assignable to `IEnumerable<T>` e.g. `T[]`, `ICollection<T>`, `IReadOnlyCollection<T>`, `IList<T>` etc.
  - `Lazy<>`, `Func<>`, `Tuple<>`
  - Parameter injection over factory method arguments e.g. `Func<TParam, TService>`, `Func<TParam1, TParam2, TService>`, etc. applied to subdependencies as well.
  - Nesting support e.g. `Tuple<TService, IEnumerable<Func<TParam, Lazy<TService1>>>>`.
 - **Custom resolvers** - the existing resolution operations can be extended by custom resolvers.
 - **Container extensions** - the functionality of the container can be extended by custom extensions, e.g. [Decorator extension](https://github.com/z4kn4fein/stashbox-decoratorextension)
 - **Custom configuration** - the behavior of the container can be controlled by custom configuration.

## Supported platforms

 - .NET 4.5 and above
 - Windows 8/8.1/10
 - Windows Phone Silverlight 8/8.1
 - Windows Phone 8.1
 - Xamarin (Android/iOS/iOS Classic)
 - .NET Standard 1.0

## Sample usage
```c#
class Wulfgar : IBarbarian
{
    public Wulfgar(IWeapon weapon)
    {
        //...
    }
}

container.RegisterType<IWeapon, AegisFang>();
container.RegisterType<IBarbarian, Wulfgar>();

var wulfgar = container.Resolve<IBarbarian>();
```
## Extensions
 - [Decorator extension](https://github.com/z4kn4fein/stashbox-decoratorextension)
 - [Stashbox.Web.WebApi](https://github.com/z4kn4fein/stashbox-web-webapi)
 - [Stashbox.Web.Mvc](https://github.com/z4kn4fein/stashbox-web-mvc)
 - [Stashbox.Extensions.Dependencyinjection](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection)
  - [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection) adapter for ASP.NET Core.
  - [Microsoft.AspNetCore.Hosting](https://github.com/aspnet/Hosting) `IWebHostBuilder` extension also included.

## Documentation
 - [Wiki](https://github.com/z4kn4fein/stashbox/wiki)
 
## Benchmarks
 - [Performance](http://www.palmmedia.de/blog/2011/8/30/ioc-container-benchmark-performance-comparison)
 - [Feature](http://featuretests.apphb.com/DependencyInjection.html)
