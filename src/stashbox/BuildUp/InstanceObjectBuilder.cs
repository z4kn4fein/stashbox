using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class InstanceObjectBuilder : IObjectBuilder
    {
        private readonly Expression expression;
        private object instance;

        public InstanceObjectBuilder(object instance)
        {
            this.expression = Expression.Constant(instance);
            this.instance = instance;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return this.expression;
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