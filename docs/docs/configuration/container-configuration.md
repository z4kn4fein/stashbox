import CodeDescPanel from '@site/src/components/CodeDescPanel';
import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Container configuration

<CodeDescPanel>
<div>

The container's constructor has an `Action<T>` parameter used to configure its behavior.

The configuration API is fluent, which means you can chain the configuration methods after each other.

</div>
<div>

```cs
var container = new StashboxContainer(options => options
    .WithDisposableTransientTracking()
    .WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters)
    .WithRegistrationBehavior(Rules.RegistrationBehavior.ThrowException));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

**Re-configuration** of the container is also supported by calling its `.Configure()` method.

</div>
<div>

```cs
var container = new StashboxContainer();
container.Configure(options => options.WithDisposableTransientTracking());
```

</div>
</CodeDescPanel>


## Default configuration
These features are set by default:

- [Constructor selection](/docs/configuration/container-configuration#constructor-selection): `Rules.ConstructorSelection.PreferMostParameters`
- [Registration behavior](/docs/configuration/container-configuration#registration-behavior): `Rules.RegistrationBehavior.SkipDuplications`
- [Default lifetime](/docs/configuration/container-configuration#default-lifetime): `Lifetimes.Transient`


## Tracking disposable transients
<CodeDescPanel>
<div>

With this option, you can enable or disable the tracking of disposable transient objects.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithDisposableTransientTracking());
```

</div>
</CodeDescPanel>

## Auto member-injection
With this option, you can enable or disable the auto member-injection without [attributes](/docs/guides/service-resolution#attributes).

<CodeDescPanel>
<div>

### `PropertiesWithPublicSetter`
With this flag, the container will perform auto-injection on properties with public setters.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithAutoMemberInjection(
        Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `PropertiesWithLimitedAccess`
With this flag, the container will perform auto-injection on properties even when they don't have a public setter.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithAutoMemberInjection(
        Rules.AutoMemberInjectionRules.PropertiesWithLimitedAccess));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `PrivateFields`
With this flag, the container will perform auto-injection on private fields too.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithAutoMemberInjection(
        Rules.AutoMemberInjectionRules.PrivateFields));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### Combined rules
You can also combine these flags with bitwise logical operators to get a merged ruleset.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithAutoMemberInjection(Rules.AutoMemberInjectionRules.PrivateFields | 
        Rules.AutoMemberInjectionRules.PropertiesWithPublicSetter));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

#### Member selection filter
You can pass your own member selection logic to control which members should be auto injected.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithAutoMemberInjection(
        filter: member => member.Type != typeof(ILogger)));
```

</div>
</CodeDescPanel>

:::info
Members defined with C# 11's [`required`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/required) keyword are automatically injected by the container. 
:::

## Constructor selection
With this option, you can set the constructor selection rule used to determine which constructor the container should use for instantiation.
<CodeDescPanel>
<div>

### `PreferMostParameters`
It prefers the constructor which has the most extended parameter list.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithConstructorSelectionRule(
        Rules.ConstructorSelection.PreferMostParameters));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `PreferLeastParameters`
It prefers the constructor which has the shortest parameter list.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithConstructorSelectionRule(
        Rules.ConstructorSelection.PreferLeastParameters));
```

</div>
</CodeDescPanel>

## Registration behavior
With this option, you can set the actual behavior used when a new service is registered into the container. These options do not affect named registrations.
<CodeDescPanel>
<div>

### `SkipDuplications`
The container will skip new registrations when the given [implementation type](/docs/getting-started/glossary#service-type--implementation-type) is already registered.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithRegistrationBehavior(
        Rules.RegistrationBehavior.SkipDuplications));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `ThrowException`
The container throws an [exception](/docs/diagnostics/validation#servicealreadyregisteredexception) when the given [implementation type](/docs/getting-started/glossary#service-type--implementation-type) is already registered.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithRegistrationBehavior(
        Rules.RegistrationBehavior.ThrowException));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `ReplaceExisting`
