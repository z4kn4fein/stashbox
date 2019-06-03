using Stashbox.BuildUp.Expressions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
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

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            Expression<Func<IDependencyResolver, object>> lambda;
            if (serviceRegistration.RegistrationContext.ContainerFactory != null)
                lambda = serviceRegistration.RegistrationContext.ContainerFactory.GetMethod()
                    .CallMethod(serviceRegistration.RegistrationContext.ContainerFactory.Target.AsConstant(), resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType))
                    .AsLambda<Func<IDependencyResolver, object>>(Constants.ResolverType.AsParameter());
            else
                lambda = serviceRegistration.RegistrationContext.SingleFactory.GetMethod()
                    .CallMethod(serviceRegistration.RegistrationContext.SingleFactory.Target.AsConstant())
                    .AsLambda<Func<IDependencyResolver, object>>(Constants.ResolverType.AsParameter());

            var expr = lambda.InvokeLambda(resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType));

            return this.expressionBuilder.CreateFillExpression(containerContext, serviceRegistration, expr, resolutionContext, resolveType);
        }
    }
}