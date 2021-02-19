## [v3.5.1] - 2021-02-19
- Bugfix: When a singleton registration had been replaced with `.ReplaceExisting()`, the container still used the old instance. #98

## [v3.5.0] - 2021-01-31
- Assembly scanning:
   - Added option to filter service types and disable self-registration.
   - Recognize generic definitions.
- Added support to covariant/contravariant generic type resolution.
- Bugfix: Services with named scope lifetime were not choose right from the registration repo.

## [v3.4.0] - 2020-11-15
- Added the core components of multitenant functionality.
- Throw `ObjectDisposedException` when the container or scope is used after their disposal.

## [v3.3.0] - 2020-11-05
- Added the option to rebuild singletons in child container with dependencies overridden in it.
- Fix: Singleton instances were built when the Validate() was called, now just the expression is generated for them.

## [v3.2.9] - 2020-11-02
- Added the option to replace a registration only if an existing one is registered with the same type or name.

## [v3.2.8] - 2020-10-17
- Switch to license expression in nuget package.

## [v3.2.7] - 2020-10-16
- Minor bugfixes.

## [v3.2.6] - 2020-10-16
- The Validate() method now throws an AggregateException containing all the underlying exceptions.
- Minor bugfixes.

## [v3.2.5] - 2020-10-12
- Minor bugfixes.

## [v3.2.4] - 2020-07-22
- Added the `.WhenDecoratedServiceHas()` and `.WhenDecoratedServiceIs()` decorator configuration options.

## [v3.2.2] - 2020-07-21
- Added support of conditional and lifetime managed decorators #93      

## [v3.2.1] - 2020-07-09
- Fix: Factory resolution didn't use the built-in expression compiler.

## [v3.2.0] - 2020-06-29
- Added IAsyncDisposable support #90
 - It works on >=net461, >=netstandard2.0 frameworks.
 - On net461 and netstandard2.0 the usage of IAsyncDisposable interface requires the
   Microsoft.Bcl.AsyncInterfaces package, on netstandard2.1 it's part of the framework.
- Fix: resolving with custom parameter values #91

## [v3.1.2] - 2020-06-22
- Fix: IdentityServer not compatible #88
- Fix: Call interception #89

## [v3.1.1] - 2020-06-11
- Fix: String constant is not handled well by the built-in compiler #86
- Fix: Registration behaviour doesn't respect replacing #87

## [v3.1.0] - 2020-06-08
- Fix: Nested named resolution could cause stack overflow #74
- Fix: Improve support for Assemblies loaded into Collectible AssemblyLoadContexts #73
- Fix: Unknown type resolution does not work recursively #77
- Fix: Exception when building expressions #76
- Fix: Bad performance #79
- Fix: Expected override behaviour not working with scopes #80

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
