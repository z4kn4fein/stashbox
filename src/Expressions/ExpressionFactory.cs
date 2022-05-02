using Stashbox.Registration.ServiceRegistrations;
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

            var complexRegistration = serviceRegistration as ComplexRegistration;

            if (members.Length == 0 && methods.Length == 0 &&
                complexRegistration?.Initializer == null) return instance;

            var variable = instance.Type.AsVariable();
            var assign = variable.AssignTo(instance);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(GetMemberExpressions(members,
                serviceRegistration, resolutionContext, variable));

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration, resolutionContext, variable));

            if (complexRegistration?.Initializer != null)
                lines.Add(complexRegistration.Initializer.AsConstant()
                    .CallMethod(complexRegistration.Initializer.GetType().GetMethod("Invoke")!,
                        variable, resolutionContext.CurrentScopeParameter));

            lines.Add(variable.Type != serviceType ? variable.ConvertTo(serviceType) : variable);

            return lines.AsBlock(variable);
        }

        public static Expression ConstructBuildUpExpression(
            ResolutionContext resolutionContext,
            Expression instance,
            Type serviceType)
        {
            var type = instance.Type;

            var methods = type.GetUsableMethods();
            var members = type.GetUsableMembers(null,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            if (members.Length == 0 && methods.Length == 0) return instance;

            var variable = type.AsVariable();
            var assign = variable.AssignTo(instance);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(GetMemberExpressions(members,
                null, resolutionContext, variable));

            lines.AddRange(CreateMethodExpressions(methods,
                null, resolutionContext, instance));

            lines.Add(variable.Type != serviceType ? variable.ConvertTo(serviceType) : variable);

            return lines.AsBlock(variable);
        }

        public static Expression? ConstructExpression(
            ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext)
        {
            var constructors = serviceRegistration.ImplementationType.GetConstructors();

            var initExpression = CreateInitExpression(
                serviceRegistration.ImplementationType,
                serviceRegistration,
                resolutionContext,
                constructors);
            if (initExpression == null)
                return null;

            var methods = serviceRegistration.ImplementationType.GetUsableMethods();
            var members = serviceRegistration.ImplementationType.GetUsableMembers(serviceRegistration,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(GetMemberBindings(members,
                    serviceRegistration, resolutionContext));

            var complexRegistration = serviceRegistration as ComplexRegistration;

            if (methods.Length == 0 && complexRegistration?.Initializer == null)
                return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration, resolutionContext, variable));

            if (complexRegistration?.Initializer != null)
                lines.Add(complexRegistration.Initializer.AsConstant()
                    .CallMethod(complexRegistration.Initializer.GetType().GetMethod("Invoke")!,
                        variable, resolutionContext.CurrentScopeParameter));

            lines.Add(variable);

            return lines.AsBlock(variable);
        }

        public static Expression? ConstructExpression(
            ResolutionContext resolutionContext,
            Type serviceType)
        {
            var methods = serviceType.GetUsableMethods();
            var members = serviceType.GetUsableMembers(null,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var initExpression = SelectConstructor(
                serviceType,
                null,
                resolutionContext,
                serviceType.GetConstructors(),
                out var parameters)?.MakeNew(parameters) as Expression;

            if (initExpression == null)
                return null;

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(GetMemberBindings(members,
                    null, resolutionContext));

            if (methods.Length == 0) return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };
            lines.AddRange(CreateMethodExpressions(methods, null, resolutionContext, variable));
            lines.Add(variable);

            return lines.AsBlock(variable);
        }

        private static Expression? CreateInitExpression(
            Type typeToConstruct,
            ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            IEnumerable<ConstructorInfo> constructors)
        {
            var rule = resolutionContext.CurrentContainerContext.ContainerConfiguration.ConstructorSelectionRule;
            if (serviceRegistration is ComplexRegistration complexRegistration)
            {
                if (complexRegistration.SelectedConstructor != null)
                {
                    if (complexRegistration.ConstructorArguments != null)
                        return complexRegistration.SelectedConstructor
                            .MakeNew(complexRegistration.ConstructorArguments.Select(Expression.Constant));

                    return complexRegistration.SelectedConstructor.MakeNew(
                        CreateParameterExpressionsForMethod(
                            serviceRegistration,
                            resolutionContext, complexRegistration.SelectedConstructor));
                }

                if (complexRegistration.ConstructorSelectionRule != null)
                    rule = complexRegistration.ConstructorSelectionRule;
            }

            constructors = rule(constructors);

            return SelectConstructor(typeToConstruct, serviceRegistration, resolutionContext,
                constructors, out var parameters)?.MakeNew(parameters);
        }
    }
}
