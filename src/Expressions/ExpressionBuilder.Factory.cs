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
        private static Expression GetExpressionForFactory(FactoryRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (resolutionContext.CircularDependencyBarrier.Contains(serviceRegistration.RegistrationId))
                throw new CircularDependencyException(serviceRegistration.ImplementationType);

            resolutionContext.CircularDependencyBarrier.Add(serviceRegistration.RegistrationId);

            var parameters = GetFactoryParameters(serviceRegistration, resolutionContext);
            var expression = ConstructFactoryExpression(serviceRegistration, parameters);
            var result = ExpressionFactory.ConstructBuildUpExpression(serviceRegistration, resolutionContext, expression, resolveType);

            resolutionContext.CircularDependencyBarrier.Pop();
            return result;
        }

        private static Expression ConstructFactoryExpression(FactoryRegistration serviceRegistration, IEnumerable<Expression> parameters)
        {
            if (serviceRegistration.IsFactoryDelegateACompiledLambda || serviceRegistration.Factory.IsCompiledLambda())
                return serviceRegistration.Factory.InvokeDelegate(parameters);

            var method = serviceRegistration.Factory.GetMethod();
            return method.IsStatic
                ? method.CallStaticMethod(parameters)
                : method.CallMethod(serviceRegistration.Factory.Target.AsConstant(), parameters);
        }

        private static IEnumerable<Expression> GetFactoryParameters(FactoryRegistration serviceRegistration, ResolutionContext resolutionContext)
        {
            var length = serviceRegistration.FactoryParameters.Length;
            for (var i = 0; i < length - 1; i++)
            {
                var typeInfo = new TypeInformation(serviceRegistration.FactoryParameters[i], null);
                yield return resolutionContext.CurrentContainerContext.ResolutionStrategy.BuildExpressionForType(resolutionContext, typeInfo).ServiceExpression;
            }
        }
    }
}
