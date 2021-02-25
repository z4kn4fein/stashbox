# Utilities
These are additional utility functions of Stashbox that can be helpful in some cases. 

## Is Registered?
<!-- panels:start -->
<!-- div:left-panel -->
There might be cases where you want to find out whether a service is registered into the container or not.

You can also verify that an implementation is registered with a given name.
<!-- div:right-panel -->
<!-- tabs:start -->
#### **Generic API**
```cs
bool isIJobRegistered = container.IsRegistered<IJob>();
```
#### **Runtime type API**
```cs
bool isIJobRegistered = container.IsRegistered(typeof(IJob));
```
#### **Named**
```cs
bool isIJobRegistered = container.IsRegistered<IJob>("DbBackup");
```
<!-- tabs:end -->
<!-- panels:end -->

## Can Resolve?
<!-- panels:start -->
<!-- div:left-panel -->
There might be cases where rather than finding out that a service is registered, you are more interested in that it's resolvable from the container's actual state or not.

For example, when the [unknown type resolution](configuration/container-configuration?id=unknown-type-resolution) is enabled, `.CanResolve()` will return `true` when the requested type is resolvable but not registered into the container.
<!-- div:right-panel -->
<!-- tabs:start -->
#### **Generic API**
```cs
bool isIJobResolvable = container.CanResolve<IJob>();
```
#### **Runtime type API**
```cs
bool isIJobResolvable = container.CanResolve(typeof(IJob));
```
#### **Named**
```cs
bool isIJobResolvable = container.CanResolve<IJob>("DbBackup");
```
<!-- tabs:end -->
<!-- panels:end -->

## Get All Mappings
<!-- panels:start -->
<!-- div:left-panel -->
You can get all registrations in a key-value pair collection (where the key is the service type and the value is the actual registration) by calling the `.GetRegistrationMappings()` method.
<!-- div:right-panel -->
```cs
IEnumerable<KeyValuePair<Type, ServiceRegistration>> mappings = 
    container.GetRegistrationMappings();
```
<!-- panels:end -->

## Registration Diagnostics
<!-- panels:start -->
<!-- div:left-panel -->
You can get a much more readable version of the registration mappings by calling the `.GetRegistrationDiagnostics()` method.

`RegistrationDiagnosticsInfo` has an overridden `.ToString()` method that returns the mapping details formatted in a human-readable form.
<!-- div:right-panel -->
```cs
container.Register<IJob, DbBackup>("DbBackupJob");
container.Register(typeof(IEventHandler<>), typeof(EventHandler<>));

IEnumerable<RegistrationDiagnosticsInfo> diagnostics = 
    container.GetRegistrationDiagnostics();

diagnostics.ForEach(Console.WriteLine);
// this will be printed:
// IJob => DbBackup, name: DbBackupJob
// IEventHandler<> => EventHandler<>, name:
```
<!-- panels:end -->