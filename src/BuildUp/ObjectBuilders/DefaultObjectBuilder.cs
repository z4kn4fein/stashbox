using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.ObjectBuilders
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
            if (!containerContext.ContainerConfiguration.CircularDependencyTrackingEnabled)
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
            if (serviceRegistration.RegistrationContext.DefinedScopeName == null)
                return this.expressionBuilder.CreateExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

            var variable = Constants.ResolutionScopeType.AsVariable();

            var newScope = resolutionContext.CurrentScopeParameter
                .ConvertTo(Constants.ResolverType)
                .CallMethod(Constants.BeginScopeMethod,
                    serviceRegistration.RegistrationContext.DefinedScopeName.AsConstant(),
                    true.AsConstant());


            resolutionContext.AddDefinedVariable(variable);
            resolutionContext.AddInstruction(variable.AssignTo(newScope.ConvertTo(Constants.ResolutionScopeType)));

            var newContext = resolutionContext.Clone(scopeParameter:
                    new KeyValue<object, ParameterExpression>(serviceRegistration.RegistrationContext.DefinedScopeName, variable));

            var expression = this.expressionBuilder.CreateExpression(containerContext, serviceRegistration, newContext, resolveType);

            foreach (var definedVariable in newContext.DefinedVariables.Repository)
                resolutionContext.AddDefinedVariable(definedVariable.Key, definedVariable.Value);

            foreach (var instruction in newContext.SingleInstructions)
                resolutionContext.AddInstruction(instruction);

            return expression;
        }
    }
}
