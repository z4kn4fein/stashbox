using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
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


        public static Func<ResolutionInfo, object> CreateConstructorExpression(IContainerContext containerContext, ResolutionConstructor resolutionConstructor,
            ResolutionProperty[] properties = null)
        {
            var strategyParameter = Expression.Constant(containerContext.ResolutionStrategy, typeof(IResolutionStrategy));
            var containerContextParameter = Expression.Constant(containerContext, typeof(IContainerContext));
            var resolutionInfoParameter = Expression.Parameter(typeof(ResolutionInfo), "resolutionInfo");

            var arguments = CreateExpressionFromResolutionTargets(resolutionConstructor.Parameters, strategyParameter, containerContextParameter, resolutionInfoParameter);

            var newExpression = Expression.New(resolutionConstructor.Constructor, arguments);

            if (properties != null)
            {
                var propertyExpressions = new MemberAssignment[properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    var propertyExpression = Expression.Bind((MemberInfo)property.PropertyInfo,
                        CreateResolutionTargetExpression(property.ResolutionTarget, strategyParameter,
                            containerContextParameter, resolutionInfoParameter));
                    propertyExpressions[i] = propertyExpression;

                }

                var initExpression = Expression.MemberInit(newExpression, propertyExpressions);
                return Expression.Lambda<Func<ResolutionInfo, object>>(newExpression, new ParameterExpression[] { resolutionInfoParameter }).Compile();
            }

            return Expression.Lambda<Func<ResolutionInfo, object>>(newExpression, new ParameterExpression[] { resolutionInfoParameter }).Compile();
        }

        public static Func<ResolutionInfo, object, object> CreateMethodExpression(IContainerContext containerContext, ResolutionMethod resolutionMethod)
        {
            var strategyParameter = Expression.Constant(containerContext.ResolutionStrategy, typeof(IResolutionStrategy));
            var containerContextParameter = Expression.Constant(containerContext, typeof(IContainerContext));
            var resolutionInfoParameter = Expression.Parameter(typeof(ResolutionInfo), "resolutionInfo");
            var instanceParameter = Expression.Parameter(typeof(object), "instance");

            var arguments = CreateExpressionFromResolutionTargets(resolutionMethod.Parameters, strategyParameter, containerContextParameter, resolutionInfoParameter);

            var callExpression = Expression.Call(resolutionMethod.Method, arguments);
            return Expression.Lambda<Func<ResolutionInfo, object, object>>(callExpression, instanceParameter, resolutionInfoParameter).Compile();
        }

        private static Expression[] CreateExpressionFromResolutionTargets(ResolutionTarget[] resolutionTargets, ConstantExpression strategyParameter,
            ConstantExpression containerContextParameter, ParameterExpression resolutionInfoParameter)
        {
            var arguments = new Expression[resolutionTargets.Length];

            for (var i = 0; i < resolutionTargets.Length; i++)
            {
                var parameter = resolutionTargets[i];
                arguments[i] = CreateResolutionTargetExpression(parameter, strategyParameter, containerContextParameter, resolutionInfoParameter);
            }

            return arguments;
        }

        private static Expression CreateResolutionTargetExpression(ResolutionTarget resolutionTarget, ConstantExpression strategyParameter,
            ConstantExpression containerContextParameter, ParameterExpression resolutionInfoParameter)
        {
            var target = Expression.Constant(resolutionTarget, typeof(ResolutionTarget));
            var evaluate = Expression.Call(strategyParameter, "EvaluateResolutionTarget", null, new Expression[] { containerContextParameter, target, resolutionInfoParameter });
            var call = Expression.Convert(evaluate, resolutionTarget.TypeInformation.Type);
            return call;
        }
    }
}