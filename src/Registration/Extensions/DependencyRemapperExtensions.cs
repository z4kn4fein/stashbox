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
        /// Re-maps an existing registration.
        /// </summary>
        /// <param name="reMapper">The re-mapper.</param>
        /// <param name="typeTo">The service/implementation type.</param>
        /// <param name="configurator">The configurator for the registered type.</param>
        /// <returns>The <see cref="IStashboxContainer"/> instance.</returns>
        public static IStashboxContainer ReMap(this IDependencyReMapper reMapper, Type typeTo, Action<RegistrationConfigurator> configurator = null) =>
            reMapper.ReMap(typeTo, typeTo, configurator);
    }
}
