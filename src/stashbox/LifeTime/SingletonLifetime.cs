using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.LifeTime
{
    public class SingletonLifetime : ILifetime
    {
        private object instance;
        private readonly object syncObject = new object();

        public object GetInstance(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo)
        {
            if (this.instance != null) return this.instance;
            lock (this.syncObject)
            {
                if (this.instance != null) return this.instance;
                this.instance = objectBuilder.BuildInstance(resolutionInfo);
            }

            return this.instance;
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

        public Expression GetExpression(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo)
        {
            if (this.instance != null) return Expression.Constant(this.instance);
            lock (this.syncObject)
            {
                if (this.instance != null) return Expression.Constant(this.instance);
                this.instance = objectBuilder.BuildInstance(resolutionInfo);
            }

            return Expression.Constant(this.instance);
        }
    }
}
