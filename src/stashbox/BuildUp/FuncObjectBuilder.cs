using Stashbox.Registration;
using Stashbox.Resolution;
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

        protected override Expression GetExpressionInternal(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext,
            Type resolveType)
        {
            if (this.expression != null) return this.expression;
            lock (this.syncObject)
            {
                if (this.expression != null) return this.expression;

                var internalMethodInfo = serviceRegistration.RegistrationContext.FuncDelegate.GetMethod();

                var parameters = this.GetFuncParametersWithScope(serviceRegistration.ServiceType.GetSingleMethod("Invoke").GetParameters(), resolutionContext);
                var expr = internalMethodInfo.IsStatic ?
                    internalMethodInfo.InvokeMethod(parameters) :
                    serviceRegistration.RegistrationContext.FuncDelegate.Target.AsConstant().CallMethod(internalMethodInfo, parameters);

                return this.expression = expr.AsLambda(parameters.Take(parameters.Length - 1).Cast<ParameterExpression>());
            }
        }

        public override IObjectBuilder Produce() => new FuncObjectBuilder();

        private Expression[] GetFuncParametersWithScope(ParameterInfo[] parameterInfos, ResolutionContext resolutionContext)
        {
            var length = parameterInfos.Length;
            var expressions = new Expression[length + 1];

            for (int i = 0; i < length; i++)
                expressions[i] = parameterInfos[i].ParameterType.AsParameter();

            expressions[expressions.Length - 1] = resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType);

            return expressions;
        }
    }
}
