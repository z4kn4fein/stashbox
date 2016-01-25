using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.LifeTime
{
    /// <summary>
    /// Represents a singleton lifetime manager.
    /// </summary>
    public class SingletonLifetime : ILifetime
    {
        private volatile object instance;
        private readonly object syncObject = new object();

        /// <summary>
        /// Gets the instance managed by the <see cref="SingletonLifetime"/>
        /// </summary>
        /// <param name="objectBuilder">An <see cref="IObjectBuilder"/> implementation.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The lifetime managed object.</returns>
        public object GetInstance(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (this.instance != null) return this.instance;
            lock (this.syncObject)
            {
                if (this.instance != null) return this.instance;
                this.instance = objectBuilder.BuildInstance(resolutionInfo, resolveType);
            }

            return this.instance;
        }

        /// <summary>
        /// Gets the expression for getting the instance managed by the <see cref="SingletonLifetime"/>
        /// </summary>
        /// <param name="objectBuilder">An <see cref="IObjectBuilder"/> implementation.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolutionInfoExpression">The expression of the info about the actual resolution.</param>
        /// <param name="resolveType">The type info about the resolved type.</param>
        /// <returns>The lifetime managed object.</returns>
        public Expression GetExpression(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            return Expression.Constant(this.GetInstance(objectBuilder, resolutionInfo, resolveType));
        }

        /// <summary>
        /// Cleans up the lifetime manager.
        /// </summary>
        public void CleanUp()
        {
            if (this.instance == null) return;
            lock (this.syncObject)
            {
                if (this.instance == null) return;
                var disposable = this.instance as IDisposable;
                disposable?.Dispose();
                this.instance = null;
            }
        }
    }
}
