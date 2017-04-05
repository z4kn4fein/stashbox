using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;
using Stashbox.BuildUp.Expressions;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp
{
    internal class WireUpObjectBuilder : ObjectBuilderBase
    {
        private volatile Expression expression;
        private readonly object syncObject = new object();
        private readonly IExpressionBuilder expressionBuilder;

        public WireUpObjectBuilder(IContainerContext containerContext, IExpressionBuilder expressionBuilder)
            : base(containerContext)
        {
            this.expressionBuilder = expressionBuilder;
        }

        protected override Expression GetExpressionInternal(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;

                var expr = this.expressionBuilder.CreateFillExpression(serviceRegistration, Expression.Constant(serviceRegistration.RegistrationContext.ExistingInstance), 
                    resolutionInfo, serviceRegistration.ImplementationType);
                var factory = expr.CompileDelegate(Constants.ScopeExpression);

                var instance = factory(resolutionInfo.ResolutionScope);

                if (serviceRegistration.ShouldHandleDisposal && instance is IDisposable disposable)
                    resolutionInfo.RootScope.AddDisposableTracking(disposable);

                return this.expression = Expression.Constant(factory(resolutionInfo.ResolutionScope));
            }
        }

        public override bool HandlesObjectDisposal => true;
    }
}