The container will replace the already [registered service](/docs/getting-started/glossary#service-registration--registered-service) with the given one when they have the same [implementation type](/docs/getting-started/glossary#service-type--implementation-type).

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithRegistrationBehavior(
        Rules.RegistrationBehavior.ReplaceExisting));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `PreserveDuplications`
The container will keep registering the new services with the same [implementation type](/docs/getting-started/glossary#service-type--implementation-type).

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithRegistrationBehavior(
        Rules.RegistrationBehavior.PreserveDuplications));
```

</div>
</CodeDescPanel>


## Default lifetime
<CodeDescPanel>
<div>

With this option, you can set the default lifetime used when a service doesn't have a configured one.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithDefaultLifetime(Lifetimes.Scoped));
```

</div>
</CodeDescPanel>


## Lifetime validation
<CodeDescPanel>
<div>

With this option, you can enable or disable the life-span and [root scope](/docs/getting-started/glossary#root-scope) resolution [validation](/docs/diagnostics/validation#lifetime-validation) on the dependency tree.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithLifetimeValidation());
```

</div>
</CodeDescPanel>


## Conventional resolution
<CodeDescPanel>
<div>

With this option, you can enable or disable conventional resolution, which means the container treats the constructor/method parameter or member names as dependency names used by [named resolution](/docs/getting-started/glossary#named-resolution).

</div>
<div>

```cs
new StashboxContainer(options => options
    .TreatParameterAndMemberNameAsDependencyName());
```

</div>
</CodeDescPanel>

## Using named service for un-named requests
<CodeDescPanel>
<div>

With this option, you can enable or disable the selection of named registrations when the resolution request is un-named but with the same type.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithNamedDependencyResolutionForUnNamedRequests());
```

</div>
</CodeDescPanel>

## Named service resolution
<CodeDescPanel>
<div>

### `WithUniversalName`
Sets the universal name that represents a special name which allows named resolution work for any given name.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithUniversalName("Any"));
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithAdditionalDependencyNameAttribute`
Adds an attribute type that is considered a dependency name indicator just like the [`DependencyName` attribute](/docs/guides/service-resolution#attributes).

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithAdditionalDependencyNameAttribute<CustomNameAttribute>());
```

</div>
</CodeDescPanel>

<CodeDescPanel>
<div>

### `WithAdditionalDependencyAttribute`
Adds an attribute type that is considered a dependency indicator just like the [`Dependency` attribute](/docs/guides/service-resolution#attributes).

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithAdditionalDependencyAttribute<CustomDependencyAttribute>());
```

</div>
</CodeDescPanel>

## Default value injection
<CodeDescPanel>
<div>

With this option, you can enable or disable the default value injection.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithDefaultValueInjection());
```

</div>
</CodeDescPanel>


## Unknown type resolution
<CodeDescPanel>
<div>

With this option, you can enable or disable the resolution of unregistered types. You can also use a configurator delegate to configure the registrations the container will create from the unknown types.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithUnknownTypeResolution(config => config.AsImplementedTypes()));
```

</div>
</CodeDescPanel>


## Custom compiler
<CodeDescPanel>
<div>

With this option, you can set an external expression tree compiler. It can be useful on platforms where the IL generator modules are not available; therefore, the expression compiler in Stashbox couldn't work.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithExpressionCompiler(
        Rules.ExpressionCompilers.MicrosoftExpressionCompiler));
```

</div>
</CodeDescPanel>


## Re-build singletons in child containers
<CodeDescPanel>
<div>

With this option, you can enable or disable the re-building of singletons in child containers. It allows the child containers to override singleton dependencies in the parent.

</div>
<div>

```cs
new StashboxContainer(options => options
    .WithReBuildSingletonsInChildContainer());
```

</div>
</CodeDescPanel>


:::note
This feature is not affecting the already built singleton instances in the parent.
:::