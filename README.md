# stashbox [![Build status](https://ci.appveyor.com/api/projects/status/0849ee6awjyxohei/branch/master?svg=true)](https://ci.appveyor.com/project/pcsajtai/stashbox/branch/master) [![Coverage Status](https://coveralls.io/repos/z4kn4fein/stashbox/badge.svg?branch=master&service=github)](https://coveralls.io/github/z4kn4fein/stashbox?branch=master) [![Join the chat at https://gitter.im/z4kn4fein/stashbox](https://img.shields.io/badge/gitter-join%20chat-green.svg)](https://gitter.im/z4kn4fein/stashbox?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![NuGet Version](http://img.shields.io/nuget/v/Stashbox.svg?style=flat)](https://www.nuget.org/packages/Stashbox/) [![NuGet Downloads](http://img.shields.io/nuget/dt/Stashbox.svg?style=flat)](https://www.nuget.org/packages/Stashbox/)
Stashbox is a lightweight, portable dependency injection framework for .NET based solutions.

**Features**:

 - Fluent interface and attributes for easier configuration
 - Supports member injection and invocation of injection methods
 - Child container creation
 - Lifetime scopes
 - Conditional resolution
 - IDisposable objects created by Stashbox will be disposed by the container automatically
 - Supports the resolution of IEnumerable, arrays
 - Supports the resolution of `Lazy<T>`
 - Supports open generic types
 - Resolution by custom factory delegates
 - Supports remapping
 - Custom resolvers can be added
 - Can be extended by container extensions
 - Dependency overrides

**Supported platforms**:

 - .NET 4.5 and above
 - Windows 8/8.1/10
 - Windows Phone Silverlight 8/8.1
 - Windows Phone 8.1
 - Xamarin (Android/iOS/iOS Classic)
 
**Documentation**
* [Wiki](https://github.com/z4kn4fein/stashbox/wiki)