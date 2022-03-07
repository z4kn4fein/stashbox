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
            var members = serviceRegistration.ImplementationType.GetUsableMembers(serviceRegistration,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            if (members.Length == 0 && methods.Length == 0 &&
                serviceRegistration.Initializer == null) return instance;

            var variable = instance.Type.AsVariable();
            var assign = variable.AssignTo(instance);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(GetMemberExpressions(members,
                serviceRegistration, resolutionContext, variable));

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration, resolutionContext, variable));

            if (serviceRegistration.Initializer != null)
                lines.Add(serviceRegistration.Initializer.AsConstant()
                    .CallMethod(serviceRegistration.Initializer.GetType().GetMethod("Invoke")!,
                        variable, resolutionContext.CurrentScopeParameter));

            lines.Add(variable.Type != serviceType ? variable.ConvertTo(serviceType) : variable);

            return lines.AsBlock(variable);
        }

        public static Expression ConstructBuildUpExpression(
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType)
        {
            var serviceRegistration = ServiceRegistration.Empty;
            var type = instance.Type;

            var methods = type.GetUsableMethods();
            var members = type.GetUsableMembers(serviceRegistration,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            if (members.Length == 0 && methods.Length == 0) return instance;

            var variable = type.AsVariable();
            var assign = variable.AssignTo(instance);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(GetMemberExpressions(members,
                serviceRegistration, resolutionContext, variable));

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration, resolutionContext, instance));

            lines.Add(variable.Type != serviceType ? variable.ConvertTo(serviceType) : variable);

            return lines.AsBlock(variable);
        }

        public static Expression? ConstructExpression(
            ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext)
        {
            var constructors = serviceRegistration.ImplementationType.GetConstructors();
            var methods = serviceRegistration.ImplementationType.GetUsableMethods();
            var members = serviceRegistration.ImplementationType.GetUsableMembers(serviceRegistration,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var initExpression = CreateInitExpression(
                serviceRegistration.ImplementationType,
                serviceRegistration,
                resolutionContext,
                constructors);
            if (initExpression == null)
                return null;

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(GetMemberBindings(members,
                    serviceRegistration, resolutionContext));

            if (methods.Length == 0 && serviceRegistration.Initializer == null)
                return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration, resolutionContext, variable));

            if (serviceRegistration.Initializer != null)
                lines.Add(serviceRegistration.Initializer.AsConstant()
                    .CallMethod(serviceRegistration.Initializer.GetType().GetMethod("Invoke")!,
                        variable, resolutionContext.CurrentScopeParameter));

            lines.Add(variable);

            return lines.AsBlock(variable);
        }

        public static Expression? ConstructExpression(
            ResolutionContext resolutionContext,
            Type serviceType)
        {
            var serviceRegistration = ServiceRegistration.Empty;
            var methods = serviceType.GetUsableMethods();
            var members = serviceType.GetUsableMembers(serviceRegistration,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var initExpression = SelectConstructor(
                serviceType,
                serviceRegistration,
                resolutionContext,
                serviceType.GetConstructors(),
                out var parameters)?.MakeNew(parameters) as Expression;

            if (initExpression == null)
                return null;

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(GetMemberBindings(members,
                    serviceRegistration, resolutionContext));

            if (methods.Length == 0) return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };
            lines.AddRange(CreateMethodExpressions(methods, serviceRegistration, resolutionContext, variable));
            lines.Add(variable);

            return lines.AsBlock(variable);
        }

        private static Expression? CreateInitExpression(
            Type typeToConstruct,
            ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            IEnumerable<ConstructorInfo> constructors)
        {
            if (serviceRegistration.SelectedConstructor != null)
            {
                if (serviceRegistration.ConstructorArguments != null)
                    return serviceRegistration.SelectedConstructor
                        .MakeNew(serviceRegistration.ConstructorArguments.Select(Expression.Constant));

                return serviceRegistration.SelectedConstructor.MakeNew(
                    CreateParameterExpressionsForMethod(
                        serviceRegistration,
                        resolutionContext, serviceRegistration.SelectedConstructor));
            }

            var rule = serviceRegistration.ConstructorSelectionRule ??
                       resolutionContext.CurrentContainerContext.ContainerConfiguration.ConstructorSelectionRule;
            constructors = rule(constructors);

            return SelectConstructor(typeToConstruct, serviceRegistration, resolutionContext,
                constructors, out var parameters)?.MakeNew(parameters);
        }
    }
}
