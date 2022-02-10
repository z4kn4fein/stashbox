using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Resolution.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Stashbox.Expressions
{
    internal static partial class ExpressionFactory
    {
        private static IEnumerable<Expression> CreateParameterExpressionsForMethod(
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            MethodBase method)
        {
            var parameters = method.GetParameters();
            var paramLength = parameters.Length;
            for (var i = 0; i < paramLength; i++)
            {
                var parameter = parameters[i].AsTypeInformation(method.DeclaringType, registrationContext,
                    resolutionContext.CurrentContainerContext.ContainerConfiguration);

                var injectionParameter = registrationContext.InjectionParameters.SelectInjectionParameterOrDefault(parameter);
                if (injectionParameter != null) yield return injectionParameter;

                yield return resolutionContext.CurrentContainerContext.ResolutionStrategy.BuildExpressionForType(
                    resolutionContext, parameter).ServiceExpression ?? throw new ResolutionFailedException(method.DeclaringType, registrationContext.Name,
                    $"Method {method} found with unresolvable parameter: ({parameter.Type}){parameter.ParameterOrMemberName}");
            }
        }

        private static ConstructorInfo SelectConstructor(
            Type typeToConstruct,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            IEnumerable<ConstructorInfo> constructorsEnumerable,
            out Expression[] parameterExpressions)
        {

            ConstructorInfo[] constructors = null;
            if (resolutionContext.ParameterExpressions.Length > 0)
            {
                var containingFactoryParameter = constructorsEnumerable
                    .Where(c => c.GetParameters()
                        .Any(p => resolutionContext.ParameterExpressions
                            .WhereOrDefault(pe => pe.Any(item => item.I2.Type == p.ParameterType ||
                                                                 item.I2.Type.Implements(p.ParameterType)))?.Any() ?? false));

                var everythingElse = constructorsEnumerable.Except(containingFactoryParameter);
                constructors = containingFactoryParameter.Concat(everythingElse).CastToArray();
            }
            else
                constructors = constructorsEnumerable.CastToArray();

            if (constructors.Length == 0)
                throw new ResolutionFailedException(typeToConstruct,
                    registrationContext.Name,
                    "No public constructor found. Make sure there is at least one public constructor on the type.");

            var checkedConstructors = new Dictionary<MethodBase, TypeInformation>();

            var unknownTypeCheckDisabledContext = resolutionContext.CurrentContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled
                ? resolutionContext.BeginUnknownTypeCheckDisabledContext()
                : resolutionContext;

            var length = constructors.Length;
            for (var i = 0; i < length; i++)
            {
                var constructor = constructors[i];
                if (TryBuildMethod(constructor, registrationContext, unknownTypeCheckDisabledContext,
                    out var failedParameter, out parameterExpressions)) return constructor;

                checkedConstructors.Add(constructor, failedParameter);
            }

            if (resolutionContext.CurrentContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled)
                for (var i = 0; i < length; i++)
                {
                    var constructor = constructors[i];
                    if (TryBuildMethod(constructor, registrationContext, resolutionContext, out _, out parameterExpressions))
                        return constructor;
                }

            if (resolutionContext.NullResultAllowed)
            {
                parameterExpressions = null;
                return null;
            }

            var stringBuilder = new StringBuilder();
            foreach (var checkedConstructor in checkedConstructors)
                stringBuilder.AppendLine($"Constructor {checkedConstructor.Key} found with unresolvable parameter: ({checkedConstructor.Value.Type.FullName}){checkedConstructor.Value.ParameterOrMemberName}.");

            throw new ResolutionFailedException(typeToConstruct, registrationContext.Name, stringBuilder.ToString());
        }

        private static IEnumerable<Expression> CreateMethodExpressions(
            IEnumerable<MethodInfo> methods,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression instance)
        {
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length == 0)
                    yield return instance.CallMethod(method);
                else
                    yield return instance.CallMethod(method,
                        CreateParameterExpressionsForMethod(
                        registrationContext, resolutionContext, method));
            }
        }

        private static bool TryBuildMethod(
            MethodBase method,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            out TypeInformation failedParameter,
            out Expression[] parameterExpressions)
        {
            var parameters = method.GetParameters();
            var paramLength = parameters.Length;
            parameterExpressions = new Expression[paramLength];
            failedParameter = default;
            for (var i = 0; i < paramLength; i++)
            {
                var parameter = parameters[i].AsTypeInformation(method.DeclaringType, registrationContext,
                    resolutionContext.CurrentContainerContext.ContainerConfiguration);

                var injectionParameter = registrationContext.InjectionParameters.SelectInjectionParameterOrDefault(parameter);

                parameterExpressions[i] = injectionParameter ?? resolutionContext.CurrentContainerContext.ResolutionStrategy
                    .BuildExpressionForType(resolutionContext, parameter).ServiceExpression;

                if (parameterExpressions[i] != null) continue;

                failedParameter = parameter;
                return false;
            }

            return true;
        }
    }
}
