using Stashbox.Exceptions;
using Stashbox.Expressions;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a lifetime descriptor.
    /// </summary>
    public abstract class LifetimeDescriptor
    {

        private protected virtual bool StoreResultInLocalVariable { get; } = false;

        /// <summary>
        /// An indicator used to validate the lifetime configuration of the resolution tree.
        /// Services with longer life-span shouldn't contain dependencies with shorter ones.
        /// </summary>
        protected virtual int LifeSpan { get; } = 0;

        /// <summary>
        /// The name of the lifetime, used only for diagnostic reasons.
        /// </summary>
        protected string Name { get; }

        /// <summary>
        /// Constructs the lifetime descriptor.
        /// </summary>
        protected LifetimeDescriptor()
        {
            this.Name = this.GetType().Name;
        }

        internal Expression ApplyLifetime(ExpressionBuilder expressionBuilder, ServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type requestedType)
        {
            if (resolutionContext.CurrentContainerContext.ContainerConfiguration.LifetimeValidationEnabled &&
                this.LifeSpan > 0)
            {
                if (resolutionContext.CurrentLifeSpan > this.LifeSpan)
                    throw new LifetimeValidationFailedException(serviceRegistration.ImplementationType,
                        $"The life-span of {serviceRegistration.ImplementationType} ({this.Name}|{this.LifeSpan}) " +
                        $"is shorter than its direct or indirect parent's {resolutionContext.NameOfServiceLifeSpanValidatingAgainst}." + Environment.NewLine +
                        "This could lead to incidental lifetime promotions with longer life-span, it's recommended to double check your lifetime configurations.");

                resolutionContext = resolutionContext.BeginLifetimeValidationContext(this.LifeSpan,
                    $"{serviceRegistration.ImplementationType} ({this.Name}|{this.LifeSpan})");
            }

            if (!this.StoreResultInLocalVariable)
                return this.BuildLifetimeAppliedExpression(expressionBuilder,
                    serviceRegistration, resolutionContext, requestedType);

            var variable = resolutionContext.GetKnownVariableOrDefault(serviceRegistration.RegistrationId);
            if (variable != null)
                return variable;

            var resultExpression = this.BuildLifetimeAppliedExpression(expressionBuilder,
                serviceRegistration, resolutionContext, requestedType);
            if (resultExpression == null)
                return null;

            variable = requestedType.AsVariable();
            resolutionContext.AddDefinedVariable(serviceRegistration.RegistrationId, variable);
            resolutionContext.AddInstruction(variable.AssignTo(resultExpression));
            return variable;
        }

        private protected abstract Expression BuildLifetimeAppliedExpression(ExpressionBuilder expressionBuilder,
            ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type requestedType);

        private protected static Expression GetExpressionForRegistration(ExpressionBuilder expressionBuilder,
            ServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type requestedType)
        {
            if (!IsRegistrationOutputCacheable(serviceRegistration, resolutionContext))
                return expressionBuilder.BuildExpressionForRegistration(serviceRegistration, resolutionContext, requestedType);

            var expression = resolutionContext.GetCachedExpression(serviceRegistration.RegistrationId);
            if (expression != null)
                return expression;

            expression = expressionBuilder.BuildExpressionForRegistration(serviceRegistration, resolutionContext, requestedType);
            resolutionContext.CacheExpression(serviceRegistration.RegistrationId, expression);
            return expression;
        }

        private protected static bool IsRegistrationOutputCacheable(ServiceRegistration serviceRegistration, ResolutionContext resolutionContext) =>
            !serviceRegistration.IsDecorator &&
            resolutionContext.FactoryDelegateCacheEnabled &&
            resolutionContext.PerResolutionRequestCacheEnabled &&
            serviceRegistration.RegistrationType != RegistrationType.OpenGeneric;
    }
}
