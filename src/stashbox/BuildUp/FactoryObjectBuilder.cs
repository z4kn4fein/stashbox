using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class FactoryObjectBuilder : ObjectBuilderBase
    {
        private readonly IExpressionBuilder expressionBuilder;

        public FactoryObjectBuilder(IExpressionBuilder expressionBuilder)
        {
            this.expressionBuilder = expressionBuilder;
        }

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            Expression<Func<IDependencyResolver, object>> lambda;
            if (serviceRegistration.RegistrationContext.ContainerFactory != null)
                lambda = scope => serviceRegistration.RegistrationContext.ContainerFactory(scope);
            else
                lambda = scope => serviceRegistration.RegistrationContext.SingleFactory();

            var expr = Expression.Invoke(lambda, Expression.Convert(Constants.ScopeExpression, Constants.ResolverType));

            return this.expressionBuilder.CreateFillExpression(containerContext, serviceRegistration, expr, resolutionInfo, resolveType);
        }
    }
}