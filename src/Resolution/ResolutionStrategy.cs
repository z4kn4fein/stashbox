using Stashbox.Entity;
using Stashbox.Resolution.Resolvers;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Resolution
{
    internal class ResolutionStrategy : IResolverSupportedResolutionStrategy
    {
        private ArrayStore<IMultiServiceResolver> multiServiceResolverRepository = ArrayStore<IMultiServiceResolver>.Empty;
        private ArrayStore<IResolver> resolverRepository = ArrayStore<IResolver>.Empty;
        private readonly UnknownTypeResolver unknownTypeResolver = new UnknownTypeResolver();
        private readonly ParentContainerResolver parentContainerResolver = new ParentContainerResolver();

        public Expression BuildResolutionExpression(IContainerContext containerContext, ResolutionContext resolutionContext, TypeInformation typeInformation,
            IEnumerable<InjectionParameter> injectionParameters = null, bool forceSkipUnknownTypeCheck = false)
        {
            if (typeInformation.Type == Constants.ResolverType)
                return resolutionContext.CurrentScopeParameter.ConvertTo(Constants.ResolverType);

            if (resolutionContext.ParameterExpressions.Length > 0)
            {
                var length = resolutionContext.ParameterExpressions.Length;
                for (var i = length; i-- > 0;)
                {
                    var parameters = resolutionContext.ParameterExpressions[i]
                        .WhereOrDefault(p => p.Value.Type == typeInformation.Type ||
                                        p.Value.Type.Implements(typeInformation.Type));

                    if (parameters == null) continue;
                    var selected = parameters.Repository.FirstOrDefault(parameter => !parameter.Key) ?? parameters.Repository.Last();
                    selected.Key = true;
                    return selected.Value;
                }
            }

            var matchingParam = injectionParameters?.FirstOrDefault(param => param.Name == typeInformation.ParameterOrMemberName);
            if (matchingParam != null)
            {
                if (matchingParam.Value == null)
                    return typeInformation.Type == Constants.ObjectType
                        ? matchingParam.Value.AsConstant()
                        : matchingParam.Value.AsConstant().ConvertTo(typeInformation.Type);

                return matchingParam.Value.GetType() == typeInformation.Type
                    ? matchingParam.Value.AsConstant()
                    : matchingParam.Value.AsConstant().ConvertTo(typeInformation.Type);
            }


            var exprOverride = resolutionContext.GetExpressionOverrideOrDefault(typeInformation.Type);
            if (exprOverride != null)
                return exprOverride;

            var registration = containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInformation, resolutionContext);
            return registration != null ? registration.GetExpression(resolutionContext.ChildContext ?? containerContext, resolutionContext, typeInformation.Type) :
                this.BuildResolutionExpressionUsingResolvers(containerContext, typeInformation, resolutionContext, forceSkipUnknownTypeCheck);
        }

        public Expression[] BuildAllResolutionExpressions(IContainerContext containerContext, ResolutionContext resolutionContext, TypeInformation typeInformation)
        {
            var registrations = containerContext.RegistrationRepository.GetRegistrationsOrDefault(typeInformation.Type, resolutionContext)?.CastToArray();
            if (registrations == null)
                return this.BuildAllResolverExpressionsUsingResolvers(containerContext, typeInformation, resolutionContext);

            var lenght = registrations.Length;
            var expressions = new Expression[lenght];
            for (var i = 0; i < lenght; i++)
                expressions[i] = registrations[i].Value.GetExpression(resolutionContext.ChildContext ?? containerContext, resolutionContext, typeInformation.Type);

            return expressions;
        }

        public Expression BuildResolutionExpressionUsingResolvers(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext, bool forceSkipUnknownTypeCheck = false)
        {
            for (var i = 0; i < this.resolverRepository.Length; i++)
            {
                var item = this.resolverRepository.Get(i);
                if (item.CanUseForResolution(containerContext, typeInfo, resolutionContext))
                    return item.GetExpression(containerContext, this, typeInfo, resolutionContext);
            }

            if (this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext))
                return this.parentContainerResolver.GetExpression(containerContext, this, typeInfo, resolutionContext);

            return !forceSkipUnknownTypeCheck && this.unknownTypeResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext)
                ? this.unknownTypeResolver.GetExpression(containerContext, this, typeInfo, resolutionContext)
                : null;
        }

        public bool CanResolveType(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            for (var i = 0; i < this.resolverRepository.Length; i++)
                if (this.resolverRepository.Get(i).CanUseForResolution(containerContext, typeInfo, resolutionContext))
                    return true;

            return this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext) ||
                this.unknownTypeResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext);
        }

        public void RegisterResolver(IResolver resolver)
        {
            Swap.SwapValue(ref this.resolverRepository, (t1, t2, t3, t4, repo) =>
               repo.Add(t1), resolver, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            if (resolver is IMultiServiceResolver multiServiceResolver)
                Swap.SwapValue(ref this.multiServiceResolverRepository, (t1, t2, t3, t4, repo) =>
                    repo.Add(t1), multiServiceResolver, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
        }

        private Expression[] BuildAllResolverExpressionsUsingResolvers(IContainerContext containerContext, TypeInformation typeInfo, ResolutionContext resolutionContext)
        {
            for (var i = 0; i < this.multiServiceResolverRepository.Length; i++)
            {
                var item = this.multiServiceResolverRepository.Get(i);
                if (item.CanUseForResolution(containerContext, typeInfo, resolutionContext))
                    return item.GetAllExpressions(containerContext, this, typeInfo, resolutionContext);
            }

            return this.parentContainerResolver.CanUseForResolution(containerContext, typeInfo, resolutionContext)
                ? this.parentContainerResolver.GetAllExpressions(containerContext, this, typeInfo, resolutionContext)
                : null;
        }
    }
}
