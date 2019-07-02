using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.ObjectBuilders
{
    internal class FuncObjectBuilder : ObjectBuilderBase
    {
        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext,
            Type resolveType)
        {
            var internalMethodInfo = serviceRegistration.RegistrationContext.FuncDelegate.GetMethod();

            var parameters = GetFuncParametersWithScope(serviceRegistration.ImplementationType.GetSingleMethod("Invoke").GetParameters(), resolutionContext);
            if (serviceRegistration.RegistrationContext.FuncDelegate.IsCompiledLambda())
                return serviceRegistration.RegistrationContext.FuncDelegate.InvokeDelegate(parameters)
                    .AsLambda(parameters.Take(parameters.Length - 1).Cast<ParameterExpression>());

            var expr = internalMethodInfo.IsStatic
                ? internalMethodInfo.CallStaticMethod(parameters)
                : serviceRegistration.RegistrationContext.FuncDelegate.Target.AsConstant().CallMethod(internalMethodInfo, parameters);

            return expr.AsLambda(parameters.Take(parameters.Length - 1).Cast<ParameterExpression>());
        }

        private static Expression[] GetFuncParametersWithScope(IList<ParameterInfo> parameterInfos, ResolutionContext resolutionContext)
        {
            var length = parameterInfos.Count;
            var expressions = new Expression[length + 1];

            for (var i = 0; i < length; i++)
                expressions[i] = parameterInfos[i].ParameterType.AsParameter();

            expressions[expressions.Length - 1] = resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType);

            return expressions;
        }
    }
}
