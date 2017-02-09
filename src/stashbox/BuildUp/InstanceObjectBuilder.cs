using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class InstanceObjectBuilder : IObjectBuilder
    {
        private object instance;

        public InstanceObjectBuilder(object instance)
        {
            this.instance = instance;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return Expression.Constant(this.instance);
        }

        public bool HandlesObjectDisposal => true;

        public void CleanUp()
        {
            if (this.instance == null) return;
            var disposable = this.instance as IDisposable;
            disposable?.Dispose();
            this.instance = null;
        }
    }
}