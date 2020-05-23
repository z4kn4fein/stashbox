using System;
using System.Collections.Generic;

namespace Stashbox.Registration
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
        /// <param name="remap">If true, all the registrations mapped to a service type will be replaced.</param>
        /// <param name="replace">True if an existing decorator registration should be replaced.</param>
        void AddDecorator(Type type, IServiceRegistration serviceRegistration, bool remap, bool replace);

        /// <summary>
        /// Gets a decorator registration.
        /// </summary>
        /// <param name="type">The decorated type.</param>
        /// <returns>The decorator registrations if any exists, otherwise null.</returns>
        IEnumerable<IServiceRegistration> GetDecoratorsOrDefault(Type type);
    }
}
