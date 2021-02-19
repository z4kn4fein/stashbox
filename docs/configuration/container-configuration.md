# Container configuration API

<!-- panels:start -->
<!-- div:left-panel -->
The container's constructor has an `Action<T>` parameter used to configure its behavior.

The configuration API is fluent, which means you can chain the configuration methods after each other.
<!-- div:right-panel -->
```cs
var container = new StashboxContainer(options => options
  .WithDisposableTransientTracking()
  .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters)
  .WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
The **re-configuration** of the container is also supported by calling its `.Configure()` method.
<!-- div:right-panel -->
```cs
var container = new StashboxContainer();
container.Configure(options => options.WithDisposableTransientTracking());
```
<!-- panels:end -->
## Tracking disposable transients
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can enable or disable the tracking of disposable transient objects.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options.WithDisposableTransientTracking());
```
<!-- panels:end -->
## Auto member-injection
With this option, you can enable or disable the auto member-injection without [attributes](usage/service-resolution?id=attributes).
<!-- panels:start -->
<!-- div:left-panel -->
### PropertiesWithPublicSetter
With this flag, the container will perform auto-injection on properties with public setters.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### PropertiesWithLimitedAccess
With this flag, the container will perform auto-injection on properties even when they don't have a public setter.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### PrivateFields
With this flag, the container will perform auto-injection on private fields too.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### Combined rules
You can also combine these rules also.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields | 
    Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter));
```
<!-- panels:end -->

?> Member selection filter: `config.WithAutoMemberInjection(filter: member => member.Type != typeof(IJob))`

## Constructor selection
With this option, you can set the constructor selection rule used to determine which constructor should the container use for instantiation.
<!-- panels:start -->
<!-- div:left-panel -->
### PreferMostParameters
Prefers the constructor which has the most extended parameter list.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferMostParameters));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### PreferLeastParameters
Prefers the constructor which has the shortest parameter list.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters));
```
<!-- panels:end -->
## Registration behavior
With this option, you can set the actual behavior used when a new service is registered into the container. These options do not affect named registrations.
<!-- panels:start -->
<!-- div:left-panel -->
### SkipDuplications
The container will skip new registrations when the given implementation type is already registered.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithRegistrationBehavior(Rules.RegistrationBehavior.SkipDuplications));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### ThrowException
The container throws an [exception](diagnostics/exceptions?id=servicealreadyregisteredexception) when the given implementation type is already registered.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### ReplaceExisting
The container will replace the already registered service with the given one when they have the same implementation type.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithRegistrationBehavior(Rules.RegistrationBehavior.ReplaceExisting));
```
<!-- panels:end -->
<!-- panels:start -->
<!-- div:left-panel -->
### PreserveDuplications
The container will keep registering the new services with the same implementation type.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithRegistrationBehavior(Rules.RegistrationBehavior.PreserveDuplications));
```
<!-- panels:end -->

## Default lifetime
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can set the default lifetime used when a service doesn't have a configured one.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options.WithDefaultLifetime(Lifetimes.Scoped));
```
<!-- panels:end -->

## Lifetime validation
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can enable or disable the life-span and root scope resolution [validation](diagnostics/validation?id=lifetime-validation) on the dependency tree.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options.WithLifetimeValidation());
```
<!-- panels:end -->

## Conventional resolution
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can enable or disable conventional resolution, which means the container treats the constructor/method parameter or member names as dependency names used by named resolution.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .TreatParameterAndMemberNameAsDependencyName());
```
<!-- panels:end -->
## Using named service for un-named requests
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can enable or disable the selection of named registrations when the resolution request is un-named but with the same type.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithNamedDependencyResolutionForUnNamedRequests());
```
<!-- panels:end -->

## Circular dependencies in delegates
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can enable or disable the runtime circular dependency tracking.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options.WithRuntimeCircularDependencyTracking());
```
<!-- panels:end -->

!> By default, the container checks for circular dependencies when it builds the expression graph, but this could not prevent stack overflows when factory delegates passed by the user are containing circular dependencies. If you turn this feature on, the container will generate nodes into the expression tree that tracks the entering and exiting resolution calls across user-defined factory delegates.

## Circular dependencies with Lazy
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can enable or disable circular dependencies through `Lazy<>` objects.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options.WithCircularDependencyWithLazy());
```
<!-- panels:end -->

## Default value injection
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can enable or disable the default value injection.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options.WithDefaultValueInjection());
```
<!-- panels:end -->

## Unknown type resolution
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can enable or disable the resolution of unregistered types. You can also use a configurator delegate to configure the registrations the container will create from the unknown types.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithUnknownTypeResolution(config => config.AsImplementedTypes()));
```
<!-- panels:end -->

## Custom compiler
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can set an external expression tree compiler. It can be useful on platforms where the IL generator modules are not available; therefore, the expression compiler in Stashbox couldn't work.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithExpressionCompiler(Rules.ExpressionCompilers.MicrosoftExpressionCompiler));
```
<!-- panels:end -->

## Re-build singletons in child containers
<!-- panels:start -->
<!-- div:left-panel -->
With this option, you can enable or disable the re-building of singletons in child containers. It allows the child containers to effectively override singleton dependencies in the parent. This feature is not affecting the already built singleton instances in the parent.
<!-- div:right-panel -->
```cs
new StashboxContainer(options => options
  .WithReBuildSingletonsInChildContainer());
```
<!-- panels:end -->