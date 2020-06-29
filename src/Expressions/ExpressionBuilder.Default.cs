using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data;
using System.Linq.Expressions;

namespace Stashbox.Expressions
{
    internal partial class ExpressionBuilder
    {
        private Expression GetExpressionForDefault(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            if (resolutionContext.WeAreInCircle(serviceRegistration.RegistrationId))
                throw new CircularDependencyException(serviceRegistration.ImplementationType);

            resolutionContext.PullOutCircularDependencyBarrier(serviceRegistration.RegistrationId);
            var result = this.PrepareDefaultExpression(serviceRegistration, resolutionContext);
            resolutionContext.LetDownCircularDependencyBarrier();
            return result;
        }

        private Expression PrepareDefaultExpression(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            if (serviceRegistration.RegistrationContext.DefinedScopeName == null)
                return this.expressionFactory.ConstructExpression(serviceRegistration, resolutionContext);

            var variable = Constants.ResolutionScopeType.AsVariable();

            var newScope = resolutionContext.CurrentScopeParameter
                .CallMethod(Constants.BeginScopeMethod,
                    serviceRegistration.RegistrationContext.DefinedScopeName.AsConstant(),
                    true.AsConstant());

            var newScopeContext = resolutionContext.BeginNewScopeContext(new KeyValue<object, ParameterExpression>(serviceRegistration.RegistrationContext.DefinedScopeName, variable));

            resolutionContext.AddDefinedVariable(variable);
            resolutionContext.AddInstruction(variable.AssignTo(newScope.ConvertTo(Constants.ResolutionScopeType)));

            var expression = this.expressionFactory.ConstructExpression(serviceRegistration, newScopeContext);

            foreach (var definedVariable in newScopeContext.DefinedVariables.Walk())
                resolutionContext.AddDefinedVariable(definedVariable);

            foreach (var instruction in newScopeContext.SingleInstructions)
                resolutionContext.AddInstruction(instruction);

            return expression;
        }
    }
}
