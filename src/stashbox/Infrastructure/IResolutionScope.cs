using System;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a resolution scope.
    /// </summary>
    public interface IResolutionScope : IDisposableHandler
    {
        /// <summary>
        /// Adds or updates an item in the scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void AddOrUpdateScopedItem(object key, object value);

        /// <summary>
        /// Gets an item from the scope.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The item or null if it doesn't exists.</returns>
        object GetScopedItemOrDefault(object key);
    }
}
