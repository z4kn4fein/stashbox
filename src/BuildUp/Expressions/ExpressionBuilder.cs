using Stashbox.ContainerExtension;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Expressions
{
    internal class ExpressionBuilder : IExpressionBuilder
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IConstructorSelector constructorSelector;
        private readonly IResolutionStrategy resolutionStrategy;

        public ExpressionBuilder(IContainerExtensionManager containerExtensionManager,
            IConstructorSelector constructorSelector,
            IResolutionStrategy resolutionStrategy)
        {
            this.containerExtensionManager = containerExtensionManager;
            this.constructorSelector = constructorSelector;
            this.resolutionStrategy = resolutionStrategy;
        }

        public Expression CreateFillExpression(
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            Expression instance,
            ResolutionContext resolutionContext,
            Type serviceType)
        {
            var lines = new List<Expression>();

            if (instance.Type != serviceRegistration.ImplementationType)
                instance = instance.ConvertTo(serviceRegistration.ImplementationType);

            var variable = instance.Type.AsVariable();
            var assign = variable.AssignTo(instance);

            lines.Add(assign);

            if (serviceRegistration.InjectionMembers.Length > 0)
                lines.AddRange(this.FillMembersExpression(containerContext, serviceRegistration.InjectionMembers,
                    serviceRegistration.RegistrationContext, resolutionContext, variable));

            if (serviceRegistration.InjectionMethods.Length > 0 || serviceRegistration.RegistrationContext.Initializer != null || this.containerExtensionManager.HasPostBuildExtensions)
                lines.AddRange(this.CreatePostWorkExpressionIfAny(containerContext, serviceRegistration, resolutionContext, serviceType, variable));

            lines.Add(variable.Type != serviceType ? variable.ConvertTo(serviceType) : variable); //block returns with the variable

            return lines.AsBlock(variable);
        }

        public Expression CreateBasicFillExpression(
            IContainerContext containerContext,
            MemberInformation[] injectionMembers,
            MethodInformation[] injectionMethods,
            Expression instance,
            ResolutionContext resolutionContext,
            Type requestedType)
        {
            var lines = new List<Expression>();

            if (instance.Type != requestedType)
                instance = instance.ConvertTo(requestedType);

            var variable = requestedType.AsVariable();
            var assign = variable.AssignTo(instance);

            lines.Add(assign);

            if (injectionMembers.Length > 0)
                lines.AddRange(this.FillMembersExpression(containerContext, injectionMembers,
                    RegistrationContext.Empty, resolutionContext, variable));

            if (injectionMethods.Length > 0)
            {
                var methodExpressions = new Expression[injectionMethods.Length];
                this.CreateMethodExpressions(containerContext, injectionMethods,
                     RegistrationContext.Empty, resolutionContext, instance, methodExpressions);
                lines.AddRange(methodExpressions);
            }

            lines.Add(variable); //block returns with the variable

            return lines.AsBlock(variable);
        }

        public Expression CreateExpression(
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            Type serviceType)
        {
            var initExpression = this.CreateInitExpression(containerContext, serviceRegistration, resolutionContext);
            if (initExpression == null)
                return null;

            if (serviceRegistration.InjectionMembers.Length > 0)
            {
                var bindings = this.GetMemberBindings(containerContext, serviceRegistration.InjectionMembers,
                    serviceRegistration.RegistrationContext, resolutionContext);
                if (bindings.Count > 0)
                    initExpression = initExpression.InitMembers(bindings);
            }

            if (serviceRegistration.InjectionMethods.Length > 0 ||
                serviceRegistration.RegistrationContext.Initializer != null ||
                this.containerExtensionManager.HasPostBuildExtensions)
            {
                var variable = initExpression.Type.AsVariable();
                var assign = variable.AssignTo(initExpression);

                var expressions = this.CreatePostWorkExpressionIfAny(containerContext, serviceRegistration,
                    resolutionContext, serviceType, variable);

                var block = new List<Expression>(expressions.Length + 1) { assign };
                block.AddRange(expressions);
                block.Add(variable);

                return block.AsBlock(variable);
            }

            return initExpression;
        }

        public Expression CreateBasicExpression(
            IContainerContext containerContext,
            ConstructorInformation[] constructors,
            MemberInformation[] injectionMembers,
            MethodInformation[] injectionMethods,
            ResolutionContext resolutionContext,
            Type requestedType)
        {
            var initExpression = (Expression)this.constructorSelector.SelectConstructor(requestedType,
                containerContext, resolutionContext, constructors, null)?.MakeNew();
            if (initExpression == null)
                return null;

            if (injectionMembers.Length > 0)
            {
                var bindings = this.GetMemberBindings(containerContext, injectionMembers,
                    RegistrationContext.Empty, resolutionContext);
                if (bindings.Count > 0)
                    initExpression = initExpression.InitMembers(bindings);
            }

            if (injectionMethods.Length > 0)
            {
                var variable = initExpression.Type.AsVariable();
                var assign = variable.AssignTo(initExpression);

                var methodExpressions = new Expression[injectionMethods.Length];
                this.CreateMethodExpressions(containerContext, injectionMethods,
                     RegistrationContext.Empty, resolutionContext, variable, methodExpressions);

                var lines = new List<Expression>(methodExpressions.Length + 1) { assign };
                lines.AddRange(methodExpressions);
                lines.Add(variable);

                return lines.AsBlock(variable);
            }

            return initExpression;
        }

        private Expression CreateInitExpression(
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext)
        {
            if (serviceRegistration.SelectedConstructor != null)
            {
                if (serviceRegistration.RegistrationContext.ConstructorArguments != null)
                    return serviceRegistration.SelectedConstructor.Constructor
                        .MakeNew(serviceRegistration.RegistrationContext.ConstructorArguments.Select(Expression.Constant));

                return this.constructorSelector.CreateResolutionConstructor(containerContext, serviceRegistration,
                    resolutionContext, serviceRegistration.SelectedConstructor)
                    .MakeNew();
            }

            var rule = serviceRegistration.RegistrationContext.ConstructorSelectionRule ??
                containerContext.ContainerConfiguration.ConstructorSelectionRule;
            var constructors = rule(serviceRegistration.Constructors).ToArray();

            return this.constructorSelector.SelectConstructor(serviceRegistration.ImplementationType,
                containerContext, resolutionContext, constructors,
                serviceRegistration.RegistrationContext.InjectionParameters)?.MakeNew();
        }



        private Expression[] CreatePostWorkExpressionIfAny(
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            Type serviceType,
            Expression instance)
        {
            var length = serviceRegistration.InjectionMethods.Length;

            if (this.containerExtensionManager.HasPostBuildExtensions)
                length++;

            if (serviceRegistration.RegistrationContext.Initializer != null)
                length++;

            var expressions = new Expression[length];

            if (length > 0)
                this.CreateMethodExpressions(containerContext, serviceRegistration.InjectionMethods,
                    serviceRegistration.RegistrationContext, resolutionContext, instance, expressions);

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

        private IEnumerable<Expression> FillMembersExpression(
            IContainerContext containerContext,
            MemberInformation[] injectionMembers,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression instance)
        {
            var length = injectionMembers.Length;

            var expressions = new List<Expression>();

            for (var i = 0; i < length; i++)
            {
                var member = injectionMembers[i];
                var expression = this.GetMemberExpression(member, registrationContext,
                    containerContext, resolutionContext);

                if (expression == null) continue;

                expressions.Add(instance.Member(member.MemberInfo).AssignTo(expression));
            }

            return expressions;
        }

        private void CreateMethodExpressions(
            IContainerContext containerContext,
            MethodInformation[] injectionMethods,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression newExpression,
            Expression[] buffer)
        {
            var length = injectionMethods.Length;
            for (var i = 0; i < length; i++)
            {
                var info = injectionMethods[i];

                var paramLength = info.Parameters.Length;
                if (paramLength == 0)
                    buffer[i] = newExpression.CallMethod(info.Method);
                else
                {
                    var parameters = new Expression[paramLength];
                    for (var j = 0; j < paramLength; j++)
                        parameters[j] = this.resolutionStrategy
                            .BuildResolutionExpression(containerContext, resolutionContext,
                            info.Parameters[j], registrationContext.InjectionParameters);

                    buffer[i] = newExpression.CallMethod(info.Method, parameters);
                }
            }
        }

        private IList<MemberBinding> GetMemberBindings(
            IContainerContext containerContext,
            MemberInformation[] injectionMembers,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext)
        {
            var length = injectionMembers.Length;
            var members = new List<MemberBinding>();

            for (var i = 0; i < length; i++)
            {
                var info = injectionMembers[i];
                var expression = this.GetMemberExpression(info, registrationContext,
                    containerContext, resolutionContext);

                if (expression == null) continue;

                members.Add(info.MemberInfo.AssignTo(expression));
            }

            return members;
        }

        private Expression GetMemberExpression(MemberInformation member, RegistrationContext registrationContext,
            IContainerContext containerContext, ResolutionContext resolutionContext)
        {
            if (!member.CanInject(containerContext.ContainerConfiguration,
                    registrationContext)) return null;

            var memberExpression = this.resolutionStrategy
                .BuildResolutionExpression(containerContext, resolutionContext,
                    member.TypeInformation, registrationContext.InjectionParameters);

            if (memberExpression == null && !resolutionContext.NullResultAllowed)
                throw new ResolutionFailedException(member.TypeInformation.ParentType,
                    $"Unresolvable member: ({member.TypeInformation.Type.FullName}){member.TypeInformation.ParameterOrMemberName}.");


            if (memberExpression is ConstantExpression constant && constant.Value == null)
                return null;

            return memberExpression;
        }
    }
}
