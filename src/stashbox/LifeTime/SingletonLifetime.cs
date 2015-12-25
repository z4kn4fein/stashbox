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

        public Expression GetExpression(IObjectBuilder objectBuilder, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            var callExpression = Expression.Call(Expression.Constant(this), "GetInstance", null, Expression.Constant(objectBuilder), resolutionInfoExpression, Expression.Constant(resolveType));
            return Expression.Convert(callExpression, resolveType.Type);
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
