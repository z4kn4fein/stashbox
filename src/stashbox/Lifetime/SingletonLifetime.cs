using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
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
        private readonly object syncObject = new object();

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;
                var expr = base.GetExpression(containerContext, serviceRegistration, objectBuilder, resolutionInfo, resolveType);
                if (expr == null)
                    return null;

                object instance;
                if (expr.NodeType == ExpressionType.New && ((NewExpression)expr).Arguments.Count == 0)
                    instance = Activator.CreateInstance(expr.Type);
                else
                    instance = expr.CompileDelegate(Constants.ScopeExpression)(resolutionInfo.RootScope);

                this.expression = Expression.Constant(instance);
            }

            return this.expression;
        }
        /// <inheritdoc />
        public override ILifetime Create() => new SingletonLifetime();
    }
}
