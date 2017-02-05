using System;
using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a dependency resolver.
    /// </summary>
    public abstract class Resolver
    {
        /// <summary>
        /// The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/>
        /// </summary>
        protected readonly IContainerContext BuilderContext;

        /// <summary>
        /// The type information about the resolved service.
        /// </summary>
        protected readonly TypeInformation TypeInfo;

        /// <summary>
        /// Constructs the <see cref="Resolver"/>
        /// </summary>
        /// <param name="containerContext">The <see cref="IContainerContext"/> of the <see cref="StashboxContainer"/></param>
        /// <param name="typeInfo">The type information about the resolved service.</param>
        protected Resolver(IContainerContext containerContext, TypeInformation typeInfo)
        {
            this.BuilderContext = containerContext;
            this.TypeInfo = typeInfo;
        }

        /// <summary>
        /// Produces an expression for creating an instance.
        /// </summary>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <returns>The expression.</returns>
        public abstract Expression GetExpression(ResolutionInfo resolutionInfo);
    }
}
