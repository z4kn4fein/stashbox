using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a singleton lifetime manager.
    /// </summary>
    public class SingletonLifetime : LifetimeBase
    {
        private volatile Expression expression;
        private object instance;
        private readonly object syncObject = new object();

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;
                var expr = base.GetExpression(containerContext, objectBuilder, resolutionInfo, resolveType);
                if (expr == null)
                    return null;

                if (expr.NodeType == ExpressionType.New && ((NewExpression)expr).Arguments.Count == 0)
                    this.instance = Activator.CreateInstance(expr.Type);
                else
                    this.instance = expr.CompileDelegate(Constants.ScopeExpression)(containerContext.RootScope);

                this.expression = Expression.Constant(this.instance);
            }

            return this.expression;
        }
        /// <inheritdoc />
        public override ILifetime Create() => new SingletonLifetime();
    }
}
