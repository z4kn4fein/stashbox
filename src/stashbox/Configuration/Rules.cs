using System;
using System.Collections.Generic;
using System.Linq;
using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.Configuration
{
    /// <summary>
    /// Represents the predefined configuration rules of the <see cref="StashboxContainer"/>
    /// </summary>
    public static class Rules
    {
        /// <summary>
        /// Represents a constructor selection rules.
        /// </summary>
        public static class ConstructorSelection
        {
            /// <summary>
            /// Prefers the constructor which has the longest parameter list.
            /// </summary>
            public static Func<IEnumerable<ConstructorInformation>, ConstructorInformation> PreferMostParameters =
                constructors => constructors.OrderByDescending(constructor => constructor.Parameters.Length).First();

            /// <summary>
            /// Prefers the constructor which has the shortest parameter list.
            /// </summary>
            public static Func<IEnumerable<ConstructorInformation>, ConstructorInformation> PreferLessParameters =
                constructors => constructors.OrderBy(constructor => constructor.Parameters.Length).First();
        }

        /// <summary>
        /// Represents a dependency selection rules.
        /// </summary>
        public static class DependencySelection
        {
            /// <summary>
            /// Prefers the last registered service.
            /// </summary>
            public static Func<IEnumerable<IServiceRegistration>, IServiceRegistration> PreferLastRegistered =
                serviceRegistrations => serviceRegistrations.OrderBy(reg => reg.RegistrationNumber).Last();

            /// <summary>
            /// Prefers the first registered service.
            /// </summary>
            public static Func<IEnumerable<IServiceRegistration>, IServiceRegistration> PreferFirstRegistered =
                serviceRegistrations => serviceRegistrations.OrderByDescending(reg => reg.RegistrationNumber).Last();
        }

        /// <summary>
        /// Represents an enumerable order rules.
        /// </summary>
        public static class EnumerableOrder
        {
            /// <summary>
            /// Preserves the registration order of the resolved services.
            /// </summary>
            public static Func<IEnumerable<IServiceRegistration>, IEnumerable<IServiceRegistration>> PreserveOrder =
                serviceRegistrations => serviceRegistrations.OrderBy(c => c.RegistrationNumber);

            /// <summary>
            /// Doesn't change the resolution order of the services.
            /// </summary>
            public static Func<IEnumerable<IServiceRegistration>, IEnumerable<IServiceRegistration>> ByPass =
                serviceRegistrations => serviceRegistrations;
        }
    }
}
