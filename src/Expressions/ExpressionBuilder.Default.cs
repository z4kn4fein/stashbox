﻿using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Expressions
{
    internal static partial class ExpressionBuilder
    {
        private static Expression? GetExpressionForDefault(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            if (resolutionContext.CircularDependencyBarrier.Contains(serviceRegistration.RegistrationId))
                throw new CircularDependencyException(serviceRegistration.ImplementationType);

            resolutionContext.CircularDependencyBarrier.Add(serviceRegistration.RegistrationId);
            var result = PrepareDefaultExpression(serviceRegistration, resolutionContext);
            resolutionContext.CircularDependencyBarrier.Pop();
            return result;
        }

        private static Expression? PrepareDefaultExpression(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            var definedScopeName = serviceRegistration.Options.GetOrDefault(RegistrationOption.DefinedScopeName);
            if (definedScopeName != null)
            {
                var variable = Constants.ResolutionScopeType.AsVariable();

                var newScope = resolutionContext.CurrentScopeParameter
                    .CallMethod(Constants.BeginScopeMethod,
                        definedScopeName.AsConstant(),
                        true.AsConstant());

                var newScopeContext = resolutionContext.BeginNewScopeContext(new ReadOnlyKeyValue<object, ParameterExpression>(definedScopeName, variable));

                resolutionContext.AddDefinedVariable(variable);
                resolutionContext.AddInstruction(variable.AssignTo(newScope.ConvertTo(Constants.ResolutionScopeType)));

                var expression = ExpressionFactory.ConstructExpression(serviceRegistration, newScopeContext);

                foreach (var definedVariable in newScopeContext.DefinedVariables.Walk())
                    resolutionContext.AddDefinedVariable(definedVariable);

                foreach (var instruction in newScopeContext.SingleInstructions)
                    resolutionContext.AddInstruction(instruction);

                return expression;
            }

            return ExpressionFactory.ConstructExpression(serviceRegistration, resolutionContext);
        }
    }
}
