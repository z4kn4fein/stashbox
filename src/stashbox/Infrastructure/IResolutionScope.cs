using System;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a resolution scope.
    /// </summary>
    public interface IResolutionScope : IDisposable
    {
        /// <summary>
        /// True if the scope contains scoped instances, otherwise false.
        /// </summary>
        bool HasScopedInstances { get; }

        /// <summary>
        /// Adds or updates an item in the scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void AddScopedItem(object key, object value);

        /// <summary>
        /// Gets an item from the scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The item or null if it doesn't exists.</returns>
        object GetScopedItemOrDefault(object key);

        /// <summary>
        /// Adds or updates an instance in the scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void AddScopedInstance(Type key, object value);

        /// <summary>
        /// Gets an instance from the scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The item or null if it doesn't exists.</returns>
        object GetScopedInstanceOrDefault(Type key);

        /// <summary>
        /// Adds a service for further disposable tracking.
        /// </summary>
        /// <typeparam name="TDisposable">The type parameter.</typeparam>
        /// <param name="disposable">The <see cref="IDisposable"/> object.</param>
        /// <returns>The <see cref="IDisposable"/> object.</returns>
        TDisposable AddDisposableTracking<TDisposable>(TDisposable disposable)
            where TDisposable : IDisposable;
    }
}
