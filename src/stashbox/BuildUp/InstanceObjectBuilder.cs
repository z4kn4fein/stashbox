using Stashbox.Entity;
using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp
{
    internal class InstanceObjectBuilder : ObjectBuilderBase
    {
        private readonly Expression expression;
        private object instance;

        public InstanceObjectBuilder(object instance, IContainerContext containerContext)
            : base(containerContext)
        {
            this.expression = Expression.Constant(instance);
            this.instance = instance;
        }

        protected override Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Type resolveType)
        {
            return this.expression;
        }

        public override bool HandlesObjectDisposal => true;

        public override void CleanUp()
        {
            if (this.instance == null) return;
            var disposable = this.instance as IDisposable;
            disposable?.Dispose();
            this.instance = null;
        }
    }
}