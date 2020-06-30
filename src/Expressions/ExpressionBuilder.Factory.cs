﻿using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Expressions
{
    internal partial class ExpressionBuilder
    {
        private Expression GetExpressionForFactory(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            var expression = serviceRegistration.RegistrationContext.ContainerFactory != null
                ? ConstructFactoryExpression(serviceRegistration.RegistrationContext.ContainerFactory,
                    serviceRegistration, resolutionContext.CurrentScopeParameter)
                : ConstructFactoryExpression(serviceRegistration.RegistrationContext.SingleFactory, serviceRegistration);

            return this.expressionFactory.ConstructBuildUpExpression(serviceRegistration, resolutionContext, expression, resolveType);
        }

        private static Expression ConstructFactoryExpression(Delegate @delegate, ServiceRegistration serviceRegistration, params Expression[] parameters)
        {
            if (serviceRegistration.RegistrationContext.IsFactoryDelegateACompiledLambda || @delegate.IsCompiledLambda())
                return @delegate.InvokeDelegate(parameters);

            var method = @delegate.GetMethod();
            return method.IsStatic
                ? method.CallStaticMethod(parameters)
                : method.CallMethod(@delegate.Target.AsConstant(), parameters);
        }
    }
}