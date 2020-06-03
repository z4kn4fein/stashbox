using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Expressions
{
    internal partial class ExpressionBuilder
    {
        private Expression GetExpressionForDefault(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (resolutionContext.WeAreInCircle(serviceRegistration.RegistrationId))
                throw new CircularDependencyException(resolveType);

            resolutionContext.PullOutCircularDependencyBarrier(serviceRegistration.RegistrationId);
            var result = this.PrepareDefaultExpression(serviceRegistration, resolutionContext, resolveType);
            resolutionContext.LetDownCircularDependencyBarrier();
            return result;
        }

        private Expression PrepareDefaultExpression(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (serviceRegistration.RegistrationContext.DefinedScopeName == null)
                return this.expressionFactory.ConstructExpression(serviceRegistration, resolutionContext, resolveType);

            var variable = Constants.ResolutionScopeType.AsVariable();

            var newScope = resolutionContext.CurrentScopeParameter
                .ConvertTo(Constants.ResolverType)
                .CallMethod(Constants.BeginScopeMethod,
                    serviceRegistration.RegistrationContext.DefinedScopeName.AsConstant(),
                    true.AsConstant());

            var newScopeContext = resolutionContext.BeginNewScopeContext(new KeyValue<object, ParameterExpression>(serviceRegistration.RegistrationContext.DefinedScopeName, variable));

            resolutionContext.AddDefinedVariable(variable);
            resolutionContext.AddInstruction(variable.AssignTo(newScope.ConvertTo(Constants.ResolutionScopeType)));

            var expression = this.expressionFactory.ConstructExpression(serviceRegistration, newScopeContext, resolveType);

            foreach (var definedVariable in newScopeContext.DefinedVariables.Walk())
                resolutionContext.AddDefinedVariable(definedVariable.Value);

            foreach (var instruction in newScopeContext.SingleInstructions)
                resolutionContext.AddInstruction(instruction);

            return expression;
        }
    }
}
