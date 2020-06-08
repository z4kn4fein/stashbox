using Stashbox.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Configuration
{
    /// <summary>
    /// Represents the predefined configuration rules of the <see cref="StashboxContainer"/>.
    /// </summary>
    public static class Rules
    {
        /// <summary>
        /// Represents the actual behavior used when a new service is going to be registered into the container. These options does not affect named registrations.
        /// </summary>
        public enum RegistrationBehavior
        {
            /// <summary>
            /// The container will skip new registrations when the given implementation type is already registered.
            /// </summary>
            SkipDuplications,

            /// <summary>
            /// The container will throw a <see cref="ServiceAlreadyRegisteredException"/> when the given implementation type is already registered.
            /// </summary>
            ThrowException,

            /// <summary>
            /// The container will replace the already registered service with the given one when they have the same implementation type.
            /// </summary>
            ReplaceExisting,

            /// <summary>
            /// The container will keep registering the new services with the same implementation type.
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

        /// <summary>
        /// Pre-defined expression compiler delegates.
        /// </summary>
        public static class ExpressionCompilers
        {
            /// <summary>
            /// The standard Microsoft expression compiler.
            /// </summary>
            public static readonly Func<LambdaExpression, Delegate>
                MicrosoftExpressionCompiler = lambda => lambda.Compile();
        }
    }
}
