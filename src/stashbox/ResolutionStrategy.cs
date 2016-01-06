using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using Stashbox.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.Overrides;

namespace Stashbox
{
    public class ResolutionStrategy : IResolutionStrategy
    {
        private readonly IResolverSelector resolverSelector;

        internal ResolutionStrategy(IResolverSelector resolverSelector)
        {
            this.resolverSelector = resolverSelector;
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters)
        {
            return this.resolverSelector.CanResolve(containerContext, typeInformation) ||
                                          (injectionParameters != null &&
                                           injectionParameters.Any(injectionParameter =>
                                           injectionParameter.Name == typeInformation.MemberName));
        }

        public ResolutionTarget BuildResolutionTarget(IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters)
        {
            Resolver resolver;
            this.resolverSelector.TryChooseResolver(containerContext, typeInformation, out resolver);

            return new ResolutionTarget
            {
                Resolver = resolver,
                TypeInformation = typeInformation,
                ResolutionTargetValue = injectionParameters?.FirstOrDefault(param => param.Name == typeInformation.MemberName)?.Value
            };
        }

        public object EvaluateResolutionTarget(ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo)
        {
            if (resolutionInfo.OverrideManager != null && resolutionInfo.OverrideManager.ContainsValue(resolutionTarget.TypeInformation))
                return resolutionInfo.OverrideManager.GetOverriddenValue(resolutionTarget.TypeInformation);
            return resolutionTarget.ResolutionTargetValue ?? resolutionTarget.Resolver.Resolve(resolutionInfo);
        }

        public Expression GetExpressionForResolutionTarget(ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            if (resolutionInfo.OverrideManager != null &&
                resolutionInfo.OverrideManager.ContainsValue(resolutionTarget.TypeInformation))
                return this.CreateOverrideExpression(resolutionTarget, resolutionInfoExpression);
            return resolutionTarget.ResolutionTargetValue != null ? Expression.Constant(resolutionTarget.ResolutionTargetValue) :
                resolutionTarget.Resolver.GetExpression(resolutionInfo, resolutionInfoExpression);
        }

        private Expression CreateOverrideExpression(ResolutionTarget resolutionTarget, Expression resolutionInfoExpression)
        {
            var overrideManagerExpression = Expression.Property(resolutionInfoExpression, "OverrideManager");
            var callExpression = Expression.Call(overrideManagerExpression, "GetOverriddenValue", null, Expression.Constant(resolutionTarget.TypeInformation));
            return Expression.Convert(callExpression, resolutionTarget.TypeInformation.Type);
        }
    }

    public class CheckParentResolutionStrategyDecorator : IResolutionStrategy
    {
        private readonly IResolutionStrategy resolutionStrategy;

        public CheckParentResolutionStrategyDecorator(IResolutionStrategy resolutionStrategy)
        {
            this.resolutionStrategy = resolutionStrategy;
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters)
        {
            return this.resolutionStrategy.CanResolve(containerContext, typeInformation, injectionParameters) ||
                   this.resolutionStrategy.CanResolve(containerContext.Container.ParentContainer.ContainerContext, typeInformation,
                       injectionParameters);
        }

        public ResolutionTarget BuildResolutionTarget(IContainerContext containerContext, TypeInformation typeInformation,
            InjectionParameter[] injectionParameters)
        {
            var target = this.resolutionStrategy.BuildResolutionTarget(containerContext, typeInformation,
                injectionParameters);

            if (target.Resolver == null && target.ResolutionTargetValue == null)
                return this.resolutionStrategy.BuildResolutionTarget(
                    containerContext.Container.ParentContainer.ContainerContext, typeInformation, injectionParameters);
            else
                return target;
        }

        public object EvaluateResolutionTarget(ResolutionTarget resolutionTarget,
            ResolutionInfo resolutionInfo)
        {
            return this.resolutionStrategy.EvaluateResolutionTarget(resolutionTarget, resolutionInfo);
        }

        public Expression GetExpressionForResolutionTarget(ResolutionTarget resolutionTarget, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression)
        {
            return this.resolutionStrategy.GetExpressionForResolutionTarget(resolutionTarget, resolutionInfo, resolutionInfoExpression);
        }
    }
}
