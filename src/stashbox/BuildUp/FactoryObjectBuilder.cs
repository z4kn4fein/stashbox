using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq.Expressions;
using Stashbox.BuildUp.Expressions;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.BuildUp
{
    internal class FactoryObjectBuilder : ObjectBuilderBase
    {
        private readonly IExpressionBuilder expressionBuilder;
        
        public FactoryObjectBuilder(IContainerContext containerContext, IExpressionBuilder expressionBuilder)
            : base(containerContext)
        {
            this.expressionBuilder = expressionBuilder;
        }
        
        protected override Expression GetExpressionInternal(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType)
        {
            Expression<Func<IDependencyResolver, object>> lambda;
            if (serviceRegistration.RegistrationContext.ContainerFactory != null)
                lambda = scope => serviceRegistration.RegistrationContext.ContainerFactory(scope);
            else
                lambda = scope => serviceRegistration.RegistrationContext.SingleFactory();
            
            var expr = Expression.Invoke(lambda, Expression.Convert(Constants.ScopeExpression, Constants.ResolverType));

            return this.expressionBuilder.CreateFillExpression(serviceRegistration, expr, resolutionInfo, resolveType);
        }
    }
}