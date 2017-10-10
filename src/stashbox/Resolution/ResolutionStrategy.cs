using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    internal class ResolutionStrategy : IResolutionStrategy
    {
        private readonly IResolverSelector resolverSelector;

        internal ResolutionStrategy(IResolverSelector resolverSelector)
        {
            this.resolverSelector = resolverSelector;
        }

        public Expression BuildResolutionExpression(IContainerContext containerContext, ResolutionInfo resolutionInfo, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters)
        {
            if (typeInformation.Type == Constants.ResolverType)
                return Expression.Convert(resolutionInfo.CurrentScopeParameter, Constants.ResolverType);

            if (resolutionInfo.ParameterExpressions.Length > 0 && resolutionInfo.ParameterExpressions.Any(p => p.Type == typeInformation.Type || p.Type.Implements(typeInformation.Type)))
                return resolutionInfo.ParameterExpressions.Last(p => p.Type == typeInformation.Type || p.Type.Implements(typeInformation.Type));

            var matchingParam = injectionParameters?.FirstOrDefault(param => param.Name == typeInformation.ParameterName);
            if (matchingParam != null)
                return Expression.Constant(matchingParam.Value);

            var exprOverride = resolutionInfo.GetExpressionOverrideOrDefault(typeInformation.Type);
            if (exprOverride != null)
                return exprOverride;

            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInformation, resolutionInfo.ScopeNames);
            return registration != null ? registration.GetExpression(resolutionInfo.ChildContext ?? containerContext, resolutionInfo, typeInformation.Type) :
                this.resolverSelector.GetResolverExpression(containerContext, typeInformation, resolutionInfo);
        }

        public Expression[] BuildResolutionExpressions(IContainerContext containerContext, ResolutionInfo resolutionInfo, TypeInformation typeInformation)
        {
            var registrations = containerContext.RegistrationRepository.GetRegistrationsOrDefault(typeInformation.Type, resolutionInfo.ScopeNames)?.CastToArray();
            if (registrations == null)
                return this.resolverSelector.GetResolverExpressions(containerContext, typeInformation, resolutionInfo);

            var lenght = registrations.Length;
            var expressions = new Expression[lenght];
            for (var i = 0; i < lenght; i++)
                expressions[i] = registrations[i].Value.GetExpression(resolutionInfo.ChildContext ?? containerContext, resolutionInfo, typeInformation.Type);

            return expressions;
        }
    }
}
