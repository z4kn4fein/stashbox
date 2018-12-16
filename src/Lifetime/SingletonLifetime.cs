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
    public class SingletonLifetime : ScopedLifetimeBase
    {
        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            var expression = base.GetExpression(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
            if (expression == null)
                return null;

            var factory = expression.NodeType == ExpressionType.New && ((NewExpression)expression).Arguments.Count > 0
                ? scope => Activator.CreateInstance(expression.Type)
                : expression.CompileDelegate(resolutionContext);

            return base.StoreExpressionIntoLocalVariable(resolutionContext.RootScope.GetOrAddScopedItem(base.ScopeId, base.Sync, factory).AsConstant(), resolutionContext, resolveType);
        }

        /// <inheritdoc />
        public override ILifetime Create() => new SingletonLifetime();
    }
}
