# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [v4.1.0] - 2021-11-21
### Fixed
- `IsRegistered()` returns `true` only when the container has a registration with the given type (and name).
- `CanResolve()` returns `true` only when any of the following is true:
   - The given type is registered in the current or one of the parent containers.
   - The given type is a closed generic type and its open generic definition is registered.
   - The given type is wrapped by `IEnumerable<>`, `Lazy<>`, `Func<>`, or `Tuple<>`.
   - The given type is not registered but it’s resolvable and the [unknown type resolution](https://z4kn4fein.github.io/stashbox/#/advanced/special-resolution-cases?id=unknown-type-resolution) is enabled.

## [v4.0.0] - 2021-11-18
### Removed
- .NET 4.0 support.

## [v3.6.4] - 2021-08-31
### Added
- `Skip()` method for `UnknownRegistrationConfigurator` used to prevent specific types from auto injection. [#105](https://github.com/z4kn4fein/stashbox/issues/105)
- Parameterized `WithFactory()` option for runtime type based registrations and decorators. [#105](https://github.com/z4kn4fein/stashbox/issues/105)

## [v3.6.3] - 2021-05-26
### Fixed
- Resolving Func uses the wrong constructor. [#102](https://github.com/z4kn4fein/stashbox/issues/102)
- Base class InjectionMethod not populated. [#103](https://github.com/z4kn4fein/stashbox/issues/103)

## [v3.6.2] - 2021-04-23
### Fixed
- Rare NullReferenceException on Resolve. [#101](https://github.com/z4kn4fein/stashbox/issues/101)
- Decorators having `IEnumerable<TDecoratee>` dependency were not handled correctly.


## [v3.6.1] - 2021-03-16
### Fixed
- **Lifetime validation for scoped services requested from root scope.**
The validation was executed only at the expression tree building phase, so an already built scoped factory invoked on the root scope was able to bypass the lifetime validation and store the instance as a singleton. Now the validation runs at every request.

## [v3.6.0] - 2021-02-25

[API changes](https://www.fuget.org/packages/Stashbox/3.6.0/lib/netstandard2.1/diff/3.5.1/)

### Added
- Parameterized factory delegates. [Read more](https://z4kn4fein.github.io/stashbox/#/usage/advanced-registration?id=factory-registration). Also, [here](https://z4kn4fein.github.io/stashbox/#/configuration/registration-configuration?id=factory) is the list of the new factory configuration methods.
- Multiple conditions from the same type are now combined with **OR** logical operator. [Read more](https://z4kn4fein.github.io/stashbox/#/usage/service-resolution?id=conditional-resolution).
- Named version of the `.WhenDecoratedServiceIs()` decorator condition. [Read more](https://z4kn4fein.github.io/stashbox/#/advanced/decorators?id=conditional-decoration).

### Deprecated
- `.InjectMember()` registration configuration option. `.WithDependencyBindig()` should be used instead. [Read more](https://z4kn4fein.github.io/stashbox/#/configuration/registration-configuration?id=dependency-configuration).

### Removed
- The `GetRegistrationOrDefault(type, resolutionContext, name)` method of the `IRegistrationRepository` interface.
- Some properties of the `RegistrationContext` class were moved to internal visibility.

## [v3.5.1] - 2021-02-19
### Fixed
- When a singleton registration was replaced with `.ReplaceExisting()`, the container still used the old instance. [#98](https://github.com/z4kn4fein/stashbox/issues/98)

## [v3.5.0] - 2021-01-31
### Added
- Assembly scanning:
   - Added option to filter service types and disable self-registration.
   - Recognize generic definitions.
- Support to covariant/contravariant generic type resolution.

### Fixed
- Services with named scope lifetime were not chosen right from the registration repo.

## [v3.4.0] - 2020-11-15
### Added
- The core components of multitenant functionality.
- Throw `ObjectDisposedException` when the container or scope is used after their disposal.

## [v3.3.0] - 2020-11-05
### Added
- Option to rebuild singletons in child container with dependencies overridden in it.

### Fixed
- Singleton instances were built when the Validate() was called, now just the expression is generated for them.

## [v3.2.9] - 2020-11-02
### Added
- Option to replace a registration only if an existing one is registered with the same type or name.

## [v3.2.8] - 2020-10-17
### Changed
- Switch to license expression in nuget package. [#95](https://github.com/z4kn4fein/stashbox/issues/95)

## [v3.2.7] - 2020-10-16
### Changed
- Minor bugfixes.

## [v3.2.6] - 2020-10-16
### Added
- The Validate() method now throws an AggregateException containing all the underlying exceptions.

### Changed
- Minor bugfixes.

## [v3.2.5] - 2020-10-12

### Changed
- Minor bugfixes.

## [v3.2.4] - 2020-07-22
### Added
- The `.WhenDecoratedServiceHas()` and `.WhenDecoratedServiceIs()` decorator configuration options.

## [v3.2.2] - 2020-07-21
### Added
- Support of conditional and lifetime managed decorators [#93](https://github.com/z4kn4fein/stashbox/issues/93)      

## [v3.2.1] - 2020-07-09
### Fixed
- Factory resolution didn't use the built-in expression compiler.

## [v3.2.0] - 2020-06-29
### Added
- IAsyncDisposable support [#90](https://github.com/z4kn4fein/stashbox/issues/90)
 - It works on >=net461, >=netstandard2.0 frameworks.
 - On net461 and netstandard2.0 the usage of IAsyncDisposable interface requires the
   Microsoft.Bcl.AsyncInterfaces package, on netstandard2.1 it's part of the framework.

### Fixed
- Resolution with custom parameter values [#91](https://github.com/z4kn4fein/stashbox/issues/91)

## [v3.1.2] - 2020-06-22
### Fixed
- IdentityServer not compatible [#88](https://github.com/z4kn4fein/stashbox/issues/88)
- Call interception [#89](https://github.com/z4kn4fein/stashbox/issues/89)

## [v3.1.1] - 2020-06-11
### Fixed
- String constant is not handled well by the built-in compiler [#86](https://github.com/z4kn4fein/stashbox/issues/86)
- Registration behaviour doesn't respect replacing [#87](https://github.com/z4kn4fein/stashbox/issues/87)

## [v3.1.0] - 2020-06-08
### Fixed
- Nested named resolution could cause stack overflow [#74](https://github.com/z4kn4fein/stashbox/issues/74)
- Improve support for Assemblies loaded into Collectible AssemblyLoadContexts [#73](https://github.com/z4kn4fein/stashbox/issues/73)
- Unknown type resolution does not work recursively [#77](https://github.com/z4kn4fein/stashbox/issues/77)
- Exception when building expressions [#76](https://github.com/z4kn4fein/stashbox/issues/76)
- Bad performance [#79](https://github.com/z4kn4fein/stashbox/issues/79)
- Expected override behaviour not working with scopes [#80](https://github.com/z4kn4fein/stashbox/issues/80)

### Breaking changes:
- `WithUniqueRegistrationIdentifiers()` option has been removed, `WithRegistrationBehavior()` has been added instead.
- Circular dependency tracking is enabled now by default, for runtime tracking the renamed `WithRuntimeCircularDependencyTracking()` option can be used.
- `WithMemberInjectionWithoutAnnotation()` container configuration option has been renamed to `WithAutoMemberInjection()`.
- `SetImplementationType()` option has been added to the registration configuration used when unknown type detected.
- Removed the `GetScopedInstace()` method from the `IResolutionScope`, they are treated as expression overrides now and consumed automatically by the container.
- Lifetimes became stateless and their API has been changed, read [this](https://z4kn4fein.github.io/stashbox/#/usage/lifetimes) for more info.
- Lifetime validation has been added:
 - Tracking dependencies that has shorter life-span than their direct or indirect parent's.
 - Tracking scoped services resolved from root.
 - The container throws a LifetimeValidationFailedException when the validation fails.
- `PerRequestLifetime` has been renamed to `PerScopedRequestLifetime`.
- `RegisterInstanceAs()` has been removed, every functionality is available on the `RegisterInstance()` methods.
- Service/Implementation type map validation has been added to the non-generic registration methods.
- `InjectionParameter` has been replaced with `KeyValuePair<string, object>`.
- `IserviceRegistration` interface has been removed, only its implementation remained.
- Removed the legacy container extension functionality.
- Removed the support of PCL v259.

[v4.1.0]: https://github.com/z4kn4fein/stashbox/compare/4.0.0...4.1.0
[v4.0.0]: https://github.com/z4kn4fein/stashbox/compare/3.6.4...4.0.0
[v3.6.4]: https://github.com/z4kn4fein/stashbox/compare/3.6.3...3.6.4
[v3.6.3]: https://github.com/z4kn4fein/stashbox/compare/3.6.2...3.6.3
[v3.6.2]: https://github.com/z4kn4fein/stashbox/compare/3.6.1...3.6.2
[v3.6.1]: https://github.com/z4kn4fein/stashbox/compare/3.6.0...3.6.1
[v3.6.0]: https://github.com/z4kn4fein/stashbox/compare/3.5.1...3.6.0
[v3.5.1]: https://github.com/z4kn4fein/stashbox/compare/3.5.0...3.5.1
[v3.5.0]: https://github.com/z4kn4fein/stashbox/compare/3.4.0...3.5.0
[v3.4.0]: https://github.com/z4kn4fein/stashbox/compare/3.3.0...3.4.0
[v3.3.0]: https://github.com/z4kn4fein/stashbox/compare/3.2.9...3.3.0
[v3.2.9]: https://github.com/z4kn4fein/stashbox/compare/3.2.8...3.2.9
[v3.2.8]: https://github.com/z4kn4fein/stashbox/compare/3.2.7...3.2.8
[v3.2.7]: https://github.com/z4kn4fein/stashbox/compare/3.2.6...3.2.7
[v3.2.6]: https://github.com/z4kn4fein/stashbox/compare/3.2.5...3.2.6
[v3.2.5]: https://github.com/z4kn4fein/stashbox/compare/3.2.4...3.2.5
[v3.2.4]: https://github.com/z4kn4fein/stashbox/compare/3.2.3...3.2.4
[v3.2.3]: https://github.com/z4kn4fein/stashbox/compare/3.2.2...3.2.3
[v3.2.2]: https://github.com/z4kn4fein/stashbox/compare/3.2.1...3.2.2
[v3.2.1]: https://github.com/z4kn4fein/stashbox/compare/3.2.0...3.2.1
[v3.2.0]: https://github.com/z4kn4fein/stashbox/compare/3.1.2...3.2.0
[v3.1.2]: https://github.com/z4kn4fein/stashbox/compare/3.1.1...3.1.2
[v3.1.1]: https://github.com/z4kn4fein/stashbox/compare/3.1.0...3.1.1
[v3.1.0]: https://github.com/z4kn4fein/stashbox/compare/2.8.9...3.1.0
