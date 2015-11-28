using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Extensions;
using Stashbox.Infrastructure;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.DelegateFactory
{
    public delegate object CreateInstance(ResolutionInfo resolutionInfo);
    public delegate void InvokeMethod(ResolutionInfo resolutionInfo, object instance);

    public class ExpressionDelegateFactory
    {
        public static CreateInstance CreateConstructorExpression(IContainerContext containerContext, ResolutionConstructor resolutionConstructor,
            ResolutionProperty[] properties = null)
        {
            var strategyParameter = Expression.Constant(containerContext.ResolutionStrategy, typeof(IResolutionStrategy));
            var containerContextParameter = Expression.Constant(containerContext, typeof(IContainerContext));
            var resolutionInfoParameter = Expression.Parameter(typeof(ResolutionInfo), "resolutionInfo");

            var arguments = CreateExpressionFromResolutionTargets(resolutionConstructor.Parameters, strategyParameter, containerContextParameter, resolutionInfoParameter);

            var newExpression = Expression.New(resolutionConstructor.Constructor, arguments);

            if (properties != null)
            {
                var length = properties.Length;
                var propertyExpressions = new MemberAssignment[length];
                for (int i = 0; i < length; i++)
                {
                    var property = properties[i];
                    var propertyExpression = Expression.Bind(property.PropertyInfo,
                        CreateResolutionTargetExpression(property.ResolutionTarget, strategyParameter,
                            containerContextParameter, resolutionInfoParameter));
                    propertyExpressions[i] = propertyExpression;

                }

                var initExpression = Expression.MemberInit(newExpression, propertyExpressions);
                return Expression.Lambda<CreateInstance>(initExpression, new ParameterExpression[] { resolutionInfoParameter }).Compile();
            }

            return Expression.Lambda<CreateInstance>(newExpression, new ParameterExpression[] { resolutionInfoParameter }).Compile();
        }

        public static Expression CreateExpression(IContainerContext containerContext, ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo,
            ResolutionProperty[] properties = null)
        {
            var copiedParameters = resolutionConstructor.Parameters.CreateCopy();
            var length = copiedParameters.Count;
            var arguments = new Expression[length];

            for (var i = 0; i < length; i++)
            {
                var parameter = copiedParameters[i];
                arguments[i] = containerContext.ResolutionStrategy.GetExpressionForResolutionTarget(parameter, resolutionInfo);
            }

            var newExpression = Expression.New(resolutionConstructor.Constructor, arguments);

            if (properties != null)
            {
                var copiedProperties = properties.CreateCopy();
                var propLength = copiedProperties.Count;
                var propertyExpressions = new MemberAssignment[propLength];
                for (int i = 0; i < propLength; i++)
                {
                    var property = copiedProperties[i];
                    var propertyExpression = Expression.Bind(property.PropertyInfo,
                        containerContext.ResolutionStrategy.GetExpressionForResolutionTarget(property.ResolutionTarget, resolutionInfo));
                    propertyExpressions[i] = propertyExpression;
                }

                return Expression.MemberInit(newExpression, propertyExpressions);
            }

            return newExpression;
        }

        public static InvokeMethod CreateMethodExpression(IContainerContext containerContext, ResolutionTarget[] parameters, MethodInfo methodInfo)
        {
            var strategyParameter = Expression.Constant(containerContext.ResolutionStrategy, typeof(IResolutionStrategy));
            var containerContextParameter = Expression.Constant(containerContext, typeof(IContainerContext));
            var resolutionInfoParameter = Expression.Parameter(typeof(ResolutionInfo), "resolutionInfo");
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var convertedInstance = Expression.Convert(instanceParameter, methodInfo.DeclaringType);

            var arguments = CreateExpressionFromResolutionTargets(parameters, strategyParameter, containerContextParameter, resolutionInfoParameter);

            var callExpression = Expression.Call(convertedInstance, methodInfo, arguments);
            return Expression.Lambda<InvokeMethod>(callExpression, resolutionInfoParameter, instanceParameter).Compile();
        }

        private static Expression[] CreateExpressionFromResolutionTargets(ResolutionTarget[] resolutionTargets, ConstantExpression strategyParameter,
            ConstantExpression containerContextParameter, ParameterExpression resolutionInfoParameter)
        {
            var length = resolutionTargets.Length;
            var arguments = new Expression[length];

            for (var i = 0; i < length; i++)
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