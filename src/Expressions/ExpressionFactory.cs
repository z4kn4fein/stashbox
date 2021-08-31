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
    internal static partial class ExpressionFactory
    {
        public static Expression ConstructBuildUpExpression(
            ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType)
        {
            if (instance.Type != serviceRegistration.ImplementationType)
                instance = instance.ConvertTo(serviceRegistration.ImplementationType);

            var methods = serviceRegistration.ImplementationType.GetUsableMethods();
            var members = serviceRegistration.ImplementationType.GetUsableMembers(serviceRegistration.RegistrationContext,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            if (members.Length == 0 && methods.Length == 0 &&
                serviceRegistration.RegistrationContext.Initializer == null) return instance;

            var variable = instance.Type.AsVariable();
            var assign = variable.AssignTo(instance);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(GetMemberExpressions(members,
                serviceRegistration.RegistrationContext, resolutionContext, variable));

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration.RegistrationContext, resolutionContext, variable));

            if (serviceRegistration.RegistrationContext.Initializer != null)
                lines.Add(serviceRegistration.RegistrationContext.Initializer.AsConstant()
                    .CallMethod(serviceRegistration.RegistrationContext.Initializer.GetType().GetMethod("Invoke"),
                        variable, resolutionContext.CurrentScopeParameter));

            lines.Add(variable.Type != serviceType ? variable.ConvertTo(serviceType) : variable);

            return lines.AsBlock(variable);
        }

        public static Expression ConstructBuildUpExpression(
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType)
        {
            var registrationContext = RegistrationContext.Empty;
            var type = instance.Type;

            var methods = type.GetUsableMethods();
            var members = type.GetUsableMembers(registrationContext,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            if (members.Length == 0 && methods.Length == 0) return instance;

            var variable = type.AsVariable();
            var assign = variable.AssignTo(instance);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(GetMemberExpressions(members,
                registrationContext, resolutionContext, variable));

            lines.AddRange(CreateMethodExpressions(methods,
                registrationContext, resolutionContext, instance));

            lines.Add(variable.Type != serviceType ? variable.ConvertTo(serviceType) : variable);

            return lines.AsBlock(variable);
        }

        public static Expression ConstructExpression(
            ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext)
        {
            var constructors = serviceRegistration.ImplementationType.GetConstructors();
            var methods = serviceRegistration.ImplementationType.GetUsableMethods();
            var members = serviceRegistration.ImplementationType.GetUsableMembers(serviceRegistration.RegistrationContext,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var initExpression = CreateInitExpression(
                serviceRegistration.ImplementationType,
                serviceRegistration.RegistrationContext,
                resolutionContext,
                constructors);
            if (initExpression == null)
                return null;

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(GetMemberBindings(members,
                    serviceRegistration.RegistrationContext, resolutionContext));

            if (methods.Length == 0 && serviceRegistration.RegistrationContext.Initializer == null)
                return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration.RegistrationContext, resolutionContext, variable));

            if (serviceRegistration.RegistrationContext.Initializer != null)
                lines.Add(serviceRegistration.RegistrationContext.Initializer.AsConstant()
                    .CallMethod(serviceRegistration.RegistrationContext.Initializer.GetType().GetMethod("Invoke"),
                        variable, resolutionContext.CurrentScopeParameter));

            lines.Add(variable);

            return lines.AsBlock(variable);
        }

        public static Expression ConstructExpression(
            ResolutionContext resolutionContext,
            Type serviceType)
        {
            var registrationContext = RegistrationContext.Empty;
            var methods = serviceType.GetUsableMethods();
            var members = serviceType.GetUsableMembers(registrationContext,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var initExpression = (Expression)SelectConstructor(
                serviceType,
                registrationContext,
                resolutionContext,
                serviceType.GetConstructors(),
                out var parameters).MakeNew(parameters);

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(GetMemberBindings(members,
                    registrationContext, resolutionContext));

            if (methods.Length == 0) return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };
            lines.AddRange(CreateMethodExpressions(methods, registrationContext, resolutionContext, variable));
            lines.Add(variable);

            return lines.AsBlock(variable);
        }

        private static Expression CreateInitExpression(
            Type typeToConstruct,
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
                    CreateParameterExpressionsForMethod(
                        registrationContext,
                        resolutionContext, registrationContext.SelectedConstructor));
            }

            var rule = registrationContext.ConstructorSelectionRule ??
                       resolutionContext.CurrentContainerContext.ContainerConfiguration.ConstructorSelectionRule;
            constructors = rule(constructors);

            return SelectConstructor(typeToConstruct, registrationContext, resolutionContext,
                constructors, out var parameters)?.MakeNew(parameters);
        }
    }
}
