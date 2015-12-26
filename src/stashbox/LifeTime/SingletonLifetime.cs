using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.LifeTime
{
    public class SingletonLifetime : ILifetime
    {
        private volatile object instance;
        private readonly object syncObject = new object();

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

        public Expression GetExpression(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            return Expression.Constant(this.GetInstance(objectBuilder, resolutionInfo, resolveType));
        }

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
