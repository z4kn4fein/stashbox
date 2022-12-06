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
        private static Expression GetExpressionForFactory(ServiceRegistration serviceRegistration, FactoryOptions factoryOptions, ResolutionContext resolutionContext, TypeInformation typeInformation)
        {
            if (resolutionContext.CircularDependencyBarrier.Contains(serviceRegistration.RegistrationId))
                throw new CircularDependencyException(serviceRegistration.ImplementationType);

            resolutionContext.CircularDependencyBarrier.Add(serviceRegistration.RegistrationId);

            var parameters = GetFactoryParameters(factoryOptions, resolutionContext);
            var expression = ConstructFactoryExpression(factoryOptions, parameters);
            var result = ExpressionFactory.ConstructBuildUpExpression(serviceRegistration, resolutionContext, expression, typeInformation);

            resolutionContext.CircularDependencyBarrier.Pop();
            return result;
        }

        private static Expression ConstructFactoryExpression(FactoryOptions factoryOptions, IEnumerable<Expression> parameters)
        {
            if (factoryOptions.IsFactoryDelegateACompiledLambda || factoryOptions.Factory.IsCompiledLambda())
                return factoryOptions.Factory.InvokeDelegate(parameters);

            var method = factoryOptions.Factory.GetMethod();
            return method.IsStatic
                ? method.CallStaticMethod(parameters)
                : method.CallMethod(factoryOptions.Factory.Target.AsConstant(), parameters);
        }

        private static IEnumerable<Expression> GetFactoryParameters(FactoryOptions factoryOptions, ResolutionContext resolutionContext)
        {
            var length = factoryOptions.FactoryParameters.Length;
            for (var i = 0; i < length - 1; i++)
            {
                var typeInfo = new TypeInformation(factoryOptions.FactoryParameters[i], null);
                yield return resolutionContext.CurrentContainerContext.ResolutionStrategy.BuildExpressionForType(resolutionContext, typeInfo).ServiceExpression;
            }
        }
    }
}
