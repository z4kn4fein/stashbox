using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Registration;
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

            if (resolutionContext.GetCircularDependencyBarrier(serviceRegistration.RegistrationNumber))
                throw new CircularDependencyException(resolveType);

            resolutionContext.SetCircularDependencyBarrier(serviceRegistration.RegistrationNumber, true);
            var result = this.PrepareExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
            resolutionContext.SetCircularDependencyBarrier(serviceRegistration.RegistrationNumber, false);
            return result;
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


                resolutionContext.AddDefinedVariable(variable);
                resolutionContext.AddInstruction(variable.AssignTo(newScope.ConvertTo(Constants.ResolutionScopeType)));

                return this.expressionBuilder.CreateExpression(containerContext,
                    serviceRegistration, resolutionContext.CreateNew(scopeParameter:
                    new KeyValue<object, ParameterExpression>(serviceRegistration.RegistrationContext.DefinedScopeName, variable)),
                    resolveType);
            }

            return this.expressionBuilder.CreateExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
        }
    }
}
