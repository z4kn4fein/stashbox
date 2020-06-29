using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Expressions
{
    internal class ExpressionFactory
    {
        private readonly MethodExpressionFactory methodExpressionBuilder;
        private readonly MemberExpressionFactory memberExpressionBuilder;

        public ExpressionFactory(MethodExpressionFactory methodExpressionBuilder,
            MemberExpressionFactory memberExpressionBuilder)
        {
            this.methodExpressionBuilder = methodExpressionBuilder;
            this.memberExpressionBuilder = memberExpressionBuilder;
        }

        public Expression ConstructBuildUpExpression(
            ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType)
        {
            if (instance.Type != serviceRegistration.ImplementationType)
                instance = instance.ConvertTo(serviceRegistration.ImplementationType);

            var methods = serviceRegistration.ImplementationTypeInfo.GetUsableMethods();
            var members = serviceRegistration.ImplementationTypeInfo.GetUsableMembers(serviceRegistration.RegistrationContext,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            if (members.Length == 0 && methods.Length == 0 &&
                serviceRegistration.RegistrationContext.Initializer == null) return instance;

            var variable = instance.Type.AsVariable();
            var assign = variable.AssignTo(instance);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(this.memberExpressionBuilder.GetMemberExpressions(members,
                serviceRegistration.RegistrationContext, resolutionContext, variable));

            lines.AddRange(this.methodExpressionBuilder.CreateMethodExpressions(methods,
                serviceRegistration.RegistrationContext, resolutionContext, variable));

            if (serviceRegistration.RegistrationContext.Initializer != null)
                lines.Add(serviceRegistration.RegistrationContext.Initializer.AsConstant()
                    .CallMethod(serviceRegistration.RegistrationContext.Initializer.GetType().GetSingleMethod("Invoke"),
                        variable, resolutionContext.CurrentScopeParameter));

            lines.Add(variable.Type != serviceType ? variable.ConvertTo(serviceType) : variable); //block returns with the variable

            return lines.AsBlock(variable);
        }

        public Expression ConstructBuildUpExpression(
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType)
        {
            var registrationContext = RegistrationContext.Empty;
            var typeInfo = instance.Type.GetTypeInfo();

            var methods = typeInfo.GetUsableMethods();
            var members = typeInfo.GetUsableMembers(registrationContext,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            if (members.Length == 0 && methods.Length == 0) return instance;

            var variable = instance.Type.AsVariable();
            var assign = variable.AssignTo(instance);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(this.memberExpressionBuilder.GetMemberExpressions(members,
                registrationContext, resolutionContext, variable));

            lines.AddRange(this.methodExpressionBuilder.CreateMethodExpressions(methods,
                registrationContext, resolutionContext, instance));

            lines.Add(variable.Type != serviceType ? variable.ConvertTo(serviceType) : variable); //block returns with the variable

            return lines.AsBlock(variable);
        }

        public Expression ConstructExpression(
            ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext)
        {
            var constructors = serviceRegistration.ImplementationTypeInfo.GetUsableConstructors();
            var methods = serviceRegistration.ImplementationTypeInfo.GetUsableMethods();
            var members = serviceRegistration.ImplementationTypeInfo.GetUsableMembers(serviceRegistration.RegistrationContext,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var initExpression = this.CreateInitExpression(
                serviceRegistration.RegistrationContext,
                resolutionContext,
                constructors);
            if (initExpression == null)
                return null;

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(this.memberExpressionBuilder.GetMemberBindings(members,
                    serviceRegistration.RegistrationContext, resolutionContext));

            if (methods.Length == 0 && serviceRegistration.RegistrationContext.Initializer == null)
                return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(this.methodExpressionBuilder.CreateMethodExpressions(methods,
                serviceRegistration.RegistrationContext, resolutionContext, variable));

            if (serviceRegistration.RegistrationContext.Initializer != null)
                lines.Add(serviceRegistration.RegistrationContext.Initializer.AsConstant()
                    .CallMethod(serviceRegistration.RegistrationContext.Initializer.GetType().GetSingleMethod("Invoke"),
                        variable, resolutionContext.CurrentScopeParameter));

            lines.Add(variable);

            return lines.AsBlock(variable);
        }

        public Expression ConstructExpression(
            ResolutionContext resolutionContext,
            Type serviceType)
        {
            var registrationContext = RegistrationContext.Empty;
            var typeInfo = serviceType.GetTypeInfo();
            var methods = typeInfo.GetUsableMethods();
            var members = typeInfo.GetUsableMembers(registrationContext,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var initExpression = (Expression)this.methodExpressionBuilder.SelectConstructor(
                registrationContext,
                resolutionContext,
                typeInfo.DeclaredConstructors.CastToArray(),
                out var parameters).MakeNew(parameters);

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(this.memberExpressionBuilder.GetMemberBindings(members,
                    registrationContext, resolutionContext));

            if (methods.Length == 0) return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };
            lines.AddRange(this.methodExpressionBuilder.CreateMethodExpressions(methods, registrationContext, resolutionContext, variable));
            lines.Add(variable);

            return lines.AsBlock(variable);
        }

        private Expression CreateInitExpression(
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            IEnumerable<ConstructorInfo> constructors)
        {
            if (registrationContext.SelectedConstructor != null)
            {
                if (registrationContext.ConstructorArguments != null)
                    return registrationContext.SelectedConstructor
                        .MakeNew(registrationContext.ConstructorArguments.Select(Expression.Constant));

                return registrationContext.SelectedConstructor.MakeNew(
                    this.methodExpressionBuilder.CreateParameterExpressionsForMethod(
                        registrationContext,
                        resolutionContext, registrationContext.SelectedConstructor));
            }

            var rule = registrationContext.ConstructorSelectionRule ??
                       resolutionContext.CurrentContainerContext.ContainerConfiguration.ConstructorSelectionRule;
            constructors = rule(constructors);

            return this.methodExpressionBuilder.SelectConstructor(registrationContext, resolutionContext,
                constructors.CastToArray(), out var parameters)?.MakeNew(parameters);
        }
    }
}
