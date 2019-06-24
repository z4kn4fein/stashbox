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
            var expression = serviceRegistration.RegistrationContext.ContainerFactory != null
                ? ConstructFactoryExpression(serviceRegistration.RegistrationContext.ContainerFactory,
                    resolutionContext,
                    resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType))
                : ConstructFactoryExpression(serviceRegistration.RegistrationContext.SingleFactory, resolutionContext);

            return this.expressionBuilder.CreateFillExpression(containerContext, serviceRegistration, expression, resolutionContext, resolveType);
        }

        private static Expression ConstructFactoryExpression(Delegate @delegate, ResolutionContext resolutionContext, params Expression[] parameters)
        {
            if (@delegate.IsCompiledLambda())
                return @delegate.InvokeDelegate(parameters);

            var method = @delegate.GetMethod();
            return method.IsStatic
                ? method.CallStaticMethod(parameters)
                : method.CallMethod(@delegate.Target.AsConstant(), parameters);
        }
    }
}