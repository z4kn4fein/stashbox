using System.Linq;
using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;

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
            if (resolutionInfo.ParameterExpressions != null && resolutionInfo.ParameterExpressions.Any(p => p.Type == typeInformation.Type))
                return resolutionInfo.ParameterExpressions.Last(p => p.Type == typeInformation.Type);

            var matchingParam = injectionParameters?.FirstOrDefault(param => param.Name == typeInformation.ParameterName);
            if (matchingParam != null)
                return Expression.Constant(matchingParam.Value);

            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInformation, true);
            return registration != null ? registration.GetExpression(resolutionInfo, typeInformation) : 
                this.resolverSelector.GetResolverExpression(containerContext, typeInformation, resolutionInfo);
        }

        public Expression[] BuildResolutionExpressions(IContainerContext containerContext, ResolutionInfo resolutionInfo, TypeInformation typeInformation)
        {
            var registrations = containerContext.RegistrationRepository.GetRegistrationsOrDefault(typeInformation);
            if (registrations == null)
                return this.resolverSelector.GetResolverExpressions(containerContext, typeInformation, resolutionInfo);

            var serviceRegistrations = containerContext.ContainerConfigurator.ContainerConfiguration.EnumerableOrderRule(registrations).ToArray();
            var lenght = serviceRegistrations.Length;
            var expressions = new Expression[lenght];
            for (var i = 0; i < lenght; i++)
                expressions[i] = serviceRegistrations[i].GetExpression(resolutionInfo, typeInformation);
                
            return expressions;
        }
    }
}
