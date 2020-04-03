using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.Configuration
{
    /// <summary>
    /// Represents the predefined configuration rules of the <see cref="StashboxContainer"/>
    /// </summary>
    public static class Rules
    {
        /// <summary>
        /// 
        /// </summary>
        public enum RegistrationBehavior
        {
            /// <summary>
            /// 
            /// </summary>
            SkipDuplicates,

            /// <summary>
            /// 
            /// </summary>
            ThrowExceptionOnAlreadyRegistered,

            /// <summary>
            /// 
            /// </summary>
            ReplaceExisting,

            /// <summary>
            /// 
            /// </summary>
            PreserveDuplications
        }

        /// <summary>
        /// Represents the rules for auto injecting members.
        /// </summary>
        [Flags]
        public enum AutoMemberInjectionRules
        {
            /// <summary>
            /// None will be injected.
            /// </summary>
            None = 0,

            /// <summary>
            /// With this flag the container will perform auto injection on properties which has a public setter.
            /// </summary>
            PropertiesWithPublicSetter = 4,

            /// <summary>
            /// With this flag the container will perform auto injection on properties which has a non public setter as well.
            /// </summary>
            PropertiesWithLimitedAccess = 8,

            /// <summary>
            /// With this flag the container will perform auto injection on private fields too.
            /// </summary>
            PrivateFields = 16
        }

        /// <summary>
        /// Represents the constructor selection rules.
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
    }
}
