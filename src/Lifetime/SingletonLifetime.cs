using Stashbox.BuildUp;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a singleton lifetime manager.
    /// </summary>
    public class SingletonLifetime : LifetimeBase
    {
        private readonly object sync = new object();
        private volatile Expression instanceExpression;

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            if (this.instanceExpression != null) return this.instanceExpression;
            lock (this.sync)
            {
                if (this.instanceExpression != null) return this.instanceExpression;

                var expression = base.GetExpression(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
                if (expression == null)
                    return null;

                var instance = expression.NodeType == ExpressionType.New && ((NewExpression)expression).Arguments.Count == 0
                    ? Activator.CreateInstance(expression.Type)
                    : expression.CompileDelegate(resolutionContext)(containerContext.Container.RootScope);

                return this.instanceExpression = instance.AsConstant();
            }
        }

        /// <inheritdoc />
        public override ILifetime Create() => new SingletonLifetime();
    }
}
