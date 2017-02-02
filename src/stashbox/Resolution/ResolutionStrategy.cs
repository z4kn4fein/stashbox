using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.BuildUp.Resolution;
using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using Stashbox.Overrides;

namespace Stashbox.Resolution
{
    internal class ResolutionStrategy : IResolutionStrategy
    {
        private readonly IResolverSelector resolverSelector;
        private readonly MethodInfo containsMethodInfo;
        private readonly MethodInfo getOverriddenValueMethodInfo;
        private readonly MethodInfo propertyAccessor;

        internal ResolutionStrategy(IResolverSelector resolverSelector)
        {
            this.resolverSelector = resolverSelector;
            var typeInfo = typeof(OverrideManager).GetTypeInfo();
            this.containsMethodInfo = typeInfo.GetDeclaredMethod("ContainsValue");
            this.getOverriddenValueMethodInfo = typeInfo.GetDeclaredMethod("GetOverriddenValue");
            this.propertyAccessor = typeof(ResolutionInfo).GetRuntimeProperty("OverrideManager").SetMethod;
        }

        public ResolutionTarget BuildResolutionTarget(IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters)
        {
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

        public object EvaluateResolutionTarget(ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo)
        {
            if (resolutionInfo.OverrideManager != null && resolutionInfo.OverrideManager.ContainsValue(resolutionTarget.TypeInformation))
                return resolutionInfo.OverrideManager.GetOverriddenValue(resolutionTarget.TypeInformation);
            return resolutionTarget.ResolutionTargetValue ?? resolutionTarget.Resolver.Resolve(resolutionInfo);
        }

        public Expression GetExpressionForResolutionTarget(ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo)
        {
            var nullExpr = Expression.Default(resolutionTarget.TypeInformation.Type);

            return resolutionTarget.ResolutionTargetValue != null ? Expression.Constant(resolutionTarget.ResolutionTargetValue) :
                     resolutionTarget.Resolver == null ? nullExpr : resolutionTarget.Resolver.GetExpression(resolutionInfo);
        }
    }
}
