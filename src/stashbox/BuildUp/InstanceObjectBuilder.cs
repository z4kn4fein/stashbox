using Stashbox.Entity;
using System;
using System.Linq.Expressions;
using Stashbox.Infrastructure;

namespace Stashbox.BuildUp
{
    internal class InstanceObjectBuilder : ObjectBuilderBase
    {
        private readonly Expression expression;

        public InstanceObjectBuilder(object instance, IContainerContext containerContext, bool isDecorator, bool shouldHandleDisposal)
            : base(containerContext, isDecorator, shouldHandleDisposal)
        {
            this.expression = Expression.Constant(instance);

            if (shouldHandleDisposal && instance is IDisposable disposable)
                containerContext.RootScope.AddDisposableTracking(disposable);
        }

        protected override Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Type resolveType)
        {
            return this.expression;
        }

        public override bool HandlesObjectDisposal => true;
    }
}