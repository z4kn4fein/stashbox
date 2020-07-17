using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Resolution.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Stashbox.Expressions
{
    internal partial class ExpressionFactory
    {
        private IEnumerable<Expression> CreateParameterExpressionsForMethod(
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

                yield return resolutionContext.ResolutionStrategy.BuildExpressionForType(
                    resolutionContext, parameter) ?? throw new ResolutionFailedException(method.DeclaringType,
                    $"Method {method} found with unresolvable parameter: ({parameter.Type}){parameter.ParameterOrMemberName}");
            }
        }

        private ConstructorInfo SelectConstructor(
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            ConstructorInfo[] constructors,
            out Expression[] parameterExpressions)
        {
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

            throw new ResolutionFailedException(constructors[0].DeclaringType, stringBuilder.ToString());
        }

        private IEnumerable<Expression> CreateMethodExpressions(
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
                        this.CreateParameterExpressionsForMethod(
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

                parameterExpressions[i] = injectionParameter ?? resolutionContext.ResolutionStrategy
                    .BuildExpressionForType(resolutionContext, parameter);

                if (parameterExpressions[i] != null) continue;

                failedParameter = parameter;
                return false;
            }

            return true;
        }
    }
}
