using System;
using System.Collections.Generic;
using System.Linq;
using Stashbox.Entity;

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
            public static readonly Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> PreferMostParameters =
                constructors => constructors.OrderByDescending(constructor => constructor.Parameters.Length);

            /// <summary>
            /// Prefers the constructor which has the shortest parameter list.
            /// </summary>
            public static readonly Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> PreferLeastParameters =
                constructors => constructors.OrderBy(constructor => constructor.Parameters.Length);
        }
    }
}
