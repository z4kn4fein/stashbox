import CodeDescPanel from '@site/src/components/CodeDescPanel';
import Tabs from '@theme/Tabs'; 
import TabItem from '@theme/TabItem';

# Utilities

## Is registered?

<CodeDescPanel>
<div>

With the `IsRegistered()` function, you can find out whether a service is registered into the container or not.

It returns `true` only when the container has a registration with the given type (and name). It only checks the actual container's registrations. For every cases, you should use the `CanResolve()` method.

</div>
<div>

<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

```cs
bool isIJobRegistered = container.IsRegistered<IJob>();
```

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

```cs
bool isIJobRegistered = container.IsRegistered(typeof(IJob));
```

</TabItem>
<TabItem value="Named" label="Named">

#### **Named**
```cs
bool isIJobRegistered = container.IsRegistered<IJob>("DbBackup");
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Can resolve?

<CodeDescPanel>
<div>

There might be cases when you are more interested in whether a service is resolvable from the container's actual state rather than finding out whether it's registered.

`CanResolve()` returns `true` only when at least one of the following is true:
- The requested type is registered in the current or one of the parent containers.
- The requested type is a closed generic type, and its open generic definition is registered.
- The requested type is a wrapper (`IEnumerable<>`, `Lazy<>`, `Func<>`, `KeyValuePair<,>`, `ReadOnlyKeyValue<,>`, `Metadata<,>`, `ValueTuple<,>`, or `Tuple<,>`), and the underlying type is registered.
- The requested type is not registered, but it's resolvable, and [unknown type resolution](/docs/configuration/container-configuration#unknown-type-resolution) is enabled.

</div>
<div>

<Tabs groupId="generic-runtime-apis">
<TabItem value="Generic API" label="Generic API">

```cs
bool isIJobResolvable = container.CanResolve<IJob>();
```

</TabItem>
<TabItem value="Runtime type API" label="Runtime type API">

```cs
bool isIJobResolvable = container.CanResolve(typeof(IJob));
```

</TabItem>
<TabItem value="Named" label="Named">

```cs
bool isIJobResolvable = container.CanResolve<IJob>("DbBackup");
```

</TabItem>
</Tabs>
</div>
</CodeDescPanel>

## Get all mappings

<CodeDescPanel>
<div>

You can get all registrations in a key-value pair collection (where the key is the [service type](/docs/getting-started/glossary#service-type--implementation-type) and the value is the actual registration) by calling the `.GetRegistrationMappings()` method.

</div>
<div>

```cs
IEnumerable<KeyValuePair<Type, ServiceRegistration>> mappings = 
    container.GetRegistrationMappings();
```

</div>
</CodeDescPanel>

## Registration diagnostics

<CodeDescPanel>
<div>

You can get a much more readable version of the registration mappings by calling the `.GetRegistrationDiagnostics()` method.

`RegistrationDiagnosticsInfo` has an overridden `.ToString()` method that returns the mapping details formatted in a human-readable form.

</div>
<div>

```cs
container.Register<IJob, DbBackup>("DbBackupJob");
container.Register(typeof(IEventHandler<>), typeof(EventHandler<>));

IEnumerable<RegistrationDiagnosticsInfo> diagnostics = 
    container.GetRegistrationDiagnostics();

diagnostics.ForEach(Console.WriteLine);
// output:
// IJob => DbBackup, name: DbBackupJob
// IEventHandler<> => EventHandler<>, name: null
```

</div>
</CodeDescPanel>
