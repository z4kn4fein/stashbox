# stashbox [![Appveyor build status](https://img.shields.io/appveyor/ci/pcsajtai/stashbox/master.svg?label=appveyor)](https://ci.appveyor.com/project/pcsajtai/stashbox/branch/master) [![Travis CI build status](https://img.shields.io/travis/z4kn4fein/stashbox/master.svg?label=travis)](https://travis-ci.org/z4kn4fein/stashbox) [![Tests](https://img.shields.io/appveyor/tests/pcsajtai/stashbox-0vuru/master.svg)](https://ci.appveyor.com/project/pcsajtai/stashbox-0vuru/build/tests) [![Coverage Status](https://img.shields.io/codecov/c/github/z4kn4fein/stashbox.svg)](https://codecov.io/gh/z4kn4fein/stashbox) [![Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=stashbox&metric=alert_status)](https://sonarcloud.io/dashboard?id=stashbox) [![Sourcelink](https://img.shields.io/badge/sourcelink-enabled-brightgreen.svg)](https://github.com/dotnet/sourcelink)

Stashbox is a lightweight, portable dependency injection framework for .NET based solutions. [![Join the chat at https://gitter.im/z4kn4fein/stashbox](https://img.shields.io/gitter/room/z4kn4fein/stashbox.svg)](https://gitter.im/z4kn4fein/stashbox?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![Slack](https://img.shields.io/badge/chat-on%20slack-orange.svg?style=flat)](https://pcsajtai-dev-slack-in.herokuapp.com/)

Github (stable) | NuGet (stable) | MyGet (pre-release) | Open Hub 
--- | --- | --- | ---
[![Github release](https://img.shields.io/github/release/z4kn4fein/stashbox.svg)](https://github.com/z4kn4fein/stashbox/releases) | [![NuGet Version](https://buildstats.info/nuget/Stashbox)](https://www.nuget.org/packages/Stashbox/) | [![MyGet package](https://img.shields.io/myget/pcsajtai/v/Stashbox.svg?label=myget)](https://www.myget.org/feed/pcsajtai/package/nuget/Stashbox) | [![OpenHub](https://www.openhub.net/p/stashbox/widgets/project_thin_badge?format=gif)](https://www.openhub.net/p/stashbox)

## Features

 - **[Fluent interface](https://github.com/z4kn4fein/stashbox/wiki/Fluent-registration-api)** - for faster and easier configuration, [attribute based](https://github.com/z4kn4fein/stashbox/wiki/Resolution-by-attributes) and [conventional resolution](https://github.com/z4kn4fein/stashbox/wiki/Conventional-resolution) is also supported.
 - **[Interface/implementation mapping](https://github.com/z4kn4fein/stashbox/wiki/Service-registration)** - [single service](https://github.com/z4kn4fein/stashbox/wiki/Service-registration#standard), [instance registration](https://github.com/z4kn4fein/stashbox/wiki/Service-registration#instance), [remapping](https://github.com/z4kn4fein/stashbox/wiki/Service-registration#remap) and replacing is also supported.
 - **[Named registration](https://github.com/z4kn4fein/stashbox/wiki/Service-registration#named)** - multiple implementations are identifiable with names.
 - **[Assembly registration](https://github.com/z4kn4fein/stashbox/wiki/Assembly-registration)** - service lookup in assemblies and auto determining the interface types is also supported.
 - **[Factory delegate registration](https://github.com/z4kn4fein/stashbox/wiki/Factory-registration)** - factory delegates registration with several parameters is also supported. 
 - **[Open generic registration](https://github.com/z4kn4fein/stashbox/wiki/Generics)** - closed generic types are constructed from open generic definitions with constraint and nested generic definition checking.
 - **[Wiring into the container](https://github.com/z4kn4fein/stashbox/wiki/Service-registration#wireup)** - further operations like member and method injection is executed on existing instances.
 - **[Initializer / finalizer](https://github.com/z4kn4fein/stashbox/wiki/Scopes#cleanup-delegate)** - custom initializer *(called when a service is instantiated by the container)* and finalizer *(called when the container or scope which created the service is being disposed)* actions can be set.
 - **[Multiple service resolution](https://github.com/z4kn4fein/stashbox/wiki/Multi-resolution)** - all implementation of a registered interface can be obtained.
 - **[Unknown type resolution](https://github.com/z4kn4fein/stashbox/wiki/Container-configuration#options-available)** - non-registered services can be resolved or injected.
 - **[Default and optional value injection](https://github.com/z4kn4fein/stashbox/wiki/Container-configuration#options-available)** - primitive types or dependencies with default or optional values can be injected.
 - **[Building up existing instances](https://github.com/z4kn4fein/stashbox/wiki/Service-resolution#buildup)** - member and method injection is executed on existing instances without wiring them into the container.
 - **[Child containers](https://github.com/z4kn4fein/stashbox/wiki/Scopes#child-scopes)** - building up and maintaining the parent/child hierarchy between the containers is also supported.
 - **[Lifetime scopes](https://github.com/z4kn4fein/stashbox/wiki/Scopes#lifetime-scope)** - opening and maintaining lifetime scopes is also supported.
 - **[Lifetime management](https://github.com/z4kn4fein/stashbox/wiki/Lifetimes)** - including `Singleton`, `Scoped`, `NamedScope` and `PerResolutionRequest` lifetime, and custom, user-defined lifetimes are also supported.
 - **[Conditional resolution](https://github.com/z4kn4fein/stashbox/wiki/Conditional-resolution)** - attribute, parent-type and custom, user-defined conditions can be specified.
 - **[IDisposable object tracking](https://github.com/z4kn4fein/stashbox/wiki/Scopes#disposal)** - `IDisposable` objects are being disposed by the container.
 - **[Circular dependency tracking](https://github.com/z4kn4fein/stashbox/wiki/Container-configuration#options-available)** - the container checks the resolution graph for circular dependencies and it throws a specific `CircularDependencyException` if it finds any.
 - **[Generic wrappers](https://github.com/z4kn4fein/stashbox/wiki/Generic-wrappers)**
     - Collections: everything assignable to `IEnumerable<T>` e.g. `T[]`, `ICollection<T>`, `IReadOnlyCollection<T>`, `IList<T>` etc.
     - `Lazy<>`, `Func<>`, `Tuple<>`
     - [Parameter injection over delegate arguments](https://github.com/z4kn4fein/stashbox/wiki/Delegate-resolution) (e.g. `Func<TParam, TService>`, `Func<TParam1, TParam2, TService>`) applied to subdependencies as well.
     - Nested wrappers like `Tuple<TService, IEnumerable<Func<TParam, Lazy<TService1>>>>`.
 - **[Custom resolvers](https://github.com/z4kn4fein/stashbox/wiki/Resolvers)** - the existing activation rutines are extendable with custom resolvers.
 - **[Container extensions](https://github.com/z4kn4fein/stashbox/wiki/Extensions)** - the containers functionality is extendable with custom extensions, e.g. [Auto configuration parser extension](https://github.com/z4kn4fein/stashbox-configuration-extension)
 - **[Custom configuration](https://github.com/z4kn4fein/stashbox/wiki/Container-configuration)** - the behavior of the container can be controlled with custom configuration options.
 - **Graph validation** - the resolution graph can be validated by calling the `Validate()` function of the container.
 - **[Decorator support / Interception](https://github.com/z4kn4fein/stashbox/wiki/Decorators)** - decorator services can be registered and used for interception with [Castle DynamicProxy](http://www.castleproject.org/projects/dynamicproxy).

## Supported platforms

 - .NET 4.0 and above
 - Windows 8/8.1/10
 - Windows Phone Silverlight 8/8.1
 - Windows Phone 8.1
 - Xamarin (Android/iOS/iOS Classic)
 - .NET Standard 1.0
 - .NET Standard 1.3
 - .NET Standard 2.0

## Sample usage
```c#
class Wulfgar : IBarbarian
{
    private readonly IWeapon weapon;
    
    public Wulfgar(IWeapon weapon)
    {
        this.weapon = weapon;
    }
}

var container = new StashboxContainer();

container.Register<IWeapon, AegisFang>();
container.Register<IBarbarian, Wulfgar>();

var wulfgar = container.Resolve<IBarbarian>();
```
## Extensions
- ASP.NET Core
    - [Stashbox.Extensions.Dependencyinjection](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection) package contains:
        - [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/Extensions/tree/master/src/DependencyInjection) `IServiceCollection` adapter.
        - [Microsoft.AspNetCore.Hosting](https://github.com/aspnet/AspNetCore/tree/master/src/Hosting) `IWebHostBuilder` extension.
        - [Microsoft.Extensions.Hosting](https://github.com/aspnet/Extensions) `IHostBuilder` extension.
- ASP.NET
    - [Stashbox.Web.WebApi](https://github.com/z4kn4fein/stashbox-web-webapi)
    - [Stashbox.Web.Mvc](https://github.com/z4kn4fein/stashbox-web-mvc)
    - [Stashbox.AspNet.SingalR](https://github.com/z4kn4fein/stashbox-signalr)
- OWIN
    - [Stashbox.Owin](https://github.com/z4kn4fein/stashbox-owin)
    - [Stashbox.AspNet.WebApi.Owin](https://github.com/z4kn4fein/stashbox-webapi-owin)
    - [Stashbox.AspNet.SingalR.Owin](https://github.com/z4kn4fein/stashbox-signalr-owin)
- WCF
    - [Stashbox.Extension.Wcf](https://github.com/devworker55/stashbox-extension-wcf)
- Hangfire
    - [Stashbox.Hangfire](https://github.com/z4kn4fein/stashbox-hangfire)
- Mocking
    - [Stashbox.Mocking](https://github.com/z4kn4fein/stashbox-mocking) (Moq, FakeItEasy, NSubstitute, RhinoMocks, Rocks)
- Other
    - [Decorator extension](https://github.com/z4kn4fein/stashbox-decoratorextension) (obsolate, the decorator pattern support is a built-in feature from version 2.3.0)
    - [Stashbox.Configuration](https://github.com/z4kn4fein/stashbox-configuration-extension) auto configuration parser

## Documentation
 - [Wiki](https://github.com/z4kn4fein/stashbox/wiki)
 - [ASP.NET Core sample](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection/tree/master/sample)
 
## Benchmarks
 - [Performance](https://github.com/danielpalme/IocPerformance)
 - [Feature](http://featuretests.apphb.com/DependencyInjection.html)

<br/>

*Powered by [Jetbrains'](https://www.jetbrains.com) [Open Source License](https://www.jetbrains.com/community/opensource)*

[![Jetbrains](https://cdn.rawgit.com/z4kn4fein/stashbox/master/img/jetbrains.svg)](https://www.jetbrains.com)
