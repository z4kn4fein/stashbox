using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Infrastructure.Registration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Stashbox.BuildUp.Expressions
{
    internal class ExpressionBuilder : IExpressionBuilder
    {
        private readonly IContainerExtensionManager containerExtensionManager;

        public ExpressionBuilder(IContainerExtensionManager containerExtensionManager)
        {
            this.containerExtensionManager = containerExtensionManager;
        }

        public Expression CreateFillExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, Expression instance,
            ResolutionContext resolutionContext, Type serviceType)
        {
            var block = new List<Expression>();

            if (instance.Type != serviceType)
                instance = Expression.Convert(instance, serviceType);

            var variable = Expression.Variable(serviceType);
            var assign = Expression.Assign(variable, instance);

            block.Add(assign);

            if (serviceRegistration.MetaInformation.InjectionMembers.Length > 0)
                block.AddRange(this.FillMembersExpression(containerContext, serviceRegistration, resolutionContext, variable));

            if (serviceRegistration.MetaInformation.InjectionMethods.Length > 0 || serviceRegistration.RegistrationContext.Initializer != null || this.containerExtensionManager.HasPostBuildExtensions)
                block.AddRange(this.CreatePostWorkExpressionIfAny(containerContext, serviceRegistration, resolutionContext, serviceType, variable));

            block.Add(variable); //return

            return Expression.Block(new[] { variable }, block);
        }

        public Expression CreateExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type serviceType)
        {
            var initExpression = this.CreateInitExpression(containerContext, serviceRegistration, resolutionContext);
            if (initExpression == null)
                return null;

            if (serviceRegistration.MetaInformation.InjectionMembers.Length > 0)
            {
                var bindings = this.GetMemberBindings(containerContext, serviceRegistration, resolutionContext);
                if (bindings.Count > 0)
                    initExpression = Expression.MemberInit((NewExpression)initExpression,
                        this.GetMemberBindings(containerContext, serviceRegistration, resolutionContext));
            }

            if (serviceRegistration.MetaInformation.InjectionMethods.Length > 0 || serviceRegistration.RegistrationContext.Initializer != null || this.containerExtensionManager.HasPostBuildExtensions)
            {
                var variable = Expression.Variable(initExpression.Type);
                var assign = Expression.Assign(variable, initExpression);

                var expressions = this.CreatePostWorkExpressionIfAny(containerContext, serviceRegistration, resolutionContext, serviceType, variable);

                var block = new List<Expression>(expressions.Length + 1) { assign };
                block.AddRange(expressions);
                block.Add(variable);

                return Expression.Block(new[] { variable }, block);
            }

            return initExpression;
        }

        private Expression CreateInitExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            if (serviceRegistration.RegistrationContext.SelectedConstructor != null)
            {
                if (serviceRegistration.RegistrationContext.ConstructorArguments != null)
                    return Expression.New(serviceRegistration.RegistrationContext.SelectedConstructor, serviceRegistration.RegistrationContext.ConstructorArguments.Select(Expression.Constant));

                var resolutionConstructor = this.CreateResolutionConstructor(containerContext, serviceRegistration, resolutionContext, serviceRegistration.RegistrationContext.SelectedConstructor);
                return Expression.New(resolutionConstructor.Constructor, resolutionConstructor.Parameters);
            }

            var rule = serviceRegistration.RegistrationContext.ConstructorSelectionRule ?? containerContext.ContainerConfigurator.ContainerConfiguration.ConstructorSelectionRule;
            var constructors = rule(serviceRegistration.MetaInformation.Constructors).ToArray();

            var constructor = this.SelectConstructor(containerContext, serviceRegistration, resolutionContext, constructors);
            return constructor == null ? null : Expression.New(constructor.Constructor, constructor.Parameters);
        }

        private ResolutionConstructor CreateResolutionConstructor(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            var paramLength = parameters.Length;
            var parameterExpressions = new Expression[paramLength];

            for (var i = 0; i < paramLength; i++)
            {
                var parameter = parameters[i];

                var expression = containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext,
                    resolutionContext, serviceRegistration.MetaInformation.GetTypeInformationForParameter(parameter),
                    serviceRegistration.RegistrationContext.InjectionParameters);

                parameterExpressions[i] = expression ?? throw new ResolutionFailedException(serviceRegistration.ImplementationType,
                    $"Constructor {constructor}, unresolvable parameter: ({parameter.ParameterType}){parameter.Name}");
            }

            return new ResolutionConstructor { Constructor = constructor, Parameters = parameterExpressions };
        }

        private ResolutionConstructor SelectConstructor(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, ConstructorInformation[] constructors)
        {
            var length = constructors.Length;
            var checkedConstructors = new Dictionary<ConstructorInfo, TypeInformation>();
            for (var i = 0; i < length; i++)
            {
                var constructor = constructors[i];
                var paramLength = constructor.Parameters.Length;
                var parameterExpressions = new Expression[paramLength];

                var hasNullParameter = false;
                TypeInformation failedParameter = null;
                for (var j = 0; j < paramLength; j++)
                {
                    var parameter = constructor.Parameters[j];

                    var expression = containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext,
                        resolutionContext, parameter, serviceRegistration.RegistrationContext.InjectionParameters);

                    if (expression == null)
                    {
                        hasNullParameter = true;
                        failedParameter = parameter;
                        break;
                    }

                    parameterExpressions[j] = expression;
                }

                if (hasNullParameter)
                {
                    if (!resolutionContext.NullResultAllowed)
                        checkedConstructors.Add(constructor.Constructor, failedParameter);

                    continue;
                }

                return new ResolutionConstructor { Constructor = constructor.Constructor, Parameters = parameterExpressions };
            }

            if (resolutionContext.NullResultAllowed)
                return null;

            var stringBuilder = new StringBuilder();
            foreach (var checkedConstructor in checkedConstructors)
                stringBuilder.AppendLine($"Checked constructor {checkedConstructor.Key}, unresolvable parameter: ({checkedConstructor.Value.Type}){checkedConstructor.Value.ParameterName}");

            throw new ResolutionFailedException(serviceRegistration.ImplementationType, stringBuilder.ToString());
        }

        private Expression[] CreatePostWorkExpressionIfAny(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type serviceType, Expression instance)
        {
            var length = serviceRegistration.MetaInformation.InjectionMethods.Length;

            if (this.containerExtensionManager.HasPostBuildExtensions)
                length++;

            if (serviceRegistration.RegistrationContext.Initializer != null)
                length++;

            var expressions = new Expression[length];

            if (length > 0)
                this.CreateMethodExpressions(containerContext, serviceRegistration, resolutionContext, instance, expressions);

            if (serviceRegistration.RegistrationContext.Initializer != null)
                expressions[expressions.Length - (this.containerExtensionManager.HasPostBuildExtensions ? 2 : 1)] = Expression.Call(Expression.Constant(serviceRegistration.RegistrationContext.Initializer),
                    serviceRegistration.RegistrationContext.Initializer.GetType().GetSingleMethod("Invoke"), instance, Expression.Convert(resolutionContext.CurrentScopeParameter, Constants.ResolverType));

            if (this.containerExtensionManager.HasPostBuildExtensions)
            {
                var call = Expression.Call(Expression.Constant(this.containerExtensionManager), Constants.BuildExtensionMethod, instance, Expression.Constant(containerContext),
                      Expression.Constant(resolutionContext), Expression.Constant(serviceRegistration), Expression.Constant(serviceType));

                expressions[expressions.Length - 1] = Expression.Assign(instance, Expression.Convert(call, instance.Type));
            }

            return expressions;
        }

        private IEnumerable<Expression> FillMembersExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Expression instance)
        {
            var length = serviceRegistration.MetaInformation.InjectionMembers.Length;

            var expressions = new List<Expression>();

            for (var i = 0; i < length; i++)
            {
                var member = serviceRegistration.MetaInformation.InjectionMembers[i];

                if (!serviceRegistration.CanInjectMember(member)) continue;

                var expression = containerContext.ResolutionStrategy
                    .BuildResolutionExpression(containerContext, resolutionContext, member.TypeInformation, serviceRegistration.RegistrationContext.InjectionParameters);

                if (expression == null) continue;

                if (member.MemberInfo is PropertyInfo prop)
                {
                    var propExpression = Expression.Property(instance, prop);
                    expressions.Add(Expression.Assign(propExpression, expression));
                }
                else
                {
                    var fieldExpression = Expression.Field(instance, member.MemberInfo as FieldInfo);
                    expressions.Add(Expression.Assign(fieldExpression, expression));
                }
            }

            return expressions;
        }

        private void CreateMethodExpressions(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Expression newExpression, Expression[] buffer)
        {
            var length = serviceRegistration.MetaInformation.InjectionMethods.Length;
            for (var i = 0; i < length; i++)
            {
                var info = serviceRegistration.MetaInformation.InjectionMethods[i];

                var paramLength = info.Parameters.Length;
                if (paramLength == 0)
                    buffer[i] = Expression.Call(newExpression, info.Method, Constants.EmptyExpressions);
                else
                {
                    var parameters = new Expression[paramLength];
                    for (var j = 0; j < paramLength; j++)
                        parameters[j] = containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext,
                            info.Parameters[j], serviceRegistration.RegistrationContext.InjectionParameters);

                    buffer[i] = Expression.Call(newExpression, info.Method, parameters);
                }
            }
        }

        private IList<MemberBinding> GetMemberBindings(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            var length = serviceRegistration.MetaInformation.InjectionMembers.Length;
            var members = new List<MemberBinding>();

            for (var i = 0; i < length; i++)
            {
                var info = serviceRegistration.MetaInformation.InjectionMembers[i];
                if (!serviceRegistration.CanInjectMember(info)) continue;

                var expression = containerContext.ResolutionStrategy
                    .BuildResolutionExpression(containerContext, resolutionContext, info.TypeInformation, serviceRegistration.RegistrationContext.InjectionParameters);

                if (expression == null) continue;

                members.Add(Expression.Bind(info.MemberInfo, expression));
            }

            return members;
        }
    }
}
