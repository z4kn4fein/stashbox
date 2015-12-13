using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    public class InstanceObjectBuilder : IObjectBuilder
    {
        private object instance;
        private readonly object syncObject = new object();

        public InstanceObjectBuilder(object instance)
        {
            this.instance = instance;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo)
        {
            return Expression.Constant(this.instance);
        }

        public object BuildInstance(ResolutionInfo resolutionInfo)
        {
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
    }
}