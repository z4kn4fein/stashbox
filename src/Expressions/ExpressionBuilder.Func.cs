using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Expressions
{
    internal static partial class ExpressionBuilder
    {
        private static Expression GetExpressionForFunc(FuncRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            var internalMethodInfo = serviceRegistration.FuncDelegate.GetMethod();

            var parameters = GetFuncParametersWithScope(serviceRegistration.ImplementationType.GetMethod("Invoke")!.GetParameters(), resolutionContext);
            if (serviceRegistration.FuncDelegate.IsCompiledLambda())
                return serviceRegistration.FuncDelegate.InvokeDelegate(parameters)
                    .AsLambda(parameters.Take(parameters.Length - 1).Cast<ParameterExpression>());

            var expr = internalMethodInfo.IsStatic
                ? internalMethodInfo.CallStaticMethod(parameters)
                : serviceRegistration.FuncDelegate.Target.AsConstant().CallMethod(internalMethodInfo, parameters);

            return expr.AsLambda(parameters.Take(parameters.Length - 1).Cast<ParameterExpression>());
        }

        private static Expression[] GetFuncParametersWithScope(IList<ParameterInfo> parameterInfos, ResolutionContext resolutionContext)
        {
            var length = parameterInfos.Count;
            var expressions = new Expression[length + 1];

            for (var i = 0; i < length; i++)
                expressions[i] = parameterInfos[i].ParameterType.AsParameter();

            expressions[expressions.Length - 1] = resolutionContext.CurrentScopeParameter;

            return expressions;
        }
    }
}