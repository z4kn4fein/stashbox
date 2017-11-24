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

        public Expression BuildResolutionExpression(IContainerContext containerContext, ResolutionContext resolutionContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters)
        {
            if (typeInformation.Type == Constants.ResolverType)
                return resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType);

            if (resolutionContext.ParameterExpressions.Length > 0 && resolutionContext.ParameterExpressions.Any(p => p.Type == typeInformation.Type || p.Type.Implements(typeInformation.Type)))
                return resolutionContext.ParameterExpressions.Last(p => p.Type == typeInformation.Type || p.Type.Implements(typeInformation.Type));

            var matchingParam = injectionParameters?.FirstOrDefault(param => param.Name == typeInformation.ParameterName);
            if (matchingParam != null)
                return matchingParam.Value.AsConstant();

            var exprOverride = resolutionContext.GetExpressionOverrideOrDefault(typeInformation.Type);
            if (exprOverride != null)
                return exprOverride;

            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInformation, resolutionContext);
            return registration != null ? registration.GetExpression(resolutionContext.ChildContext ?? containerContext, resolutionContext, typeInformation.Type) :
                this.resolverSelector.GetResolverExpression(containerContext, typeInformation, resolutionContext);
        }

        public Expression[] BuildResolutionExpressions(IContainerContext containerContext, ResolutionContext resolutionContext, TypeInformation typeInformation)
        {
            var registrations = containerContext.RegistrationRepository.GetRegistrationsOrDefault(typeInformation.Type, resolutionContext)?.CastToArray();
            if (registrations == null)
                return this.resolverSelector.GetResolverExpressions(containerContext, typeInformation, resolutionContext);

            var lenght = registrations.Length;
            var expressions = new Expression[lenght];
            for (var i = 0; i < lenght; i++)
                expressions[i] = registrations[i].Value.GetExpression(resolutionContext.ChildContext ?? containerContext, resolutionContext, typeInformation.Type);

            return expressions;
        }
    }
}
