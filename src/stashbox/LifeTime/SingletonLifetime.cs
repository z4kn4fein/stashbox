using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;

namespace Stashbox.LifeTime
{
    public class SingletonLifetime : ILifetime
    {
        private object instance;
        private readonly object syncObject = new object();

        public object GetInstance(IObjectBuilder objectBuilder, IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            if (this.instance != null) return this.instance;
            lock (this.syncObject)
            {
                if (this.instance != null) return this.instance;
                this.instance = objectBuilder.BuildInstance(builderContext, resolutionInfo);
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
    }
}
