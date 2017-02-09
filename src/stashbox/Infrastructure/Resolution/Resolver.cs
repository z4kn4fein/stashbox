using System;
using System.Linq.Expressions;
using Stashbox.Entity;

namespace Stashbox.Infrastructure.Resolution
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
        /// The wrapped type.
        /// </summary>
        public virtual Type WrappedType => this.TypeInfo.Type;

        /// <summary>
        /// Indicates that the resolver can be used for resolve enumerable arguments, if it's set to true the container will call the <see cref="GetEnumerableArgumentExpressions" /> method.
        /// </summary>
        public virtual bool CanUseForEnumerableArgumentResolution => false;

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

        /// <summary>
        /// If <see cref="CanUseForEnumerableArgumentResolution"/> is set to true, this method will be called to collect the enumerable item expressions.
        /// </summary>
        /// <param name="resolutionInfo">The resolution info.</param>
        /// <returns>The enumerable item expressions.</returns>
        public virtual Expression[] GetEnumerableArgumentExpressions(ResolutionInfo resolutionInfo)
        {
            throw new NotImplementedException("The CanUseForEnumerableArgumentResolution property is set to true, but the GetEnumerableArgumentExpressions method is not implemented.");
        }
    }
}
