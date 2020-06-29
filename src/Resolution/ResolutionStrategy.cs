using Stashbox.Expressions;
using Stashbox.Resolution.Extensions;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    internal class ResolutionStrategy : IResolutionStrategy
    {
        private readonly ExpressionBuilder expressionBuilder;
        private ImmutableBucket<IResolver> resolverRepository = ImmutableBucket<IResolver>.Empty;
        private ImmutableBucket<IResolver> lastChanceResolverRepository = ImmutableBucket<IResolver>.Empty;

        public ResolutionStrategy(ExpressionBuilder expressionBuilder)
        {
            this.expressionBuilder = expressionBuilder;
        }

        public Expression BuildExpressionForType(ResolutionContext resolutionContext, TypeInformation typeInformation)
        {
            if (typeInformation.Type == Constants.ResolverType)
                return resolutionContext.CurrentScopeParameter;

#if HAS_SERVICEPROVIDER
            if (typeInformation.Type == Constants.ServiceProviderType)
                return resolutionContext.CurrentScopeParameter;
#endif
            if (resolutionContext.ParameterExpressions.Length > 0)
            {
                var type = typeInformation.Type;
                var length = resolutionContext.ParameterExpressions.Length;
                for (var i = length; i-- > 0;)
                {
                    var parameters = resolutionContext.ParameterExpressions[i]
                        .WhereOrDefault(p => p.I2.Type == type ||
                                             p.I2.Type.Implements(type));

                    if (parameters == null) continue;
                    var selected = parameters.FirstOrDefault(parameter => !parameter.I1) ?? parameters.Last();
                    selected.I1 = true;
                    return selected.I2;
                }
            }

            if (resolutionContext.DecoratingService.Key == typeInformation.Type)
                return resolutionContext.DecoratingService.Value;

            var exprOverride = resolutionContext.GetExpressionOverrideOrDefault(typeInformation.Type, typeInformation.DependencyName);
            if (exprOverride != null)
                return exprOverride;

            var registration = resolutionContext
                .CurrentContainerContext
                .RegistrationRepository
                .GetRegistrationOrDefault(typeInformation, resolutionContext);

            return registration != null
                ? this.expressionBuilder.BuildExpressionAndApplyLifetime(registration, resolutionContext, typeInformation.Type)
                : this.BuildResolutionExpressionUsingResolvers(typeInformation, resolutionContext);
        }

        public IEnumerable<Expression> BuildExpressionsForEnumerableRequest(ResolutionContext resolutionContext, TypeInformation typeInformation)
        {
            var registrations = resolutionContext
                .CurrentContainerContext
                .RegistrationRepository
                .GetRegistrationsOrDefault(typeInformation, resolutionContext)?.CastToArray();

            if (registrations == null)
                return this.BuildAllResolverExpressionsUsingResolvers(typeInformation, resolutionContext);

            var length = registrations.Length;
            var expressions = new Expression[length];
            for (var i = 0; i < length; i++)
                expressions[i] = this.expressionBuilder.BuildExpressionAndApplyLifetime(registrations[i], resolutionContext, typeInformation.Type);

            return expressions;
        }

        public Expression BuildExpressionForTopLevelRequest(Type type, object name, ResolutionContext resolutionContext)
        {
            if (type == Constants.ResolverType)
                return resolutionContext.CurrentScopeParameter;

#if HAS_SERVICEPROVIDER
            if (type == Constants.ServiceProviderType)
                return resolutionContext.CurrentScopeParameter;
#endif

            var exprOverride = resolutionContext.GetExpressionOverrideOrDefault(type, name);
            if (exprOverride != null)
                return exprOverride;

            var registration = resolutionContext
                .CurrentContainerContext
                .RegistrationRepository
                .GetRegistrationOrDefault(type, resolutionContext, name);

            return registration != null ? this.expressionBuilder.BuildExpressionAndApplyLifetime(registration, resolutionContext, type) :
                this.BuildResolutionExpressionUsingResolvers(new TypeInformation(type, name), resolutionContext);
        }

        public Expression BuildResolutionExpressionUsingResolvers(TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            var expression = this.resolverRepository.BuildResolutionExpression(typeInfo, resolutionContext, this);
            if (expression != null) return expression;

            return this.lastChanceResolverRepository.BuildResolutionExpression(typeInfo,
                resolutionContext, this);
        }

        public bool CanResolveType(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            this.resolverRepository.CanResolve(typeInfo, resolutionContext) ||
            this.lastChanceResolverRepository.CanResolve(typeInfo, resolutionContext);

        public void RegisterResolver(IResolver resolver) =>
            Swap.SwapValue(ref this.resolverRepository, (t1, t2, t3, t4, repo) =>
               repo.Add(t1), resolver, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

        public void RegisterLastChanceResolver(IResolver resolver) =>
            Swap.SwapValue(ref this.lastChanceResolverRepository, (t1, t2, t3, t4, repo) =>
                repo.Add(t1), resolver, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

        private IEnumerable<Expression> BuildAllResolverExpressionsUsingResolvers(TypeInformation typeInfo, ResolutionContext resolutionContext) =>
            this.resolverRepository.BuildAllResolutionExpressions(typeInfo, resolutionContext, this) ??
                   this.lastChanceResolverRepository.BuildAllResolutionExpressions(typeInfo, resolutionContext, this);

    }
}
