﻿using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp
{
    internal class FuncObjectBuilder : ObjectBuilderBase
    {
        private volatile Expression expression;
        private readonly object syncObject = new object();

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo,
            Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;

                var internalMethodInfo = serviceRegistration.RegistrationContext.FuncDelegate.GetMethod();

                var parameters = this.GetFuncParametersWithScope(serviceRegistration.ServiceType.GetSingleMethod("Invoke").GetParameters(), resolutionInfo);
                var expr = internalMethodInfo.IsStatic ?
                    Expression.Call(internalMethodInfo, parameters) :
                    Expression.Call(Expression.Constant(serviceRegistration.RegistrationContext.FuncDelegate.Target), internalMethodInfo, parameters);

                return this.expression = Expression.Lambda(expr, parameters.Take(parameters.Length - 1).Cast<ParameterExpression>());
            }
        }

        public override IObjectBuilder Produce() => new FuncObjectBuilder();

        private Expression[] GetFuncParametersWithScope(ParameterInfo[] parameterInfos, ResolutionInfo resolutionInfo)
        {
            var length = parameterInfos.Length;
            var expressions = new Expression[length + 1];

            for (int i = 0; i < length; i++)
                expressions[i] = Expression.Parameter(parameterInfos[i].ParameterType);

            expressions[expressions.Length - 1] = Expression.Convert(resolutionInfo.CurrentScopeParameter, Constants.ResolverType);

            return expressions;
        }
    }
}
