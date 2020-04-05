using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions
{
    internal class ExpressionBuilder : IExpressionBuilder
    {
        private readonly IMethodExpressionBuilder methodExpressionBuilder;
        private readonly IMemberExpressionBuilder memberExpressionBuilder;

        public ExpressionBuilder(IMethodExpressionBuilder methodExpressionBuilder,
            IMemberExpressionBuilder memberExpressionBuilder)
        {
            this.methodExpressionBuilder = methodExpressionBuilder;
            this.memberExpressionBuilder = memberExpressionBuilder;
        }

        public Expression ConstructBuildUpExpression(
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType)
        {
            if (instance.Type != serviceRegistration.ImplementationType)
                instance = instance.ConvertTo(serviceRegistration.ImplementationType);

            var methods = serviceRegistration.ImplementationTypeInfo.GetUsableMethods();
            var members = serviceRegistration.ImplementationTypeInfo.GetUsableMembers(serviceRegistration.RegistrationContext, containerContext.ContainerConfiguration);

            if (members.Length > 0 || methods.Length > 0 || serviceRegistration.RegistrationContext.Initializer != null)
            {
                var lines = new List<Expression>();
                var variable = instance.Type.AsVariable();
                var assign = variable.AssignTo(instance);

                lines.Add(assign);

                lines.AddRange(this.memberExpressionBuilder.GetMemberExpressions(containerContext, members,
                    serviceRegistration.RegistrationContext, resolutionContext, variable));

                lines.AddRange(this.methodExpressionBuilder.CreateMethodExpressions(containerContext, methods,
                     serviceRegistration.RegistrationContext, resolutionContext, variable));

                if (serviceRegistration.RegistrationContext.Initializer != null)
                    lines.Add(serviceRegistration.RegistrationContext.Initializer.AsConstant()
                        .CallMethod(serviceRegistration.RegistrationContext.Initializer.GetType().GetSingleMethod("Invoke"),
                            variable, resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType)));

                lines.Add(variable.Type != serviceType ? variable.ConvertTo(serviceType) : variable); //block returns with the variable

                return lines.AsBlock(variable);
            }

            return instance;
        }

        public Expression ConstructBuildUpExpression(
            IContainerContext containerContext,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType)
        {

            var typeInfo = serviceType.GetTypeInfo();
            var methods = typeInfo.GetUsableMethods();
            var members = typeInfo.GetUsableMembers(registrationContext, containerContext.ContainerConfiguration);

            if (methods.Length > 0 || members.Length > 0)
            {
                var lines = new List<Expression>();

                if (instance.Type != serviceType)
                    instance = instance.ConvertTo(serviceType);

                var variable = serviceType.AsVariable();
                var assign = variable.AssignTo(instance);

                lines.Add(assign);
                lines.AddRange(this.memberExpressionBuilder.GetMemberExpressions(containerContext, members,
                    registrationContext, resolutionContext, variable));
                lines.AddRange(this.methodExpressionBuilder.CreateMethodExpressions(containerContext, methods,
                    registrationContext, resolutionContext, instance));

                lines.Add(variable); //block returns with the variable

                return lines.AsBlock(variable);
            }

            return instance;
        }

        public Expression ConstructExpression (
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            Type serviceType)
        {
            var constructors = serviceRegistration.ImplementationTypeInfo.GetUsableConstructors();
            var methods = serviceRegistration.ImplementationTypeInfo.GetUsableMethods();
            var members = serviceRegistration.ImplementationTypeInfo.GetUsableMembers(serviceRegistration.RegistrationContext, containerContext.ContainerConfiguration);

            var initExpression = this.CreateInitExpression(containerContext, serviceRegistration.RegistrationContext, resolutionContext, constructors);
            if (initExpression == null)
                return null;

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(this.memberExpressionBuilder.GetMemberBindings(containerContext, members,
                    serviceRegistration.RegistrationContext, resolutionContext, serviceRegistration.ImplementationType));

            if (methods.Length > 0 || serviceRegistration.RegistrationContext.Initializer != null)
            {
                var variable = initExpression.Type.AsVariable();
                var assign = variable.AssignTo(initExpression);

                var lines = new List<Expression> { assign };

                lines.AddRange(this.methodExpressionBuilder.CreateMethodExpressions(containerContext, methods,
                     serviceRegistration.RegistrationContext, resolutionContext, variable));

                if (serviceRegistration.RegistrationContext.Initializer != null)
                    lines.Add(serviceRegistration.RegistrationContext.Initializer.AsConstant()
                        .CallMethod(serviceRegistration.RegistrationContext.Initializer.GetType().GetSingleMethod("Invoke"),
                            variable, resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType)));

                lines.Add(variable);

                return lines.AsBlock(variable);
            }

            return initExpression;
        }

        public Expression ConstructExpression(
            IContainerContext containerContext,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Type serviceType)
        {
            var typeInfo = serviceType.GetTypeInfo();
            var methods = typeInfo.GetUsableMethods();
            var members = typeInfo.GetUsableMembers(registrationContext, containerContext.ContainerConfiguration);

            var initExpression = (Expression)this.methodExpressionBuilder.SelectConstructor(
                containerContext,
                registrationContext,
                resolutionContext,
                typeInfo.DeclaredConstructors.CastToArray(),
                out var parameters)?.MakeNew(parameters);

            if (initExpression == null)
                return null;

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(this.memberExpressionBuilder.GetMemberBindings(containerContext, members,
                    registrationContext, resolutionContext, serviceType));

            if (methods.Length > 0)
            {
                var variable = initExpression.Type.AsVariable();
                var assign = variable.AssignTo(initExpression);

                var lines = new List<Expression> { assign };
                lines.AddRange(this.methodExpressionBuilder.CreateMethodExpressions(containerContext, methods,
                    registrationContext, resolutionContext, variable));
                lines.Add(variable);

                return lines.AsBlock(variable);
            }

            return initExpression;
        }

        private Expression CreateInitExpression(
            IContainerContext containerContext,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            ConstructorInfo[] constructors)
        {
            if (registrationContext.SelectedConstructor != null)
            {
                if (registrationContext.ConstructorArguments != null)
                    return registrationContext.SelectedConstructor
                        .MakeNew(registrationContext.ConstructorArguments.Select(Expression.Constant));

                return registrationContext.SelectedConstructor.MakeNew(
                    this.methodExpressionBuilder.CreateParameterExpressionsForMethod(
                        containerContext,
                        registrationContext,
                        resolutionContext, registrationContext.SelectedConstructor));
            }

            var rule = registrationContext.ConstructorSelectionRule ??
                containerContext.ContainerConfiguration.ConstructorSelectionRule;
            constructors = rule(constructors).ToArray();

            return this.methodExpressionBuilder.SelectConstructor(
                containerContext, registrationContext, resolutionContext, constructors, out var parameters)?.MakeNew(parameters);
        }
    }
}
