# Container validation

Stashbox has validation routines that help you detect and solve common misconfiguration issues. You can verify the container's actual state with its `.Validate()` method. This method walks through the whole resolution tree and collects all the issues into an [AggregateException](diagnostics/exceptions?id=aggregateexception).

## Registration validation
The container executes validation against the given types during registration.  
The validation fails when:
- The implementation type is not resolvable (it's an interface or an abstract class).
- The implementation type does not implement the service type.
- The given implementation type is already registered and the `RegistrationBehavior` [container configuration option](configuration/container-configuration?id=registration-behavior) is set to `ThrowException`. 

When any of the above occurs, the container throws a specific [exception](diagnostics/exceptions?id=invalidregistrationexception).

## Resolution validation
During the construction of the resolution tree, the container constantly checks its actual state to ensure its stability.

### Unresolvable type
When a requested type is not instantiable (e.g., it doesn't have a public constructor), the container throws an [exception](diagnostics/exceptions?id=resolutionfailedexception) with every diagnostic detail included.

### Missing dependency 
When a type the requested service is depending on is not resolvable (either as a constructor parameter or a class member), the container throws an [exception](diagnostics/exceptions?id=resolutionfailedexception) with every diagnostic detail included, like which constructors were tried for resolution and which parameters were unable to resolve.

### Lifetime validation
This validation enforces the following rules, and when they are being violated, the container throws an [exception](diagnostics/exceptions?id=lifetimevalidationfailedexception).
  1. **When a scoped service is requested from the root scope**. As the root scope's lifetime is bound to the container's lifetime, this action would unintentionally promote the scoped service's lifetime to a singleton.
  
  2. **When the life-span of a dependency is shorter than its parent's**. It's called [captive dependency](https://blog.ploeh.dk/2014/06/02/captive-dependency/). Every lifetime has a `LifeSpan` value, which determines how long the related service lives. The main rule is that services may not contain dependencies with shorter life-spans like singletons should not depend on scoped services. The only exception is the life-span value `0`, which indicates that the related service is state-less and could be injected into any service.  
    These are the current `LifeSpan` values: 
     - **Singleton**: 20
     - **Scoped**: 10
     - **NamedScope**: 10
     - **PerScopedRequest**: 0
     - **Transient**: 0

### Circular dependency
When the container notices an infinite dependency loop in the resolution tree, it throws an [exception](diagnostics/exceptions?id=circulardependencyexception) with every diagnostic detail included.