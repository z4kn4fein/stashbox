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
        private object instance;
        private readonly object syncObject = new object();
       
        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (this.instance != null) return Expression.Constant(this.instance);
            lock (this.syncObject)
            {
                if (this.instance != null) return Expression.Constant(this.instance);
                var expr = base.GetExpression(containerContext, objectBuilder, resolutionInfo, resolveType);
                this.instance = Expression.Lambda<Func<object>>(expr).Compile()();
                return Expression.Constant(this.instance);
            }
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
