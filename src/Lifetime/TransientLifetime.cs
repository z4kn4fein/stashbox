using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a transient lifetime.
    /// </summary>
    public class TransientLifetime : ExpressionLifetimeDescriptor
    {
        /// <inheritdoc />
        protected override int LifeSpan => 0;

        /// <inheritdoc />
        protected override string Name => nameof(TransientLifetime);

        /// <inheritdoc />
        protected override Expression ApplyLifetime(Expression expression,
            IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType) =>
            expression;
    }
}
