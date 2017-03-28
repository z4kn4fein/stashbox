using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq.Expressions;
using Stashbox.BuildUp.Expressions;

namespace Stashbox.BuildUp
{
    internal class WireUpObjectBuilder : ObjectBuilderBase
    {
        private readonly object instance;
        private volatile Expression expression;
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly object syncObject = new object();
        private readonly IContainerContext containerContext;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly InjectionParameter[] injectionParameters;
        private readonly IExpressionBuilder expressionBuilder;

        public WireUpObjectBuilder(object instance, IContainerExtensionManager containerExtensionManager, IContainerContext containerContext,
            IMetaInfoProvider metaInfoProvider, IExpressionBuilder expressionBuilder, 
            InjectionParameter[] injectionParameters, bool isDecorator, bool shouldHandleDisposal)
            : base(containerContext, isDecorator, shouldHandleDisposal)
        {
            this.instance = instance;
            this.containerExtensionManager = containerExtensionManager;
            this.containerContext = containerContext;
            this.metaInfoProvider = metaInfoProvider;
            this.injectionParameters = injectionParameters;
            this.expressionBuilder = expressionBuilder;

            if (shouldHandleDisposal && instance is IDisposable disposable)
                containerContext.RootScope.AddDisposableTracking(disposable);
        }

        protected override Expression GetExpressionInternal(ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;

                var expr = this.expressionBuilder.CreateFillExpression(this.containerExtensionManager, this.containerContext,
                    Expression.Constant(this.instance), resolutionInfo, this.metaInfoProvider.TypeTo, this.injectionParameters,
                    this.metaInfoProvider.GetResolutionMembers(resolutionInfo, this.injectionParameters),
                    this.metaInfoProvider.GetResolutionMethods(resolutionInfo, this.injectionParameters));
                var factory = expr.CompileDelegate(Constants.ScopeExpression);

                return this.expression = Expression.Constant(factory(resolutionInfo.ResolutionScope));
            }
        }

        public override bool HandlesObjectDisposal => true;
    }
}