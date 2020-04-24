using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a lifetime manager.
    /// </summary>
    public abstract class LifetimeDescriptor
    {
        /// <summary>
        /// Produces the expression which manages the lifetime of the underlying instance.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The lifetime managed instance's expression.</returns>
        public Expression GetExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType)
        {
            if (!serviceRegistration.ObjectBuilder.ResultShouldBeLifetimeManaged)
                return serviceRegistration.ObjectBuilder.GetExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

            if (serviceRegistration.IsDecorator || !this.ShouldStoreResultInLocalVariable)
                return this.GetLifetimeAppliedExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

            var variable = resolutionContext.GetKnownVariableOrDefault(serviceRegistration.RegistrationId);
            if (variable != null)
                return variable;

            var resultExpression = this.GetLifetimeAppliedExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
            if (resultExpression == null)
                return null;

            variable = resolveType.AsVariable();
            resolutionContext.AddDefinedVariable(serviceRegistration.RegistrationId, variable);
            resolutionContext.AddInstruction(variable.AssignTo(resultExpression));
            return variable;
        }

        /// <summary>
        /// Through this property derived types can indicate that their expressions should be reused as a local variable in the final expression tree or not.
        /// </summary>
        protected virtual bool ShouldStoreResultInLocalVariable { get; } = false;

        /// <summary>
        /// Derived types are using this method to apply their lifetime to the instance creation.
        /// </summary>
        /// <param name="containerContext">The container's actual context or the request initiator's context, when the request was initiated from a child container.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The lifetime managed expression, or null when it's could not be created.</returns>
        protected abstract Expression GetLifetimeAppliedExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType);

        /// <summary>
        /// Produces the expression which creates the instance managed by this <see cref="LifetimeDescriptor"/>.
        /// </summary>
        /// <param name="containerContext">The container's actual context or the request initiator's context, when the request was initiated from a child container.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The instantiation expression, or null when it's could not be created.</returns>
        protected Expression BuildExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration,
            ResolutionContext resolutionContext, Type resolveType)
        {
            if (serviceRegistration.IsDecorator)
                return serviceRegistration.ObjectBuilder.GetExpression(containerContext, serviceRegistration, resolutionContext, resolveType);

            var expression = resolutionContext.GetCachedExpression(serviceRegistration.RegistrationId);
            if (expression != null)
                return expression;

            expression = serviceRegistration.ObjectBuilder.GetExpression(containerContext, serviceRegistration, resolutionContext, resolveType);
            resolutionContext.CacheExpression(serviceRegistration.RegistrationId, expression);
            return expression;
        }

        /// <summary>
        /// Gets the factory delegate for getting the instance managed by this <see cref="LifetimeDescriptor"/>.
        /// </summary>
        /// <param name="containerContext">The container's actual context or the request initiator's context, when the request was initiated from a child container.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionContext">The info about the actual resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The factory delegate, or null when it's could not be created.</returns>
        protected Func<IResolutionScope, object> GetFactoryDelegate(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType)
        {
            if (serviceRegistration.IsDecorator)
                return this.GetNewFactoryDelegate(containerContext, serviceRegistration, resolutionContext.Clone(), resolveType);

            var factory = resolutionContext.GetCachedFactory(serviceRegistration.RegistrationId);
            if (factory != null)
                return factory;

            factory = this.GetNewFactoryDelegate(containerContext, serviceRegistration, resolutionContext.Clone(), resolveType);
            resolutionContext.CacheFactory(serviceRegistration.RegistrationId, factory);
            return factory;
        }

        private Func<IResolutionScope, object> GetNewFactoryDelegate(IContainerContext containerContext, IServiceRegistration serviceRegistration, ResolutionContext resolutionContext, Type resolveType) =>
            serviceRegistration.ObjectBuilder.GetExpression(containerContext, serviceRegistration, resolutionContext, resolveType)
                ?.CompileDelegate(resolutionContext, containerContext.ContainerConfiguration);
    }
}
