using System;
using System.Collections.Generic;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;

namespace Stashbox
{
    /// <summary>
    /// Represents a resolution scope.
    /// </summary>
    public class ResolutionScope : ResolutionScopeBase, IDependencyResolver
    {
        private readonly IActivationContext activationContext;

        /// <summary>
        /// Constructs a resolution scope.
        /// </summary>
        /// <param name="activationContext">The dependency resolver.</param>
        public ResolutionScope(IActivationContext activationContext)
        {
            this.activationContext = activationContext;
        }

        /// <inheritdoc />
        public TKey Resolve<TKey>(string name = null) where TKey : class =>
            this.activationContext.Activate(typeof(TKey), this, name) as TKey;

        /// <inheritdoc />
        public object Resolve(Type typeFrom, string name = null) =>
            this.activationContext.Activate(typeFrom, this, name);

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>() where TKey : class =>
            this.activationContext.Activate(typeof(IEnumerable<TKey>), this) as IEnumerable<TKey>;

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            return (IEnumerable<object>)this.activationContext.Activate(type, this);
        }

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, string name = null, params Type[] parameterTypes) =>
            this.activationContext.ActivateFactory(typeFrom, parameterTypes, this, name);
    }
}
