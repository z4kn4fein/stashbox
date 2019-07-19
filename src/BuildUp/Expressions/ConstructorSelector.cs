using Stashbox.Entity;
using Stashbox.Entity.Resolution;
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
    internal class ConstructorSelector : IConstructorSelector
    {
        public ResolutionConstructor CreateResolutionConstructor(
            IContainerContext containerContext,
            IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext,
            ConstructorInformation constructor)
        {
            var paramLength = constructor.Parameters.Length;
            var parameterExpressions = new Expression[paramLength];

            for (var i = 0; i < paramLength; i++)
            {
                var parameter = constructor.Parameters[i];

                var expression = containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext,
                    resolutionContext, parameter, serviceRegistration.RegistrationContext.InjectionParameters);

                parameterExpressions[i] = expression ?? throw new ResolutionFailedException(serviceRegistration.ImplementationType,
                    $"Constructor {constructor.Constructor}, unresolvable parameter: ({parameter.Type}){parameter.ParameterOrMemberName}");
            }

            return new ResolutionConstructor { Constructor = constructor.Constructor, Parameters = parameterExpressions };
        }

        public ResolutionConstructor SelectConstructor(
            Type implementationType,
            IContainerContext containerContext,
            ResolutionContext resolutionContext,
            ConstructorInformation[] constructors,
            IEnumerable<InjectionParameter> injectionParameters)
        {
            var length = constructors.Length;
            var checkedConstructors = new Dictionary<ConstructorInfo, TypeInformation>();
            for (var i = 0; i < length; i++)
            {
                var constructor = constructors[i];

                if (!this.TryBuildResolutionConstructor(constructor, resolutionContext, containerContext,
                    injectionParameters, out var failedParameter, out var parameterExpressions, true))
                {
                    checkedConstructors.Add(constructor.Constructor, failedParameter);
                    continue;
                }

                return new ResolutionConstructor { Constructor = constructor.Constructor, Parameters = parameterExpressions };
            }

            if (containerContext.ContainerConfiguration.UnknownTypeResolutionEnabled)
                for (var i = 0; i < length; i++)
                {
                    var constructor = constructors[i];
                    if (this.TryBuildResolutionConstructor(constructor, resolutionContext, containerContext,
                        injectionParameters, out var failedParameter, out var parameterExpressions))
                        return new ResolutionConstructor { Constructor = constructor.Constructor, Parameters = parameterExpressions };
                }

            if (resolutionContext.NullResultAllowed)
                return null;

            var stringBuilder = new StringBuilder();
            foreach (var checkedConstructor in checkedConstructors)
                stringBuilder.AppendLine($"Checked constructor {checkedConstructor.Key}, unresolvable parameter: ({checkedConstructor.Value.Type}){checkedConstructor.Value.ParameterOrMemberName}");

            throw new ResolutionFailedException(implementationType, stringBuilder.ToString());
        }

        private bool TryBuildResolutionConstructor(
            ConstructorInformation constructor,
            ResolutionContext resolutionContext,
            IContainerContext containerContext,
            IEnumerable<InjectionParameter> injectionParameters,
            out TypeInformation failedParameter,
            out Expression[] parameterExpressions,
            bool skipUknownResolution = false)
        {
            var paramLength = constructor.Parameters.Length;
            parameterExpressions = new Expression[paramLength];
            failedParameter = null;
            for (var i = 0; i < paramLength; i++)
            {
                var parameter = constructor.Parameters[i];

                parameterExpressions[i] = containerContext.ResolutionStrategy.BuildResolutionExpression(containerContext,
                    resolutionContext, parameter, injectionParameters, skipUknownResolution);

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
