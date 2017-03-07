using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions
{
    internal class ExpressionBuilder : IExpressionBuilder
    {
        private readonly MethodInfo buildExtensionMethod;

        public ExpressionBuilder()
        {
            this.buildExtensionMethod = typeof(IContainerExtensionManager).GetSingleMethod("ExecutePostBuildExtensions");
        }
        
        public Expression CreateFillExpression(IContainerExtensionManager extensionManager, IContainerContext containerContext, Expression instance,
            ResolutionInfo resolutionInfo, TypeInformation typeInfo, InjectionParameter[] parameters, ResolutionMember[] members, ResolutionMethod[] methods)
        {
            var block = new List<Expression>();

            if (instance.Type != typeInfo.Type)
                instance = Expression.Convert(instance, typeInfo.Type);

            var variable = Expression.Variable(typeInfo.Type);
            var assingExpr = Expression.Assign(variable, instance);

            block.Add(assingExpr);

            if (members != null && members.Length > 0)
                block.AddRange(FillMembersExpression(members, variable));

            if (methods != null && methods.Length > 0 || extensionManager.HasPostBuildExtensions)
                return CreatePostWorkExpressionIfAny(extensionManager, containerContext, resolutionInfo, variable, typeInfo, parameters, methods, block, variable);

            block.Add(variable); //return

            return Expression.Block(new[] { variable }, block);
        }

        public Expression CreateExpression(IContainerExtensionManager extensionManager, IContainerContext containerContext, 
            ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo,
            TypeInformation typeInfo, InjectionParameter[] parameters, ResolutionMember[] members, ResolutionMethod[] methods)
        {
            Expression initExpression = Expression.New(resolutionConstructor.Constructor, resolutionConstructor.Parameters);

            if (members != null && members.Length > 0)
                initExpression = CreateMemberInitExpression(members, (NewExpression)initExpression);

            if ((methods != null && methods.Length > 0) || extensionManager.HasPostBuildExtensions)
                return CreatePostWorkExpressionIfAny(extensionManager, containerContext, resolutionInfo, initExpression, typeInfo, parameters, methods);

            return initExpression;
        }
        
        private Expression CreatePostWorkExpressionIfAny(IContainerExtensionManager extensionManager, IContainerContext containerContext, ResolutionInfo resolutionInfo,
            Expression initExpression, TypeInformation typeInfo, InjectionParameter[] parameters, ResolutionMethod[] methods, 
            List<Expression> block = null, ParameterExpression variable = null)
        {
            block = block ?? new List<Expression>();

            var newVariable = variable ?? Expression.Variable(typeInfo.Type);
            if (variable == null)
            {
                var assingExpr = Expression.Assign(newVariable, initExpression);
                block.Add(assingExpr);
            }

            if (methods != null && methods.Length > 0)
                block.AddRange(CreateMethodExpressions(methods, newVariable));

            if (extensionManager.HasPostBuildExtensions)
            {
                var call = Expression.Call(Expression.Constant(extensionManager), buildExtensionMethod, newVariable, Expression.Constant(containerContext),
                      Expression.Constant(resolutionInfo), Expression.Constant(typeInfo), Expression.Constant(parameters, typeof(InjectionParameter[])));

                block.Add(Expression.Assign(newVariable, Expression.Convert(call, typeInfo.Type)));
            }

            block.Add(newVariable); //return

            return Expression.Block(new[] { newVariable }, block);
        }

        private Expression[] FillMembersExpression(ResolutionMember[] members, Expression instance)
        {
            var propLength = members.Length;
            var expressions = new Expression[propLength];
            for (var i = 0; i < propLength; i++)
            {
                var member = members[i];

                if (member.MemberInfo is PropertyInfo prop)
                {
                    var propExpression = Expression.Property(instance, prop);
                    expressions[i] = Expression.Assign(propExpression, member.Expression);
                }
                else
                {
                    var field = member.MemberInfo as FieldInfo;
                    var propExpression = Expression.Field(instance, field);
                    expressions[i] = Expression.Assign(propExpression, member.Expression);
                }
            }

            return expressions;
        }

        private Expression CreateMemberInitExpression(ResolutionMember[] members, NewExpression newExpression)
        {
            var propLength = members.Length;
            var propertyExpressions = new MemberBinding[propLength];
            for (var i = 0; i < propLength; i++)
            {
                var member = members[i];
                var propertyExpression = Expression.Bind(member.MemberInfo, member.Expression);
                propertyExpressions[i] = propertyExpression;
            }

            return Expression.MemberInit(newExpression, propertyExpressions);
        }

        private Expression[] CreateMethodExpressions(ResolutionMethod[] methods, Expression newExpression)
        {
            var lenght = methods.Length;
            newExpression = Expression.Convert(newExpression, methods[0].Method.DeclaringType);
            var methodExpressions = new Expression[lenght];
            for (var i = 0; i < lenght; i++)
            {
                var method = methods[i];
                methodExpressions[i] = Expression.Call(newExpression, method.Method, method.Parameters);
            }

            return methodExpressions;
        }
    }
}
