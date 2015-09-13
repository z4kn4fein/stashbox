﻿using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;

namespace Stashbox.BuildUp
{
    public class InstanceObjectBuilder : IObjectBuilder
    {
        private object instance;
        private readonly object syncObject = new object();

        public InstanceObjectBuilder(object instance)
        {
            Shield.EnsureNotNull(instance);

            this.instance = instance;
        }

        public object BuildInstance(IBuilderContext builderContext, ResolutionInfo resolutionInfo)
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