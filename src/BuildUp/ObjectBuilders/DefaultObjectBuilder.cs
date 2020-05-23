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

            if (resolutionContext.GetCircularDependencyBarrier(serviceRegistration.RegistrationId))
                throw new CircularDependencyException(resolveType);

            resolutionContext.SetCircularDependencyBarrier(serviceRegistration.RegistrationId, true);
            var result = this.PrepareExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
            resolutionContext.SetCircularDependencyBarrier(serviceRegistration.RegistrationId, false);
            return result;
        }

        private Expression PrepareExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (serviceRegistration.RegistrationContext.DefinedScopeName == null)
                return this.expressionBuilder.ConstructExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

            var variable = Constants.ResolutionScopeType.AsVariable();

            var newScope = resolutionContext.CurrentScopeParameter
                .ConvertTo(Constants.ResolverType)
                .CallMethod(Constants.BeginScopeMethod,
                    serviceRegistration.RegistrationContext.DefinedScopeName.AsConstant(),
                    true.AsConstant());

            var newContext = resolutionContext.BeginNewScopeContext(new KeyValue<object, ParameterExpression>(serviceRegistration.RegistrationContext.DefinedScopeName, variable));

            resolutionContext.AddDefinedVariable(variable);
            resolutionContext.AddInstruction(variable.AssignTo(newScope.ConvertTo(Constants.ResolutionScopeType)));

            var expression = this.expressionBuilder.ConstructExpression(containerContext, serviceRegistration, newContext, resolveType);

            foreach (var definedVariable in newContext.DefinedVariables.Walk())
                resolutionContext.AddDefinedVariable(definedVariable.Value);

            foreach (var instruction in newContext.SingleInstructions)
                resolutionContext.AddInstruction(instruction);

            return expression;
        }
    }
}
