using Stashbox.ContainerExtension;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
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
                instance = instance.ConvertTo(serviceType);

            var variable = serviceType.AsVariable();
            var assign = variable.AssignTo(instance);

            block.Add(assign);

            if (serviceRegistration.InjectionMembers.Length > 0)
                block.AddRange(this.FillMembersExpression(containerContext, serviceRegistration, resolutionContext, variable));

            if (serviceRegistration.InjectionMethods.Length > 0 || serviceRegistration.RegistrationContext.Initializer != null || this.containerExtensionManager.HasPostBuildExtensions)
                block.AddRange(this.CreatePostWorkExpressionIfAny(containerContext, serviceRegistration, resolutionContext, serviceType, variable));

            block.Add(variable); //block returns with the variable

            return block.AsBlock(variable);
        }

        public Expression CreateExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type serviceType)
        {
            var initExpression = this.CreateInitExpression(containerContext, serviceRegistration, resolutionContext);
            if (initExpression == null)
                return null;

            if (serviceRegistration.InjectionMembers.Length > 0)
            {
                var bindings = this.GetMemberBindings(containerContext, serviceRegistration, resolutionContext);
                if (bindings.Count > 0)
                    initExpression = initExpression.InitMembers(this.GetMemberBindings(containerContext, serviceRegistration, resolutionContext));
            }

            if (serviceRegistration.InjectionMethods.Length > 0 || serviceRegistration.RegistrationContext.Initializer != null || this.containerExtensionManager.HasPostBuildExtensions)
            {
                var variable = initExpression.Type.AsVariable();
                var assign = variable.AssignTo(initExpression);

                var expressions = this.CreatePostWorkExpressionIfAny(containerContext, serviceRegistration, resolutionContext, serviceType, variable);

                var block = new List<Expression>(expressions.Length + 1) { assign };
                block.AddRange(expressions);
                block.Add(variable);

                return block.AsBlock(variable);
            }

            return initExpression;
        }

        private Expression CreateInitExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            if (serviceRegistration.SelectedConstructor != null)
            {
                if (serviceRegistration.RegistrationContext.ConstructorArguments != null)
                    return serviceRegistration.SelectedConstructor.Constructor
                        .MakeNew(serviceRegistration.RegistrationContext.ConstructorArguments.Select(Expression.Constant));

                return this.CreateResolutionConstructor(containerContext, serviceRegistration, resolutionContext, serviceRegistration.SelectedConstructor)
                    .MakeNew();
            }

            var rule = serviceRegistration.RegistrationContext.ConstructorSelectionRule ?? containerContext.ContainerConfigurator.ContainerConfiguration.ConstructorSelectionRule;
            var constructors = rule(serviceRegistration.Constructors).ToArray();

            return this.SelectConstructor(containerContext, serviceRegistration, resolutionContext, constructors)?.MakeNew();
        }

        private ResolutionConstructor CreateResolutionConstructor(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, ConstructorInformation constructor)
        {
            var paramLength = constructor.Parameters.Length;
            var parameterExpressions = new Expression[paramLength];

            for (var i = 0; i < paramLength; i++)
            {
                var parameter = constructor.Parameters[i];

                var expression = containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext,
                    resolutionContext, parameter, serviceRegistration.RegistrationContext.InjectionParameters);

                parameterExpressions[i] = expression ?? throw new ResolutionFailedException(serviceRegistration.ImplementationType,
                    $"Constructor {constructor.Constructor}, unresolvable parameter: ({parameter.Type}){parameter.ParameterName}");
            }

            return new ResolutionConstructor { Constructor = constructor.Constructor, Parameters = parameterExpressions };
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
            var length = serviceRegistration.InjectionMethods.Length;

            if (this.containerExtensionManager.HasPostBuildExtensions)
                length++;

            if (serviceRegistration.RegistrationContext.Initializer != null)
                length++;

            var expressions = new Expression[length];

            if (length > 0)
                this.CreateMethodExpressions(containerContext, serviceRegistration, resolutionContext, instance, expressions);

            if (serviceRegistration.RegistrationContext.Initializer != null)
                expressions[expressions.Length - (this.containerExtensionManager.HasPostBuildExtensions ? 2 : 1)] =
                    serviceRegistration.RegistrationContext.Initializer.AsConstant()
                        .CallMethod(serviceRegistration.RegistrationContext.Initializer.GetType().GetSingleMethod("Invoke"),
                            instance, resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType));

            if (this.containerExtensionManager.HasPostBuildExtensions)
            {
                var call = this.containerExtensionManager.AsConstant()
                    .CallMethod(Constants.BuildExtensionMethod,
                        instance,
                        containerContext.AsConstant(),
                        resolutionContext.AsConstant(),
                        serviceRegistration.AsConstant(),
                        serviceType.AsConstant());

                expressions[expressions.Length - 1] = instance.AssignTo(call.ConvertTo(instance.Type));
            }

            return expressions;
        }

        private IEnumerable<Expression> FillMembersExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Expression instance)
        {
            var length = serviceRegistration.InjectionMembers.Length;

            var expressions = new List<Expression>();

            for (var i = 0; i < length; i++)
            {
                var member = serviceRegistration.InjectionMembers[i];

                if (!serviceRegistration.CanInjectMember(member)) continue;

                var expression = containerContext.ResolutionStrategy
                    .BuildResolutionExpression(containerContext, resolutionContext, member.TypeInformation, serviceRegistration.RegistrationContext.InjectionParameters);

                if (expression == null) continue;

                expressions.Add(instance.Member(member.MemberInfo).AssignTo(expression));
            }

            return expressions;
        }

        private void CreateMethodExpressions(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Expression newExpression, Expression[] buffer)
        {
            var length = serviceRegistration.InjectionMethods.Length;
            for (var i = 0; i < length; i++)
            {
                var info = serviceRegistration.InjectionMethods[i];

                var paramLength = info.Parameters.Length;
                if (paramLength == 0)
                    buffer[i] = newExpression.CallMethod(info.Method);
                else
                {
                    var parameters = new Expression[paramLength];
                    for (var j = 0; j < paramLength; j++)
                        parameters[j] = containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext, resolutionContext,
                            info.Parameters[j], serviceRegistration.RegistrationContext.InjectionParameters);

                    buffer[i] = newExpression.CallMethod(info.Method, parameters);
                }
            }
        }

        private IList<MemberBinding> GetMemberBindings(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            var length = serviceRegistration.InjectionMembers.Length;
            var members = new List<MemberBinding>();

            for (var i = 0; i < length; i++)
            {
                var info = serviceRegistration.InjectionMembers[i];
                if (!serviceRegistration.CanInjectMember(info)) continue;

                var expression = containerContext.ResolutionStrategy
                    .BuildResolutionExpression(containerContext, resolutionContext, info.TypeInformation, serviceRegistration.RegistrationContext.InjectionParameters);

                if (expression == null) continue;

                members.Add(info.MemberInfo.AssignTo(expression));
            }

            return members;
        }
    }
}
