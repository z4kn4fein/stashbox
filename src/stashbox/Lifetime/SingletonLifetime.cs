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
        private volatile object instance;
        private readonly object syncObject = new object();

        /// <inheritdoc />
        public override object GetInstance(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (this.instance != null) return this.instance;
            lock (this.syncObject)
            {
                if (this.instance != null) return this.instance;
                this.instance = base.GetInstance(containerContext, objectBuilder, resolutionInfo, resolveType);
            }

            return this.instance;
        }

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return Expression.Constant(this.GetInstance(containerContext, objectBuilder, resolutionInfo, resolveType));
        }

        /// <inheritdoc />
        public override ILifetime Create()
        {
            return new SingletonLifetime();
        }

        /// <inheritdoc />
        public override void CleanUp()
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
