using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Resolution.Extensions;
using Stashbox.Utils;
using Stashbox.Utils.Data;
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
            ServiceRegistration? serviceRegistration,
            ResolutionContext resolutionContext,
            MethodBase method)
        {
            var parameters = method.GetParameters();
            var paramLength = parameters.Length;
            for (var i = 0; i < paramLength; i++)
            {
                var parameter = parameters[i].AsTypeInformation(method.DeclaringType, serviceRegistration,
                    resolutionContext.CurrentContainerContext.ContainerConfiguration);

                var injectionParameter = serviceRegistration?.Options.GetOrDefault<ExpandableArray<KeyValuePair<string, object?>>>(RegistrationOption.InjectionParameters)?.SelectInjectionParameterOrDefault(parameter);
                if (injectionParameter != null) yield return injectionParameter;

                yield return resolutionContext.CurrentContainerContext.ResolutionStrategy.BuildExpressionForType(
                    resolutionContext, parameter).ServiceExpression ?? throw new ResolutionFailedException(method.DeclaringType, serviceRegistration?.Name,
                    $"Method {method} found with unresolvable parameter: ({parameter.Type}){parameter.ParameterOrMemberName}");
            }
        }

        private static ConstructorInfo? SelectConstructor(
            Type typeToConstruct,
            ServiceRegistration? serviceRegistration,
            ResolutionContext resolutionContext,
            IEnumerable<ConstructorInfo> constructorsEnumerable,
            out Expression[] parameterExpressions)
        {
            ConstructorInfo[]? resultConstructors = null;
            parameterExpressions = Constants.EmptyArray<Expression>();
            if (resolutionContext.ParameterExpressions.Length > 0)
            {
                var constructors = constructorsEnumerable.CastToArray();
                var containingFactoryParameter = constructors
                    .Where(c => c.GetParameters()
                        .Any(p => resolutionContext.ParameterExpressions
                            .Any(pe => pe.Any(item => item.I2.Type == p.ParameterType || item.I2.Type.Implements(p.ParameterType)))))
                    .CastToArray();

                var everythingElse = constructors.Except(containingFactoryParameter);
                resultConstructors = containingFactoryParameter.Concat(everythingElse).CastToArray();
            }
            else
                resultConstructors = constructorsEnumerable.CastToArray();

            if (resultConstructors.Length == 0)
                throw new ResolutionFailedException(typeToConstruct,
                    serviceRegistration?.Name,
                    "No public constructor found. Make sure there is at least one public constructor on the type.");

            var checkedConstructors = new Dictionary<MethodBase, TypeInformation>();

            var unknownTypeCheckDisabledContext = resolutionContext.CurrentContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled
                ? resolutionContext.BeginUnknownTypeCheckDisabledContext()
                : resolutionContext;

            var length = resultConstructors.Length;
            for (var i = 0; i < length; i++)
            {
                var constructor = resultConstructors[i];
                if (TryBuildMethod(constructor, serviceRegistration, unknownTypeCheckDisabledContext,
                    out var failedParameter, out parameterExpressions)) return constructor;

                checkedConstructors.Add(constructor, failedParameter);
            }

            if (resolutionContext.CurrentContainerContext.ContainerConfiguration.UnknownTypeResolutionEnabled)
            {
                for (var i = 0; i < length; i++)
                {
                    var constructor = resultConstructors[i];
                    if (TryBuildMethod(constructor, serviceRegistration, resolutionContext, out _, out parameterExpressions))
                        return constructor;
                }
            }

            if (resolutionContext.NullResultAllowed)
            {
                return null;
            }

            var stringBuilder = new StringBuilder();
            foreach (var checkedConstructor in checkedConstructors)
                stringBuilder.AppendLine($"Constructor {checkedConstructor.Key} found with unresolvable parameter: ({checkedConstructor.Value.Type.FullName}){checkedConstructor.Value.ParameterOrMemberName}.");

            throw new ResolutionFailedException(typeToConstruct, serviceRegistration?.Name, stringBuilder.ToString());
        }

        private static IEnumerable<Expression> CreateMethodExpressions(
            IEnumerable<MethodInfo> methods,
            ServiceRegistration? serviceRegistration,
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
                        serviceRegistration, resolutionContext, method));
            }
        }

        private static bool TryBuildMethod(
            MethodBase method,
            ServiceRegistration? serviceRegistration,
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
                var parameter = parameters[i].AsTypeInformation(method.DeclaringType, serviceRegistration,
                    resolutionContext.CurrentContainerContext.ContainerConfiguration);

                var injectionParameter = serviceRegistration?.Options.GetOrDefault<ExpandableArray<KeyValuePair<string, object?>>>(RegistrationOption.InjectionParameters)?.SelectInjectionParameterOrDefault(parameter);
                if (injectionParameter != null)
                {
                    parameterExpressions[i] = injectionParameter;
                    continue;
                }

                var serviceContext = resolutionContext.CurrentContainerContext.ResolutionStrategy
                    .BuildExpressionForType(resolutionContext, parameter);

                if (!serviceContext.IsEmpty())
                {
                    parameterExpressions[i] = serviceContext.ServiceExpression;
                    continue;
                }

                failedParameter = parameter;
                return false;
            }

            return true;
        }
    }
}
