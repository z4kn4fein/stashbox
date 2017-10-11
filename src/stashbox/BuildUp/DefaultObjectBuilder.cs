using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp
{
    internal class DefaultObjectBuilder : ObjectBuilderBase
    {
        private readonly IExpressionBuilder expressionBuilder;

        public DefaultObjectBuilder(IExpressionBuilder expressionBuilder)
        {
            this.expressionBuilder = expressionBuilder;
        }

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (!containerContext.ContainerConfigurator.ContainerConfiguration.CircularDependencyTrackingEnabled)
                return this.PrepareExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

            using (new CircularDependencyBarrier(resolutionContext, serviceRegistration))
                return this.PrepareExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
        }

        private Expression PrepareExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (serviceRegistration.RegistrationContext.DefinedScopeName != null)
            {
                var variable = Expression.Variable(Constants.ResolutionScopeType);
                return Expression.Block(new[] { variable },
                     Expression.Assign(variable, Expression.Convert(Expression.Call(Expression.Convert(resolutionContext.CurrentScopeParameter, Constants.ResolverType),
                        Constants.BeginScopeMethod,
                            Expression.Constant(serviceRegistration.RegistrationContext.DefinedScopeName)),
                                Constants.ResolutionScopeType)),

                     this.expressionBuilder.CreateExpression(containerContext, serviceRegistration,
                         resolutionContext.CreateNew(scopeParameter: new KeyValue<object, ParameterExpression>(serviceRegistration.RegistrationContext.DefinedScopeName, variable)),
                            resolveType)
                     );
            }

            return this.expressionBuilder.CreateExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
        }
    }
}
