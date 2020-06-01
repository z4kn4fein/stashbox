using Stashbox.Lifetime;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;

namespace Stashbox.Expressions
{
    internal partial class ExpressionBuilder
    {
        private readonly ExpressionFactory expressionFactory;
        private readonly ServiceRegistrator serviceRegistrator;

        internal ExpressionBuilder(ExpressionFactory expressionFactory, ServiceRegistrator serviceRegistrator)
        {
            this.expressionFactory = expressionFactory;
            this.serviceRegistrator = serviceRegistrator;
        }

        internal Expression BuildExpressionAndApplyLifetime(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType) =>
            IsOutputLifetimeManageable(serviceRegistration)
                ? serviceRegistration.RegistrationContext.Lifetime.ApplyLifetime(this, serviceRegistration, resolutionContext, resolveType)
                : this.BuildExpressionForRegistration(serviceRegistration, resolutionContext, resolveType);

        internal Expression BuildExpressionForRegistration(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (serviceRegistration.IsDecorator || resolutionContext.DecoratingType == resolveType)
                return this.BuildDisposalTrackingAndFinalizerExpression(serviceRegistration, resolutionContext, resolveType);

            var decorators = resolutionContext.CurrentContainerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType);
            if (decorators == null)
            {
                if (!resolveType.IsClosedGenericType())
                    return this.BuildDisposalTrackingAndFinalizerExpression(serviceRegistration, resolutionContext, resolveType);

                decorators = resolutionContext.CurrentContainerContext.DecoratorRepository.GetDecoratorsOrDefault(resolveType.GetGenericTypeDefinition());
                if (decorators == null)
                    return this.BuildDisposalTrackingAndFinalizerExpression(serviceRegistration, resolutionContext, resolveType);
            }

            var decoratingContext = resolutionContext.BeginDecoratingContext(resolveType);
            var expression = this.BuildDisposalTrackingAndFinalizerExpression(serviceRegistration, decoratingContext, resolveType);

            if (expression == null)
                return null;

            foreach (var decorator in decorators)
            {
                decoratingContext.SetExpressionOverride(resolveType, expression);
                expression = this.BuildExpressionForRegistration(decorator, decoratingContext, resolveType);
                if (expression == null)
                    return null;
            }

            return expression;
        }

        private Expression BuildDisposalTrackingAndFinalizerExpression(ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType)
        {
            var expression = this.BuildExpressionByRegistrationType(serviceRegistration, resolutionContext, resolveType);
            if (expression == null)
                return null;

            if (serviceRegistration.RegistrationContext.ExistingInstance == null && serviceRegistration.RegistrationContext.Finalizer != null)
                expression = BuildFinalizerExpression(expression, serviceRegistration, resolutionContext.CurrentScopeParameter);

            if (!RegistrationHoldsDisposable(resolutionContext.CurrentContainerContext, serviceRegistration) || !expression.Type.IsDisposable())
                return this.CheckRuntimeCircularDependencyExpression(expression, serviceRegistration, resolutionContext, resolveType);

            var method = Constants.AddDisposalMethod.MakeGenericMethod(expression.Type);
            return this.CheckRuntimeCircularDependencyExpression(resolutionContext.CurrentScopeParameter.CallMethod(method, expression),
                serviceRegistration, resolutionContext, resolveType);
        }

        private Expression BuildExpressionByRegistrationType(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            resolutionContext = resolutionContext.ShouldFallBackToRequestInitiatorContext
                ? resolutionContext.BeginCrossContainerContext(resolutionContext.RequestInitiatorContainerContext)
                : resolutionContext;

            switch (serviceRegistration.RegistrationType)
            {
                case RegistrationType.Factory:
                    return this.GetExpressionForFactory(serviceRegistration, resolutionContext, resolveType);

                case RegistrationType.OpenGeneric:
                    var genericType = serviceRegistration.ImplementationType.MakeGenericType(resolveType.GetGenericArguments());
                    var newRegistration = serviceRegistration.Clone(genericType, RegistrationType.Default);
                    newRegistration.RegistrationContext.Name = null;

                    this.serviceRegistrator.Register(resolutionContext.CurrentContainerContext, newRegistration, resolveType, false);
                    return this.BuildExpressionAndApplyLifetime(newRegistration, resolutionContext, resolveType);

                case RegistrationType.Instance:
                    return serviceRegistration.RegistrationContext.ExistingInstance.AsConstant();

                case RegistrationType.WireUp:
                    return this.expressionFactory.ConstructBuildUpExpression(serviceRegistration, resolutionContext,
                        serviceRegistration.RegistrationContext.ExistingInstance.AsConstant(), serviceRegistration.ImplementationType);

                case RegistrationType.Func:
                    return this.GetExpressionForFunc(serviceRegistration, resolutionContext, resolveType);

                default:
                    return this.GetExpressionForDefault(serviceRegistration, resolutionContext, resolveType);
            }
        }

        private Expression CheckRuntimeCircularDependencyExpression(Expression expression,
            ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (!resolutionContext.CurrentContainerContext.ContainerConfiguration.RuntimeCircularDependencyTrackingEnabled)
                return expression;

            var variable = resolveType.AsVariable();
            var expressions = new Expression[]
            {
                resolutionContext.CurrentScopeParameter.CallMethod(
                    Constants.CheckRuntimeCircularDependencyBarrierMethod,
                    serviceRegistration.RegistrationId.AsConstant(), resolveType.AsConstant()),
                variable.AssignTo(expression),
                resolutionContext.CurrentScopeParameter.CallMethod(
                    Constants.ResetRuntimeCircularDependencyBarrierMethod,
                    serviceRegistration.RegistrationId.AsConstant()),
                variable
            };

            return expressions.AsBlock(variable);
        }

        private static Expression BuildFinalizerExpression(Expression instanceExpression, ServiceRegistration serviceRegistration, Expression scopeExpression)
        {
            var addFinalizerMethod = Constants.AddWithFinalizerMethod.MakeGenericMethod(instanceExpression.Type);
            return scopeExpression.CallMethod(addFinalizerMethod, instanceExpression,
                serviceRegistration.RegistrationContext.Finalizer.AsConstant());
        }

        private static bool IsOutputLifetimeManageable(ServiceRegistration serviceRegistration) =>
            serviceRegistration.RegistrationType != RegistrationType.OpenGeneric &&
            serviceRegistration.RegistrationType != RegistrationType.Instance &&
            serviceRegistration.RegistrationContext.Lifetime != null;

        private static bool RegistrationHoldsDisposable(IContainerContext containerContext, ServiceRegistration serviceRegistration)
        {
            if (serviceRegistration.RegistrationContext.IsLifetimeExternallyOwned ||
                serviceRegistration.RegistrationContext.ExistingInstance != null)
                return false;

            return containerContext.ContainerConfiguration.TrackTransientsForDisposalEnabled ||
                   !(serviceRegistration.RegistrationContext.Lifetime is TransientLifetime);
        }
    }
}
