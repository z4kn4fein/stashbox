using Stashbox.Configuration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox
{
    /// <summary>
    /// Represents a resolution scope.
    /// </summary>
    public interface IResolutionScope : IDependencyResolver
    {
        /// <summary>
        /// The parent scope.
        /// </summary>
        IResolutionScope? ParentScope { get; }

        /// <summary>
        /// The name of the scope, if it's null then it's a regular nameless scope.
        /// </summary>
        object? Name { get; }

        /// <summary>
        /// Adds a service to dispose tracking.
        /// </summary>
        /// <param name="disposable">The <see cref="IDisposable"/> object.</param>
        /// <returns>The <see cref="IDisposable"/> object.</returns>
        object AddDisposableTracking(object disposable);

        /// <summary>
        /// Adds a service to dispose tracking.
        /// </summary>
        /// <param name="disposable">The <see cref="IDisposable"/> object.</param>
        /// <param name="requestContext">The request context.</param>
        /// <returns>The <see cref="IDisposable"/> object.</returns>
        object AddRequestContextAwareDisposableTracking(object disposable, IRequestContext requestContext);

        /// <summary>
        /// Adds a service with a cleanup delegate.
        /// </summary>
        /// <param name="finalizable">The object to cleanup.</param>
        /// <param name="finalizer">The cleanup delegate.</param>
        /// <returns>The object to cleanup.</returns>
        object AddWithFinalizer(object finalizable, Action<object> finalizer);

        /// <summary>
        /// Adds a service with an async initializer delegate.
        /// </summary>
        /// <param name="initializable">The object to initialize.</param>
        /// <param name="initializer">The async initializer delegate.</param>
        /// <returns>The object to initialize.</returns>
        object AddWithAsyncInitializer(object initializable, Func<object, IDependencyResolver, CancellationToken, Task> initializer);

        /// <summary>
        /// Returns an existing scoped object or adds it into the scope if it doesn't exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="factory">The value factory used to create the object if it doesn't exist yet.</param>
        /// <param name="requestContext">The request context.</param>
        /// <param name="serviceType">The type of the service.</param>
        /// <returns>The scoped object.</returns>
        object GetOrAddScopedObject(int key, Func<IResolutionScope, IRequestContext, object> factory,
            IRequestContext requestContext, Type serviceType);

        /// <summary>
        /// Invalidates the delegate cache.
        /// </summary>
        void InvalidateDelegateCache();

        /// <summary>
        /// Gets the names of the already opened scopes.
        /// </summary>
        /// <returns>The scope names.</returns>
        IEnumerable<object> GetActiveScopeNames();

        /// <summary>
        /// Called by every node of the internal graph when the <see cref="ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled"/> is true.
        /// Checks for runtime circular dependencies in the compiled delegates.
        /// </summary>
        /// <param name="key">The key of the dependency.</param>
        /// <param name="type">The type of the dependency.</param>
        [Obsolete("Please use parameterized factory registrations to avoid circular dependencies across delegates. `.Register<Service>(options => options.WithFactory<Dependency>(dependency => new Service(dependency)))`")]
        void CheckRuntimeCircularDependencyBarrier(int key, Type type);

        /// <summary>
        /// Called by every node of the internal graph when the <see cref="ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled"/> is true.
        /// Resets the runtime circular dependency checks state for a node.
        /// </summary>
        /// <param name="key"></param>
        [Obsolete("Please use parameterized factory registrations to avoid circular dependencies across delegates. `.Register<Service>(options => options.WithFactory<Dependency>(dependency => new Service(dependency)))`")]
        void ResetRuntimeCircularDependencyBarrier(int key);
    }
}
