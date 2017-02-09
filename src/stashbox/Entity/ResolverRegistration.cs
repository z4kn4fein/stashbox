using Stashbox.Infrastructure;
using System;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox.Entity
{
    /// <summary>
    /// Represents a resolver registration.
    /// </summary>
    public class ResolverRegistration
    {
        /// <summary>
        /// Predicate which tells whether the current resolver can be used for resolve the given type.
        /// </summary>
        public Func<IContainerContext, TypeInformation, bool> Predicate { get; set; }

        /// <summary>
        /// The factory of the resolver.
        /// </summary>
        public Func<IContainerContext, TypeInformation, Resolver> ResolverFactory { get; set; }
    }
}
