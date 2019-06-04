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
            MethodCallExpression expr;
            if (serviceRegistration.RegistrationContext.ContainerFactory != null)
                expr = serviceRegistration.RegistrationContext.ContainerFactory.GetMethod()
                    .CallMethod(serviceRegistration.RegistrationContext.ContainerFactory.Target.AsConstant(),
                        resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType));
            else
                expr = serviceRegistration.RegistrationContext.SingleFactory.GetMethod()
                    .CallMethod(serviceRegistration.RegistrationContext.SingleFactory.Target.AsConstant());

            return this.expressionBuilder.CreateFillExpression(containerContext, serviceRegistration, expr, resolutionContext, resolveType);
        }
    }
}