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
        private readonly IContainerContext containerContext;
        private readonly IContainerExtensionManager containerExtensionManager;

        public ExpressionBuilder(IContainerContext containerContext, IContainerExtensionManager containerExtensionManager)
        {
            this.containerContext = containerContext;
            this.containerExtensionManager = containerExtensionManager;
        }

        public Expression CreateFillExpression(Expression instance, ResolutionInfo resolutionInfo, Type serviceType, 
            InjectionParameter[] parameters, ResolutionMember[] members, ResolutionMethod[] methods)
        {
            var block = new List<Expression>();

            if (instance.Type != serviceType)
                instance = Expression.Convert(instance, serviceType);

            var variable = Expression.Variable(serviceType);
            var assign = Expression.Assign(variable, instance);

            block.Add(assign);

            if (members != null && members.Length > 0)
                block.AddRange(FillMembersExpression(members, variable));

            if (methods != null && methods.Length > 0 || this.containerExtensionManager.HasPostBuildExtensions)
                return CreatePostWorkExpressionIfAny(resolutionInfo, variable, serviceType, parameters, methods, block, variable);

            block.Add(variable); //return

            return Expression.Block(new[] { variable }, block);
        }

        public Expression CreateExpression(ResolutionConstructor resolutionConstructor, ResolutionInfo resolutionInfo,
            Type serviceType, InjectionParameter[] parameters, ResolutionMember[] members, ResolutionMethod[] methods)
        {
            Expression initExpression = Expression.New(resolutionConstructor.Constructor, resolutionConstructor.Parameters);

            if (members != null && members.Length > 0)
                initExpression = CreateMemberInitExpression(members, (NewExpression)initExpression);

            if (methods != null && methods.Length > 0 || this.containerExtensionManager.HasPostBuildExtensions)
                return CreatePostWorkExpressionIfAny(resolutionInfo, initExpression, serviceType, parameters, methods);

            return initExpression;
        }
        
        private Expression CreatePostWorkExpressionIfAny(ResolutionInfo resolutionInfo,
            Expression initExpression, Type serviceType, InjectionParameter[] parameters, ResolutionMethod[] methods, 
            List<Expression> block = null, ParameterExpression variable = null)
        {
            block = block ?? new List<Expression>();

            var newVariable = variable ?? Expression.Variable(serviceType);
            if (variable == null)
            {
                var assign = Expression.Assign(newVariable, initExpression);
                block.Add(assign);
            }

            if (methods != null && methods.Length > 0)
                block.AddRange(CreateMethodExpressions(methods, newVariable));

            if (this.containerExtensionManager.HasPostBuildExtensions)
            {
                var call = Expression.Call(Expression.Constant(this.containerExtensionManager), Constants.BuildExtensionMethod, newVariable, Expression.Constant(containerContext),
                      Expression.Constant(resolutionInfo), Expression.Constant(serviceType), Expression.Constant(parameters, typeof(InjectionParameter[])));

                block.Add(Expression.Assign(newVariable, Expression.Convert(call, serviceType)));
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
