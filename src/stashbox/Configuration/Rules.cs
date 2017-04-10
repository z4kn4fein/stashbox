using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Configuration
{
    /// <summary>
    /// Represents the predefined configuration rules of the <see cref="StashboxContainer"/>
    /// </summary>
    public static class Rules
    {
        /// <summary>
        /// Represents the rules for auto injecting members.
        /// </summary>
        [Flags]
        public enum AutoMemberInjection
        {
            /// <summary>
            /// With this flag the container will perform auto injection on properties which has a public setter.
            /// </summary>
            PropertiesWithPublicSetter,

            /// <summary>
            /// With this flag the container will perform auto injection on properties which has a non public setter as well.
            /// </summary>
            PropertiesWithLimitedAccess,

            /// <summary>
            /// With this flag the container will perform auto injection on private fields too.
            /// </summary>
            PrivateFields,
        }

        /// <summary>
        /// Represents a constructor selection rules.
        /// </summary>
        public static class ConstructorSelection
        {
            /// <summary>
            /// Prefers the constructor which has the longest parameter list.
            /// </summary>
            public static readonly Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> PreferMostParameters =
                constructors => constructors.OrderByDescending(constructor => constructor.GetParameters().Length);

            /// <summary>
            /// Prefers the constructor which has the shortest parameter list.
            /// </summary>
            public static readonly Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>> PreferLeastParameters =
                constructors => constructors.OrderBy(constructor => constructor.GetParameters().Length);
        }

        /// <summary>
        /// Represents a dependency selection rules.
        /// </summary>
        public static class DependencySelection
        {
            /// <summary>
            /// Prefers the last registered service.
            /// </summary>
            public static readonly Func<IEnumerable<IServiceRegistration>, IServiceRegistration> PreferLastRegistered =
                serviceRegistrations => serviceRegistrations.OrderBy(reg => reg.RegistrationNumber).Last();

            /// <summary>
            /// Prefers the first registered service.
            /// </summary>
            public static readonly Func<IEnumerable<IServiceRegistration>, IServiceRegistration> PreferFirstRegistered =
                serviceRegistrations => serviceRegistrations.OrderByDescending(reg => reg.RegistrationNumber).Last();

            /// <summary>
            /// Doesn't change the dependency order, it'll use the first usable dependency.
            /// </summary>
            public static readonly Func<IEnumerable<IServiceRegistration>, IServiceRegistration> ByPass =
                serviceRegistrations => serviceRegistrations.First();
        }

        /// <summary>
        /// Represents an enumerable order rules.
        /// </summary>
        public static class EnumerableOrder
        {
            /// <summary>
            /// Preserves the registration order of the resolved services.
            /// </summary>
            public static readonly Func<IEnumerable<IServiceRegistration>, IEnumerable<IServiceRegistration>> PreserveOrder =
                serviceRegistrations => serviceRegistrations.OrderBy(c => c.RegistrationNumber);

            /// <summary>
            /// Doesn't change the resolution order of the services.
            /// </summary>
            public static readonly Func<IEnumerable<IServiceRegistration>, IEnumerable<IServiceRegistration>> ByPass =
                serviceRegistrations => serviceRegistrations;
        }
    }
}
