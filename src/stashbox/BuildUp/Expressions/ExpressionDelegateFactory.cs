using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;

namespace Stashbox.BuildUp.Expressions
{
    internal delegate object CreateInstance(ResolutionInfo resolutionInfo);
    internal delegate void InvokeMethod(ResolutionInfo resolutionInfo, object instance);

    internal class ExpressionDelegateFactory
    {
        private static readonly MethodInfo evaluateMethodInfo;
        private static readonly MethodInfo buildExtensionMethod;

        static ExpressionDelegateFactory()
        {
            evaluateMethodInfo = typeof(IResolutionStrategy).GetTypeInfo().GetDeclaredMethod("EvaluateResolutionTarget");
            buildExtensionMethod = typeof(IContainerExtensionManager).GetTypeInfo().GetDeclaredMethod("ExecutePostBuildExtensions");
        }

        public static Expression CreateExpression(IContainerExtensionManager extensionManager, IContainerContext containerContext, ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo,
            Expression resolutionInfoExpression, TypeInformation typeInfo, InjectionParameter[] parameters, ResolutionMember[] members, ResolutionMethod[] methods)
        {
            var length = resolutionConstructor.Parameters.Length;
            var arguments = new Expression[length];

            for (var i = 0; i < length; i++)
            {
                var parameter = resolutionConstructor.Parameters[i];
                arguments[i] = containerContext.ResolutionStrategy.GetExpressionForResolutionTarget(parameter, resolutionInfo, resolutionInfoExpression);
            }

            Expression initExpression = Expression.New(resolutionConstructor.Constructor, arguments);

            if (members != null && members.Length > 0)
                initExpression = CreateMemberInitExpression(containerContext, members, resolutionInfo, resolutionInfoExpression, (NewExpression)initExpression);

            if ((methods != null && methods.Length > 0) || extensionManager.HasPostBuildExtensions)
                return CreatePostWorkExpressionIfAny(extensionManager, containerContext, resolutionInfo, resolutionInfoExpression, initExpression, typeInfo, parameters, methods);
            return initExpression;
        }

        private static Expression CreatePostWorkExpressionIfAny(IContainerExtensionManager extensionManager, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            Expression resolutionInfoExpression, Expression initExpression, TypeInformation typeInfo, InjectionParameter[] parameters, ResolutionMethod[] methods)
        {
            var block = new List<Expression>();

            var variable = Expression.Variable(typeInfo.Type);
            var assingExpr = Expression.Assign(variable, initExpression);

            block.Add(assingExpr);

            if (methods != null && methods.Length > 0)
                block.AddRange(CreateMethodExpressions(containerContext, methods, resolutionInfo, resolutionInfoExpression, variable));

            if (extensionManager.HasPostBuildExtensions)
            {
                var call = Expression.Call(Expression.Constant(extensionManager), buildExtensionMethod, variable, Expression.Constant(containerContext),
                      resolutionInfoExpression, Expression.Constant(typeInfo), Expression.Constant(parameters, typeof(InjectionParameter[])));

                block.Add(Expression.Assign(variable, Expression.Convert(call, typeInfo.Type)));
            }

            block.Add(variable); //return

            return Expression.Block(new[] { variable }, block);
        }

        private static Expression CreateMemberInitExpression(IContainerContext containerContext, ResolutionMember[] members, ResolutionInfo resolutionInfo,
            Expression resolutionInfoExpression, NewExpression newExpression)
        {
            var propLength = members.Length;
            var propertyExpressions = new MemberBinding[propLength];
            for (var i = 0; i < propLength; i++)
            {
                var member = members[i];
                var propertyExpression = Expression.Bind(member.MemberInfo,
                    containerContext.ResolutionStrategy.GetExpressionForResolutionTarget(member.ResolutionTarget, resolutionInfo, resolutionInfoExpression));
                propertyExpressions[i] = propertyExpression;
            }

            return Expression.MemberInit(newExpression, propertyExpressions);
        }

        private static Expression[] CreateMethodExpressions(IContainerContext containerContext, ResolutionMethod[] methods, ResolutionInfo resolutionInfo,
            Expression resolutionInfoExpression, Expression newExpression)
        {
            var lenght = methods.Length;
            newExpression = Expression.Convert(newExpression, methods[0].Method.DeclaringType);
            var methodExpressions = new Expression[lenght];
            for (var i = 0; i < lenght; i++)
            {
                var method = methods[i];
                var pLength = method.Parameters.Length;
                var arguments = new Expression[pLength];
                for (int j = 0; j < pLength; j++)
                {
                    var parameter = method.Parameters[j];
                    arguments[j] = containerContext.ResolutionStrategy.GetExpressionForResolutionTarget(parameter, resolutionInfo, resolutionInfoExpression);
                }

                methodExpressions[i] = Expression.Call(newExpression, method.Method, arguments);
            }

            return methodExpressions;
        }

        public static InvokeMethod CreateMethodExpression(IContainerContext containerContext, ResolutionTarget[] parameters, MethodInfo methodInfo)
        {
            var strategyParameter = Expression.Constant(containerContext.ResolutionStrategy, typeof(IResolutionStrategy));
            var resolutionInfoParameter = Expression.Parameter(typeof(ResolutionInfo), "resolutionInfo");
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var convertedInstance = Expression.Convert(instanceParameter, methodInfo.DeclaringType);

            var arguments = CreateExpressionFromResolutionTargets(parameters, strategyParameter, resolutionInfoParameter);

            var callExpression = Expression.Call(convertedInstance, methodInfo, arguments);
            return Expression.Lambda<InvokeMethod>(callExpression, resolutionInfoParameter, instanceParameter).Compile();
        }

        private static Expression[] CreateExpressionFromResolutionTargets(IReadOnlyList<ResolutionTarget> resolutionTargets, Expression strategyParameter,
            Expression resolutionInfoParameter)
        {
            var length = resolutionTargets.Count;
            var arguments = new Expression[length];

            for (var i = 0; i < length; i++)
            {
                var parameter = resolutionTargets[i];
                arguments[i] = CreateResolutionTargetExpression(parameter, strategyParameter, resolutionInfoParameter);
            }

            return arguments;
        }

        private static Expression CreateResolutionTargetExpression(ResolutionTarget resolutionTarget, Expression strategyParameter,
            Expression resolutionInfoParameter)
        {
            var target = Expression.Constant(resolutionTarget, typeof(ResolutionTarget));
            var evaluate = Expression.Call(strategyParameter, evaluateMethodInfo, target, resolutionInfoParameter);
            return Expression.Convert(evaluate, resolutionTarget.TypeInformation.Type);
        }
    }
}