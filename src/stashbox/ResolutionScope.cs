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
        /// <inheritdoc />
        public IActivationContext ActivationContext { get; }

        /// <summary>
        /// Constructs a resolution scope.
        /// </summary>
        /// <param name="activationContext">The dependency resolver.</param>
        public ResolutionScope(IActivationContext activationContext)
        {
            this.ActivationContext = activationContext;
        }

        /// <inheritdoc />
        public TKey Resolve<TKey>(string name = null, bool nullResultAllowed = false) where TKey : class =>
            this.ActivationContext.Activate(typeof(TKey), this, name, nullResultAllowed) as TKey;

        /// <inheritdoc />
        public object Resolve(Type typeFrom, string name = null, bool nullResultAllowed = false) =>
            this.ActivationContext.Activate(typeFrom, this, name, nullResultAllowed);

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>() where TKey : class =>
            this.ActivationContext.Activate(typeof(IEnumerable<TKey>), this) as IEnumerable<TKey>;

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom)
        {
            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            return (IEnumerable<object>)this.ActivationContext.Activate(type, this);
        }

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, string name = null, bool nullResultAllowed = false, params Type[] parameterTypes) =>
            this.ActivationContext.ActivateFactory(typeFrom, parameterTypes, this, name, nullResultAllowed);

        /// <inheritdoc />
        public IDependencyResolver BeginScope() => new ResolutionScope(this.ActivationContext);
    }
}
