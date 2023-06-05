using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Expressions;

internal static partial class ExpressionBuilder
{
    private static Expression GetExpressionForFunc(ServiceRegistration serviceRegistration, Delegate func, ResolutionContext resolutionContext)
    {
        var internalMethodInfo = func.GetMethod();

        var parameters = GetFuncParametersWithScope(serviceRegistration.ImplementationType.GetMethod("Invoke")!.GetParameters(), resolutionContext);
        if (func.IsCompiledLambda())
            return func.InvokeDelegate(parameters)
                .AsLambda(parameters.Take(parameters.Length - 1).Cast<ParameterExpression>());

        var expr = internalMethodInfo.IsStatic
            ? internalMethodInfo.CallStaticMethod(parameters)
            : func.Target.AsConstant().CallMethod(internalMethodInfo, parameters);

        return expr.AsLambda(parameters.Take(parameters.Length - 1).Cast<ParameterExpression>());
    }

    private static Expression[] GetFuncParametersWithScope(IList<ParameterInfo> parameterInfos, ResolutionContext resolutionContext)
    {
        var length = parameterInfos.Count;
        var expressions = new Expression[length + 1];

        for (var i = 0; i < length; i++)
        {
            expressions[i] = parameterInfos[i].ParameterType.AsParameter();
        }
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        expressions[^1] = resolutionContext.CurrentScopeParameter;
#else
        expressions[expressions.Length - 1] = resolutionContext.CurrentScopeParameter;
#endif
        return expressions;
    }
}