using System;
using System.Linq;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a lifetime manager.
    /// </summary>
    public abstract class LifetimeBase : ILifetime
    {
        /// <inheritdoc />
        public virtual bool HandlesObjectDisposal => false;

        /// <inheritdoc />
        public virtual Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder,
            ResolutionInfo resolutionInfo, Type resolveType)

        {
            var expr = objectBuilder.GetExpression(resolutionInfo, resolveType);

            if (expr == null)
                return null;

            if (!expr.Type.GetTypeInfo().ImplementedInterfaces.Contains(Constants.DisposableType) || 
                objectBuilder.HandlesObjectDisposal || this.HandlesObjectDisposal)
                return expr;

            var method = Constants.AddDisposalMethod.MakeGenericMethod(expr.Type);
            return Expression.Call(Constants.ScopeExpression, method, expr);
        }

        /// <inheritdoc />
        public abstract ILifetime Create();

        /// <inheritdoc />
        public virtual void CleanUp() { }
    }
}
