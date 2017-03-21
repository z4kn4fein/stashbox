using System;
using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a scoped lifetime.
    /// </summary>
    public class ScopedLifetime : LifetimeBase
    {
        private volatile Expression expression;
        private readonly object syncObject = new object();
        private readonly string id;

        /// <summary>
        /// Constructs a <see cref="ScopedLifetime"/>.
        /// </summary>
        public ScopedLifetime()
        {
            this.id = Guid.NewGuid().ToString();
        }

        /// <inheritdoc />
        public override ILifetime Create() => new ScopedLifetime();

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo,
            Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;
                var expr = base.GetExpression(containerContext, objectBuilder, resolutionInfo, resolveType);
                if (expr == null)
                    return null;

                var factory = expr.CompileDelegate(Constants.ScopeExpression);

                var method = Constants.GetScopedValueMethod.MakeGenericMethod(resolveType);

                this.expression = Expression.Call(method,
                    Constants.ScopeExpression,
                    Expression.Constant(factory), Expression.Constant(this.id));
            }

            return this.expression;
        }

        /// <inheritdoc />
        public override bool HandlesObjectDisposal => true;

        private static TValue GetScopedValue<TValue>(IResolutionScope scope, Func<IResolutionScope, object> factory, string lifeTimeId)
        {
            var value = scope.GetScopedItemOrDefault(lifeTimeId);
            if(value == null)
            {
                value = factory(scope);
                scope.AddOrUpdateScopedItem(lifeTimeId, value);

                if (value is IDisposable disposable)
                    scope.AddDisposableTracking(disposable);
            }

            return (TValue)value;
        }
    }
}
