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
            TypeInformation typeInformation)
        {
            if (instance.Type != serviceRegistration.ImplementationType)
                instance = instance.ConvertTo(serviceRegistration.ImplementationType);

            var methods = serviceRegistration.ImplementationType.GetUsableMethods();
            var members = serviceRegistration.ImplementationType.GetUsableMembers(serviceRegistration,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            var initializer = serviceRegistration.Options.GetOrDefault(RegistrationOption.Initializer);

            if (members.Length == 0 && methods.Length == 0 &&
                initializer == null) return instance;

            var variable = instance.Type.AsVariable();
            var assign = variable.AssignTo(instance);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(GetMemberExpressions(members,
                serviceRegistration, resolutionContext, variable, typeInformation));

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration, resolutionContext, variable, typeInformation));

            if (initializer != null)
                lines.Add(initializer.AsConstant()
                    .CallMethod(initializer.GetType().GetMethod("Invoke")!,
                        variable, resolutionContext.CurrentScopeParameter));

            lines.Add(variable.Type != typeInformation.Type ? variable.ConvertTo(typeInformation.Type) : variable);

            return lines.AsBlock(variable);
        }

        public static Expression ConstructBuildUpExpression(
            ResolutionContext resolutionContext,
            Expression instance,
            TypeInformation typeInformation)
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
                null, resolutionContext, variable, typeInformation));

            lines.AddRange(CreateMethodExpressions(methods,
                null, resolutionContext, instance, typeInformation));

            lines.Add(variable.Type != typeInformation.Type ? variable.ConvertTo(typeInformation.Type) : variable);

            return lines.AsBlock(variable);
        }

        public static Expression? ConstructExpression(
            ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            TypeInformation typeInformation)
        {
            var constructors = serviceRegistration.ImplementationType.GetConstructors();

            var initExpression = CreateInitExpression(
                serviceRegistration.ImplementationType,
                serviceRegistration,
                typeInformation,
                resolutionContext,
                constructors);
            if (initExpression == null)
                return null;

            var methods = serviceRegistration.ImplementationType.GetUsableMethods();
            var members = serviceRegistration.ImplementationType.GetUsableMembers(serviceRegistration,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(GetMemberBindings(members,
                    serviceRegistration, resolutionContext, typeInformation));

            var initializer = serviceRegistration.Options.GetOrDefault(RegistrationOption.Initializer);

            if (methods.Length == 0 && initializer == null)
                return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };

            lines.AddRange(CreateMethodExpressions(methods,
                serviceRegistration, resolutionContext, variable, typeInformation));

            if (initializer != null)
                lines.Add(initializer.AsConstant()
                    .CallMethod(initializer.GetType().GetMethod("Invoke")!,
                        variable, resolutionContext.CurrentScopeParameter));

            lines.Add(variable);

            return lines.AsBlock(variable);
        }

        public static Expression? ConstructExpression(
            ResolutionContext resolutionContext, TypeInformation typeInformation)
        {
            var methods = typeInformation.Type.GetUsableMethods();
            var members = typeInformation.Type.GetUsableMembers(null,
                resolutionContext.CurrentContainerContext.ContainerConfiguration);

            if (SelectConstructor(
                typeInformation.Type,
                null,
                typeInformation,
                resolutionContext,
                typeInformation.Type.GetConstructors(),
                out var parameters)?.MakeNew(parameters) is not Expression initExpression)
                return null;

            if (members.Length > 0)
                initExpression = initExpression.InitMembers(GetMemberBindings(members,
                    null, resolutionContext, typeInformation));

            if (methods.Length == 0) return initExpression;

            var variable = initExpression.Type.AsVariable();
            var assign = variable.AssignTo(initExpression);

            var lines = new ExpandableArray<Expression> { assign };
            lines.AddRange(CreateMethodExpressions(methods, null, resolutionContext, variable, typeInformation));
            lines.Add(variable);

            return lines.AsBlock(variable);
        }

        private static Expression? CreateInitExpression(
            Type typeToConstruct,
            ServiceRegistration serviceRegistration,
            TypeInformation typeInformation,
            ResolutionContext resolutionContext,
            IEnumerable<ConstructorInfo> constructors)
        {
            var rule = resolutionContext.CurrentContainerContext.ContainerConfiguration.ConstructorSelectionRule;
            var constructorOptions = serviceRegistration.Options.GetOrDefault<ConstructorOptions>(RegistrationOption.ConstructorOptions);
            if (constructorOptions != null && constructorOptions.SelectedConstructor != null)
            {
                if (constructorOptions.ConstructorArguments != null)
                    return constructorOptions.SelectedConstructor
                        .MakeNew(constructorOptions.ConstructorArguments.Select(Expression.Constant));

                return constructorOptions.SelectedConstructor.MakeNew(
                    CreateParameterExpressionsForMethod(
                        serviceRegistration,
                        resolutionContext, constructorOptions.SelectedConstructor, typeInformation));
            }

            var constructorSelectionRule = serviceRegistration.Options.GetOrDefault<Func<IEnumerable<ConstructorInfo>, IEnumerable<ConstructorInfo>>>(RegistrationOption.ConstructorSelectionRule);
            if (constructorSelectionRule != null)
                rule = constructorSelectionRule;

            constructors = rule(constructors);

            return SelectConstructor(typeToConstruct, serviceRegistration, typeInformation, resolutionContext,
                constructors, out var parameters)?.MakeNew(parameters);
        }
    }
}
