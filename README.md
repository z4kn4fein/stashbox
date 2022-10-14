# Stashbox
[![Appveyor Build Status](https://img.shields.io/appveyor/build/pcsajtai/stashbox?logo=appveyor&logoColor=white)](https://ci.appveyor.com/project/pcsajtai/stashbox/branch/master) 
[![GitHub Workflow Status](https://img.shields.io/github/workflow/status/z4kn4fein/stashbox/Build%20on%20Linux%20and%20macOS?logo=GitHub)](https://github.com/z4kn4fein/stashbox/actions/workflows/linux-macOS-CI.yml) 
[![Sonar Tests](https://img.shields.io/sonar/tests/z4kn4fein_stashbox?compact_message&logo=sonarcloud&server=https%3A%2F%2Fsonarcloud.io)](https://sonarcloud.io/project/overview?id=z4kn4fein_stashbox) 
[![Sonar Coverage](https://img.shields.io/sonar/coverage/z4kn4fein_stashbox?logo=SonarCloud&server=https%3A%2F%2Fsonarcloud.io)](https://sonarcloud.io/project/overview?id=z4kn4fein_stashbox) 
[![Sonar Quality Gate](https://img.shields.io/sonar/quality_gate/z4kn4fein_stashbox?logo=sonarcloud&server=https%3A%2F%2Fsonarcloud.io)](https://sonarcloud.io/project/overview?id=z4kn4fein_stashbox) 
[![Sourcelink](https://img.shields.io/badge/sourcelink-enabled-brightgreen.svg)](https://github.com/dotnet/sourcelink)

Stashbox is a lightweight, fast, and portable dependency injection framework for .NET-based solutions. It encourages the building of loosely coupled applications and simplifies the construction of hierarchical object structures. It can be integrated easily with .NET Core, Generic Host, ASP.NET, Xamarin, and many other applications.

- [Documentation](https://z4kn4fein.github.io/stashbox)
- [Release notes](https://z4kn4fein.github.io/stashbox/#/changelog)
- [ASP.NET Core sample](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection/tree/master/sample)

Github (stable) | NuGet (stable) | Fuget (stable) | NuGet (pre-release)
--- | --- | --- | ---
[![Github release](https://img.shields.io/github/release/z4kn4fein/stashbox.svg)](https://github.com/z4kn4fein/stashbox/releases) | [![NuGet Version](https://buildstats.info/nuget/Stashbox)](https://www.nuget.org/packages/Stashbox/) | [![Stashbox on fuget.org](https://www.fuget.org/packages/Stashbox/badge.svg?v=5.5.2)](https://www.fuget.org/packages/Stashbox) | [![Nuget pre-release](https://img.shields.io/nuget/vpre/Stashbox)](https://www.nuget.org/packages/Stashbox/)

## Core Attributes
 - 🚀 Fast, thread-safe, and lock-free operations.
 - ⚡️ Easy-to-use Fluent configuration API.
 - ♻️ Small memory footprint.
 - 🔄 Tracks the dependency tree for cycles. 
 - 🚨 Detects and warns about misconfigurations.
 - 🔥 Gives fast feedback on registration/resolution issues.

## Supported Platforms

 - .NET 4.5 and above
 - .NET Core
 - Mono
 - Universal Windows Platform
 - Xamarin (Android/iOS/Mac)
 - Unity

## Contact & Support
- [![Join the chat at https://gitter.im/z4kn4fein/stashbox](https://img.shields.io/gitter/room/z4kn4fein/stashbox.svg)](https://gitter.im/z4kn4fein/stashbox) [![Slack](https://img.shields.io/badge/chat-on%20slack-orange.svg?style=flat)](https://3vj.short.gy/stashbox-slack)
- Create an [issue](https://github.com/z4kn4fein/stashbox/issues) for bug reports and feature requests.
- Start a [discussion](https://github.com/z4kn4fein/stashbox/discussions) for your questions and ideas.
- Add a ⭐️ to support the project!

## Extensions
- ASP.NET Core
    - [Stashbox.Extensions.DependencyInjection](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection)
    - [Stashbox.Extensions.Hosting](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection)
    - [Stashbox.AspNetCore.Hosting](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection)
    - [Stashbox.AspNetCore.Multitenant](https://github.com/z4kn4fein/stashbox-extensions-dependencyinjection)
- ASP.NET
    - [Stashbox.Web.WebApi](https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-web-webapi)
    - [Stashbox.Web.Mvc](https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-web-mvc)
    - [Stashbox.AspNet.SignalR](https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-signalr)
- OWIN
    - [Stashbox.Owin](https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-owin)
    - [Stashbox.AspNet.WebApi.Owin](https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-webapi-owin)
    - [Stashbox.AspNet.SignalR.Owin](https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-signalr-owin)
- WCF
    - [Stashbox.Extension.Wcf](https://github.com/devworker55/stashbox-extension-wcf)
- Hangfire
    - [Stashbox.Hangfire](https://github.com/z4kn4fein/stashbox-extensions/tree/main/src/stashbox-hangfire)
- Mocking
    - [Stashbox.Mocking](https://github.com/z4kn4fein/stashbox-mocking) (Moq, FakeItEasy, NSubstitute, RhinoMocks)
 
## Benchmarks
 - [Performance](https://github.com/danielpalme/IocPerformance)
 - [Feature](http://featuretests.apphb.com/DependencyInjection.html)
    
<br>

*Powered by [Jetbrains'](https://www.jetbrains.com/?from=Stashbox) [Open Source License](https://www.jetbrains.com/community/opensource/?from=Stashbox)*

[![Jetbrains](https://raw.githubusercontent.com/z4kn4fein/stashbox/master/assets/jetbrains.svg)](https://www.jetbrains.com/?from=Stashbox)
