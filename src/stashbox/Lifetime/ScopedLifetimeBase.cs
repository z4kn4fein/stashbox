using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a shared base class for scoped lifetimes.
    /// </summary>
    public abstract class ScopedLifetimeBase : LifetimeBase
    {
        private readonly object syncObject = new object();

        private volatile Func<IResolutionScope, object> factoryDelegate;

        /// <summary>
        /// The id of the scope.
        /// </summary>
        protected readonly object ScopeId = new object();

        /// <summary>
        /// Produces a cached factory delegate to create scoped instances.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="objectBuilder">The object builder.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <param name="resolveType">The resolve type.</param>
        /// <returns></returns>
        public Func<IResolutionScope, object> GetFactoryDelegate(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            if (this.factoryDelegate != null) return this.factoryDelegate;
            lock (this.syncObject)
            {
                if (this.factoryDelegate != null) return this.factoryDelegate;
                var expr = base.GetExpression(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
                if (expr == null)
                    return null;

                return this.factoryDelegate = expr.CompileDelegate(resolutionContext.CurrentScopeParameter);
            }
        }
    }
}
