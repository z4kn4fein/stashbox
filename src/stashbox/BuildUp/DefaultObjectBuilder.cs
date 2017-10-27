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
                var variable = Constants.ResolutionScopeType.AsVariable();

                var newScope = resolutionContext.CurrentScopeParameter
                    .ConvertTo(Constants.ResolverType)
                    .CallMethod(Constants.BeginScopeMethod,
                        serviceRegistration.RegistrationContext.DefinedScopeName.AsConstant(),
                        true.AsConstant());

                return Expression.Block(new[] { variable },
                     variable.AssignTo(newScope.ConvertTo(Constants.ResolutionScopeType)),

                     this.expressionBuilder.CreateExpression(containerContext, serviceRegistration,
                         resolutionContext.CreateNew(scopeParameter: new KeyValue<object, ParameterExpression>(serviceRegistration.RegistrationContext.DefinedScopeName, variable)),
                            resolveType)
                     );
            }

            return this.expressionBuilder.CreateExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
        }
    }
}
