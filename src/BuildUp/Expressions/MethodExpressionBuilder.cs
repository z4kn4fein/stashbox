using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Stashbox.BuildUp.Expressions
{
    internal class MethodExpressionBuilder : IMethodExpressionBuilder
    {
        private readonly IResolutionStrategy resolutionStrategy;

        public MethodExpressionBuilder(IResolutionStrategy resolutionStrategy)
        {
            this.resolutionStrategy = resolutionStrategy;
        }

        public IEnumerable<Expression> CreateParameterExpressionsForMethod(
            IContainerContext containerContext,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            MethodBase method)
        {
            var parameters = method.GetParameters();
            var paramLength = parameters.Length;
            for (var i = 0; i < paramLength; i++)
            {
                var parameter = parameters[i].AsTypeInformation(method.DeclaringType, registrationContext, containerContext.ContainerConfiguration);
                yield return this.resolutionStrategy.BuildResolutionExpression(containerContext,
                    resolutionContext, parameter, registrationContext.InjectionParameters) ?? throw new ResolutionFailedException(method.DeclaringType,
                    $"Method {method}, unresolvable parameter: ({parameter.Type}){parameter.ParameterOrMemberName}");
            }
        }

        public ConstructorInfo SelectConstructor(
            IContainerContext containerContext,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            ConstructorInfo[] constructors,
            out Expression[] parameterExpressions)
        {
            var checkedConstructors = new Dictionary<MethodBase, TypeInformation>();

            var length = constructors.Length;
            for (int i = 0; i < length; i++)
            {
                var constructor = constructors[i];
                if (!this.TryBuildMethod(constructor, registrationContext, resolutionContext, containerContext, out var failedParameter, out parameterExpressions, true))
                {
                    checkedConstructors.Add(constructor, failedParameter);
                    continue;
                }

                return constructor;
            }

            if (containerContext.ContainerConfiguration.UnknownTypeResolutionEnabled)
                for (int i = 0; i < length; i++)
                {
                    ConstructorInfo constructor = constructors[i];
                    if (this.TryBuildMethod(constructor, registrationContext, resolutionContext, containerContext, out _, out parameterExpressions))
                        return constructor;
                }

            if (resolutionContext.NullResultAllowed)
            {
                parameterExpressions = null;
                return null;
            }

            var stringBuilder = new StringBuilder();
            foreach (var checkedConstructor in checkedConstructors)
                stringBuilder.AppendLine($"Checked constructor: {checkedConstructor.Key}, unresolvable parameter: ({checkedConstructor.Value.Type.FullName}){checkedConstructor.Value.ParameterOrMemberName}.");

            throw new ResolutionFailedException(constructors[0].DeclaringType, stringBuilder.ToString());
        }

        public IEnumerable<Expression> CreateMethodExpressions(
            IContainerContext containerContext,
            MethodInfo[] injectionMethods,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            Expression instance)
        {
            var length = injectionMethods.Length;
            for (int i = 0; i < length; i++)
            {
                var method = injectionMethods[i];
                var parameters = method.GetParameters();
                var paramLength = parameters.Length;
                if (paramLength == 0)
                    yield return instance.CallMethod(method);
                else
                    yield return instance.CallMethod(method, this.CreateParameterExpressionsForMethod(containerContext,
                        registrationContext, resolutionContext, method));
            }
        }

        private bool TryBuildMethod(
            MethodBase method,
            RegistrationContext registrationContext,
            ResolutionContext resolutionContext,
            IContainerContext containerContext,
            out TypeInformation failedParameter,
            out Expression[] parameterExpressions,
            bool skipUknownResolution = false)
        {
            var parameters = method.GetParameters();
            var paramLength = parameters.Length;
            parameterExpressions = new Expression[paramLength];
            failedParameter = default;
            for (var i = 0; i < paramLength; i++)
            {
                var parameter = parameters[i].AsTypeInformation(method.DeclaringType, registrationContext, containerContext.ContainerConfiguration);

                parameterExpressions[i] = this.resolutionStrategy.BuildResolutionExpression(containerContext,
                    resolutionContext, parameter, registrationContext.InjectionParameters, skipUknownResolution);

                if (parameterExpressions[i] == null)
                {
                    failedParameter = parameter;
                    return false;
                }
            }

            return true;
        }
    }
}
