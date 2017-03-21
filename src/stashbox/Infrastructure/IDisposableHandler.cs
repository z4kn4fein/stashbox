using System;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a disposable handler.
    /// </summary>
    public interface IDisposableHandler : IDisposable
    {
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
