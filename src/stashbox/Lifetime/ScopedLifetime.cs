using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a scoped lifetime.
    /// </summary>
    public class ScopedLifetime : ScopedLifetimeBase
    {
        /// <inheritdoc />
        public override ILifetime Create() => new ScopedLifetime();

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            var variable = resolutionContext.GlobalParameters.GetOrDefault(base.ScopeId);
            if (variable != null)
                return variable;

            var factory = base.GetFactoryDelegate(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
            if (factory == null)
                return null;

            var expression = resolutionContext.CurrentScopeParameter
                .CallMethod(Constants.GetOrAddScopedItemMethod, base.ScopeId.AsConstant(), factory.AsConstant())
                .ConvertTo(resolveType);

            return base.StoreExpressionIntoLocalVariable(expression, resolutionContext, resolveType);
        }
    }
}
