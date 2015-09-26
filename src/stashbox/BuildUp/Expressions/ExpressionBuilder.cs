using Stashbox.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions
{
    public class ExpressionBuilder
    {
        public static Func<object[], object> BuildConstructorExpression(ConstructorInfo constructor,
            IEnumerable<TypeInformation> parameters, Type typeInfo)
        {
            var parameter = Expression.Parameter(typeof(object[]), "parameters");
            var parameterExpressions = CreateParameterExpressions(parameters, typeInfo, parameter);
            var newExpression = Expression.New(constructor, parameterExpressions);

            return Expression.Lambda<Func<object[], object>>(newExpression, parameter).Compile();
        }

        public static Action<object, object[]> BuildMethodExpression(MethodInfo method,
            IEnumerable<TypeInformation> parameters, Type typeInfo)
        {
            var parameter = Expression.Parameter(typeof(object[]), "parameters");
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var convertedInstance = Expression.Convert(instanceParameter, method.DeclaringType);
            var parameterExpressions = CreateParameterExpressions(parameters, typeInfo, parameter);
            var newExpression = Expression.Call(convertedInstance, method, parameterExpressions);

            return Expression.Lambda<Action<object, object[]>>(newExpression, instanceParameter, parameter).Compile();
        }

        private static Expression[] CreateParameterExpressions(IEnumerable<TypeInformation> parameters,
            Type typeInfo, Expression parameter)
        {
            var typeInformations = parameters as TypeInformation[] ?? parameters.ToArray();
            var count = typeInformations.Count();
            var parameterExpressions = new Expression[count];

            for (var i = 0; i < count; i++)
            {
                Expression index = Expression.Constant(i);

                var parameterInformation = typeInformations.ElementAt(i) as TypeInformation;

                if (parameterInformation == null) continue;
                var paramType = parameterInformation.Type.IsGenericParameter ?
                    typeInfo.GenericTypeArguments[parameterInformation.Type.GenericParameterPosition] :
                    parameterInformation.Type;

                Expression indexedParamExpression = Expression.ArrayIndex(parameter, index);
                Expression castExpression = Expression.Convert(indexedParamExpression, paramType);
                parameterExpressions[i] = castExpression;
            }

            return parameterExpressions;
        }
    }
}