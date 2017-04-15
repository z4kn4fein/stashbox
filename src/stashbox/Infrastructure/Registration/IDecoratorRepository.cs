using System;
using Stashbox.Utils;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a decorator registration repository.
    /// </summary>
    public interface IDecoratorRepository
    {
        /// <summary>
        /// Adds a decorator to the repository.
        /// </summary>
        /// <param name="type">The decorated type.</param>
        /// <param name="serviceRegistration">The decorator registration.</param>
        /// <param name="replace">True if an existing decorator registration should be replaced.</param>
        void AddDecorator(Type type, IServiceRegistration serviceRegistration, bool replace);

        /// <summary>
        /// Gets a decorator registration.
        /// </summary>
        /// <param name="type">The decorated type.</param>
        /// <returns>The decorator registration if any exists, otherwise null.</returns>
        ConcurrentTree<IServiceRegistration> GetDecoratorsOrDefault(Type type);
    }
}
