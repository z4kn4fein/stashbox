using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Stashbox.Expressions
{
    internal static partial class ExpressionBuilder
    {
        private static Expression GetExpressionForFactory(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (resolutionContext.WeAreInCircle(serviceRegistration.RegistrationId))
                throw new CircularDependencyException(serviceRegistration.ImplementationType);

            resolutionContext.PullOutCircularDependencyBarrier(serviceRegistration.RegistrationId);

            var parameters = GetFactoryParameters(serviceRegistration, resolutionContext);
            var expression = ConstructFactoryExpression(serviceRegistration, parameters);
            var result = ExpressionFactory.ConstructBuildUpExpression(serviceRegistration, resolutionContext, expression, resolveType);

            resolutionContext.LetDownCircularDependencyBarrier();
            return result;
        }

        private static Expression ConstructFactoryExpression(ServiceRegistration serviceRegistration, IEnumerable<Expression> parameters)
        {
            if (serviceRegistration.RegistrationContext.IsFactoryDelegateACompiledLambda || serviceRegistration.RegistrationContext.Factory.IsCompiledLambda())
                return serviceRegistration.RegistrationContext.Factory.InvokeDelegate(parameters);

            var method = serviceRegistration.RegistrationContext.Factory.GetMethod();
            return method.IsStatic
                ? method.CallStaticMethod(parameters)
                : method.CallMethod(serviceRegistration.RegistrationContext.Factory.Target.AsConstant(), parameters);
        }

        private static IEnumerable<Expression> GetFactoryParameters(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            var length = serviceRegistration.RegistrationContext.FactoryParameters.Length;
            for (int i = 0; i < length - 1; i++)
            {
                var typeInfo = new TypeInformation(serviceRegistration.RegistrationContext.FactoryParameters[i], null);
                yield return resolutionContext.CurrentContainerContext.ResolutionStrategy.BuildExpressionForType(resolutionContext, typeInfo);
            }
        }
    }
}
