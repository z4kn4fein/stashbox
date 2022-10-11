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

            var initializer = serviceRegistration.Options.GetOrDefault(OptionIds.Initializer);

            if (members.Length == 0 && methods.Length == 0 &&
                initializer == null) return instance;

            var variable = instance.Type.AsVariable();
            var assign = variable.AssignTo(instance);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(GetMemberExpressions(members,
                serviceRegistration, resolutionContext, variable));

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration, resolutionContext, variable));

            if (initializer != null)
                lines.Add(initializer.AsConstant()
                    .CallMethod(initializer.GetType().GetMethod("Invoke")!,
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

            var initializer = serviceRegistration.Options.GetOrDefault(OptionIds.Initializer);

            if (methods.Length == 0 && initializer == null)
                return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration, resolutionContext, variable));

            if (initializer != null)
                lines.Add(initializer.AsConstant()
                    .CallMethod(initializer.GetType().GetMethod("Invoke")!,
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

            if (SelectConstructor(
                serviceType,
                null,
                resolutionContext,
                serviceType.GetConstructors(),
                out var parameters)?.MakeNew(parameters) is not Expression initExpression)
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
            var constructorOptions = serviceRegistration.Options.GetOrDefault<ConstructorOptions>(OptionIds.ConstructorOptions);
            if (constructorOptions != null)
            {
                if (constructorOptions.SelectedConstructor != null)
                {
                    if (constructorOptions.ConstructorArguments != null)
                        return constructorOptions.SelectedConstructor
                            .MakeNew(constructorOptions.ConstructorArguments.Select(Expression.Constant));

                    return constructorOptions.SelectedConstructor.MakeNew(
                        CreateParameterExpressionsForMethod(
                            serviceRegistration,
                            resolutionContext, constructorOptions.SelectedConstructor));
                }
            }

            var constructorSelectionRule = serviceRegistration.Options.GetOrDefault<Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>>>(OptionIds.ConstructorSelectionRule);
            if (constructorSelectionRule != null)
                rule = constructorSelectionRule;

            constructors = rule(constructors);

            return SelectConstructor(typeToConstruct, serviceRegistration, resolutionContext,
                constructors, out var parameters)?.MakeNew(parameters);
        }
    }
}
