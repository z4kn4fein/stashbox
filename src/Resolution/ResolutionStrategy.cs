using Stashbox.Expressions;
using Stashbox.Registration;
using Stashbox.Resolution.Extensions;
using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Stashbox.Lifetime;

namespace Stashbox.Resolution
{
    internal class ResolutionStrategy : IResolutionStrategy
    {
        private ImmutableBucket<IResolver> resolverRepository = ImmutableBucket<IResolver>.Empty;
        private ImmutableBucket<IResolver> lastChanceResolverRepository = ImmutableBucket<IResolver>.Empty;

        public Expression BuildExpressionForType(ResolutionContext resolutionContext, TypeInformation typeInformation)
        {
            if (typeInformation.Type == Constants.ResolverType || typeInformation.Type == Constants.ServiceProviderType)
                return resolutionContext.CurrentScopeParameter;

            if (typeInformation.Type == Constants.ResolutionContextType)
                return resolutionContext.AsConstant();

            if (!resolutionContext.IsTopRequest)
            {
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
                        var selected = parameters.FirstOrDefault(parameter => !parameter.I1) ?? parameters[parameters.Length - 1];
                        selected.I1 = true;
                        return selected.I2;
                    }
                }

                var decorators = resolutionContext.RemainingDecorators.GetOrDefaultByRef(typeInformation.Type);
                if (decorators is { Length: > 0 })
                    return BuildExpressionForDecorator(decorators.Front(),
                        resolutionContext.BeginDecoratingContext(typeInformation.Type, decorators), typeInformation.Type, decorators);
            }

            var exprOverride = resolutionContext.GetExpressionOverrideOrDefault(typeInformation.Type, typeInformation.DependencyName);
            if (exprOverride != null)
                return exprOverride;

            var registration = resolutionContext
                .CurrentContainerContext
                .RegistrationRepository
                .GetRegistrationOrDefault(typeInformation, resolutionContext);

            resolutionContext.IsTopRequest = false;
            return registration != null
                ? this.BuildExpressionForRegistration(registration, resolutionContext, typeInformation)
                : this.BuildResolutionExpressionUsingResolvers(typeInformation, resolutionContext);
        }

        public IEnumerable<Expression> BuildExpressionsForEnumerableRequest(ResolutionContext resolutionContext, TypeInformation typeInformation)
        {
            var registrations = resolutionContext
                .CurrentContainerContext
                .RegistrationRepository
                .GetRegistrationsOrDefault(typeInformation, resolutionContext);

            if (registrations == null)
                return this.BuildAllResolverExpressionsUsingResolvers(typeInformation, resolutionContext);

            return registrations.Select(reg =>
            {
                var decorators = resolutionContext.RemainingDecorators.GetOrDefaultByRef(typeInformation.Type);
                if (decorators == null || decorators.Length == 0)
                    return this.BuildExpressionForRegistration(reg, resolutionContext, typeInformation);

                decorators.ReplaceBack(reg);
                return BuildExpressionForDecorator(decorators.Front(),
                    resolutionContext.BeginDecoratingContext(typeInformation.Type, decorators),
                    typeInformation.Type, decorators);
            });
        }

        public Expression BuildExpressionForRegistration(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, TypeInformation typeInformation)
        {
            var requestedType = typeInformation.Type;
            if (serviceRegistration is OpenGenericRegistration openGenericRegistration)
                serviceRegistration = openGenericRegistration.ProduceClosedRegistration(requestedType);

            var decorators = resolutionContext
                .CurrentContainerContext
                .DecoratorRepository
                .GetDecoratorsOrDefault(serviceRegistration.ImplementationType, typeInformation, resolutionContext);

            if (decorators == null)
                return BuildExpressionAndApplyLifetime(serviceRegistration, resolutionContext, requestedType);

            var stack = decorators.AsStack();
            stack.PushBack(serviceRegistration);
            return BuildExpressionForDecorator(stack.Front(),
                resolutionContext.BeginDecoratingContext(requestedType, stack), requestedType, stack);
        }

        private Expression BuildResolutionExpressionUsingResolvers(TypeInformation typeInformation, ResolutionContext resolutionContext)
        {
            var expression = this.resolverRepository.BuildResolutionExpression(this, typeInformation, resolutionContext);
            return expression ?? this.lastChanceResolverRepository.BuildResolutionExpression(this, typeInformation, resolutionContext);
        }

        public bool IsTypeResolvable(ResolutionContext resolutionContext, TypeInformation typeInformation)
        {
            if (typeInformation.Type.IsGenericTypeDefinition)
                return false;

            if (typeInformation.Type == Constants.ResolverType ||
                typeInformation.Type == Constants.ServiceProviderType ||
                typeInformation.Type == Constants.ResolutionContextType)
                return true;

            if (resolutionContext.CurrentContainerContext.RegistrationRepository.ContainsRegistration(typeInformation.Type, typeInformation.DependencyName) ||
                this.resolverRepository.IsWrappedTypeRegistered(typeInformation, resolutionContext))
                return true;

            var exprOverride = resolutionContext.GetExpressionOverrideOrDefault(typeInformation.Type, typeInformation.DependencyName);
            return exprOverride != null || this.lastChanceResolverRepository.CanLookupService(typeInformation, resolutionContext);
        }

        public void RegisterResolver(IResolver resolver) =>
            Swap.SwapValue(ref this.resolverRepository, (t1, _, _, _, repo) =>
               repo.Add(t1), resolver, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

        public void RegisterLastChanceResolver(IResolver resolver) =>
            Swap.SwapValue(ref this.lastChanceResolverRepository, (t1, _, _, _, repo) =>
                repo.Add(t1), resolver, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

        private IEnumerable<Expression> BuildAllResolverExpressionsUsingResolvers(TypeInformation typeInfo,
            ResolutionContext resolutionContext) =>
            this.resolverRepository.BuildAllResolutionExpressions(this, typeInfo, resolutionContext) ??
            this.lastChanceResolverRepository.BuildAllResolutionExpressions(this, typeInfo, resolutionContext);

        private static Expression BuildExpressionForDecorator(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type requestedType, Utils.Data.Stack<ServiceRegistration> decorators)
        {
            if (serviceRegistration is OpenGenericRegistration openGenericRegistration)
                serviceRegistration = openGenericRegistration.ProduceClosedRegistration(requestedType);

            return BuildExpressionAndApplyLifetime(serviceRegistration, resolutionContext,
                requestedType, decorators.PeekBack()?.RegistrationContext.Lifetime);
        }

        private static Expression BuildExpressionAndApplyLifetime(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type requestedType, LifetimeDescriptor secondaryLifetimeDescriptor = null)
        {
            var lifetimeDescriptor = serviceRegistration.RegistrationContext.Lifetime ?? secondaryLifetimeDescriptor;
            if (!IsOutputLifetimeManageable(serviceRegistration) || lifetimeDescriptor == null)
                return ExpressionBuilder.BuildExpressionForRegistration(serviceRegistration, resolutionContext, requestedType);

            return lifetimeDescriptor.ApplyLifetime(serviceRegistration, resolutionContext, requestedType);
        }

        private static bool IsOutputLifetimeManageable(ServiceRegistration serviceRegistration) =>
            serviceRegistration.RegistrationType != RegistrationType.OpenGeneric &&
            serviceRegistration.RegistrationType != RegistrationType.Instance;
    }
}
