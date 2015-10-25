using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.DelegateFactory
{
    public delegate object CreateInstance(params object[] args);
    public delegate void InvokeMethod(object instance, params object[] args);
    public delegate object InvokeMethodWithResult(object instance, params object[] args);

    public class ExpressionDelegateFactory
    {
        public static CreateInstance BuildConstructorExpression(ConstructorInfo constructor,
            IEnumerable<Type> parameters, Type typeInfo)
        {
            var parameter = Expression.Parameter(typeof(object[]), "parameters");
            var parameterExpressions = CreateParameterExpressions(parameters, typeInfo, parameter);
            var newExpression = Expression.New(constructor, parameterExpressions);

            return Expression.Lambda<CreateInstance>(newExpression, parameter).Compile();
        }

        public static InvokeMethod BuildMethodExpression(MethodInfo method,
            IEnumerable<Type> parameters, Type typeInfo)
        {
            var parameter = Expression.Parameter(typeof(object[]), "parameters");
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var convertedInstance = Expression.Convert(instanceParameter, method.DeclaringType);
            var parameterExpressions = CreateParameterExpressions(parameters, typeInfo, parameter);
            var newExpression = Expression.Call(convertedInstance, method, parameterExpressions);

            return Expression.Lambda<InvokeMethod>(newExpression, instanceParameter, parameter).Compile();
        }

        public static InvokeMethodWithResult BuildMethodWithResultExpression(MethodInfo method,
            IEnumerable<Type> parameters, Type typeInfo)
        {
            var parameter = Expression.Parameter(typeof(object[]), "parameters");
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var convertedInstance = Expression.Convert(instanceParameter, method.DeclaringType);
            var parameterExpressions = CreateParameterExpressions(parameters, typeInfo, parameter);
            var newExpression = Expression.Call(convertedInstance, method, parameterExpressions);

            return Expression.Lambda<InvokeMethodWithResult>(newExpression, instanceParameter, parameter).Compile();
        }

        private static Expression[] CreateParameterExpressions(IEnumerable<Type> parameters,
            Type typeInfo, Expression parameter)
        {
            var typeInformations = parameters as Type[] ?? parameters.ToArray();
            var count = typeInformations.Count();
            var parameterExpressions = new Expression[count];

            for (var i = 0; i < count; i++)
            {
                Expression index = Expression.Constant(i);

                var parameterInformation = typeInformations.ElementAt(i) as Type;

                if (parameterInformation == null) continue;
                var paramType = parameterInformation.IsGenericParameter ?
                    typeInfo.GenericTypeArguments[parameterInformation.GenericParameterPosition] :
                    parameterInformation;

                Expression indexedParamExpression = Expression.ArrayIndex(parameter, index);
                Expression castExpression = Expression.Convert(indexedParamExpression, paramType);
                parameterExpressions[i] = castExpression;
            }

            return parameterExpressions;
        }
    }
}