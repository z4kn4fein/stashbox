using System.Linq;
using System.Linq.Expressions;
using Stashbox.BuildUp.Resolution;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;

namespace Stashbox.Resolution
{
    internal class ResolutionStrategy : IResolutionStrategy
    {
        private readonly IResolverSelector resolverSelector;

        internal ResolutionStrategy(IResolverSelector resolverSelector)
        {
            this.resolverSelector = resolverSelector;
        }

        public ResolutionTarget BuildResolutionTarget(IContainerContext containerContext, ResolutionInfo resolutionInfo, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters)
        {
            if (resolutionInfo.ParameterExpressions != null && resolutionInfo.ParameterExpressions.Any(p => p.Type == typeInformation.Type))
                return new ResolutionTarget
                {
                    TypeInformation = typeInformation,
                    Resolver = new ParameterExpressionResolver(containerContext, typeInformation, resolutionInfo.ParameterExpressions.First(p => p.Type == typeInformation.Type))
                };

            if (resolutionInfo.OverrideManager != null && resolutionInfo.OverrideManager.ContainsValue(typeInformation))
                return new ResolutionTarget
                {
                    TypeInformation = typeInformation,
                    ResolutionTargetValue = resolutionInfo.OverrideManager.GetOverriddenValue(typeInformation.Type)
                };

            var matchingParam = injectionParameters?.FirstOrDefault(param => param.Name == typeInformation.ParameterName);
            if (matchingParam != null)
                return new ResolutionTarget
                {
                    TypeInformation = typeInformation,
                    ResolutionTargetValue = matchingParam.Value
                };


            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInformation, true);
            if (registration != null)
                return new ResolutionTarget
                {
                    Resolver = new ContainerResolver(containerContext, typeInformation, registration),
                    TypeInformation = typeInformation
                };

            Resolver resolver;
            this.resolverSelector.TryChooseResolver(containerContext, typeInformation, out resolver);

            return new ResolutionTarget
            {
                Resolver = resolver,
                TypeInformation = typeInformation
            };
        }
        
        public Expression GetExpressionForResolutionTarget(ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo)
        {
            return resolutionTarget.ResolutionTargetValue != null ? 
                Expression.Constant(resolutionTarget.ResolutionTargetValue):
                    resolutionTarget.Resolver.GetExpression(resolutionInfo);
        }
    }
}
