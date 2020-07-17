using Stashbox.Registration.Fluent;
using System;

namespace Stashbox
{
    /// <summary>
    /// Represents the extension methods of <see cref="IDependencyReMapper"/>.
    /// </summary>
    public static class DependencyReMapperExtensions
    {
        /// <summary>
        /// Replaces an existing registration mapping.
        /// </summary>
        /// <param name="reMapper">The re-mapper.</param>
        /// <param name="typeTo">Type that will be returned.</param>
        /// <param name="configurator">The configurator for the registered types.</param>
        /// <returns>The <see cref="IStashboxContainer"/> which on this method was called.</returns>
        public static IStashboxContainer ReMap(this IDependencyReMapper reMapper, Type typeTo, Action<RegistrationConfigurator> configurator = null) =>
            reMapper.ReMap(typeTo, typeTo, configurator);
    }
}
