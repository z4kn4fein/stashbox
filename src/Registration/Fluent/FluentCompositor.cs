using System;
using System.Linq.Expressions;

namespace Stashbox.Registration.Fluent
{
    internal class FluentCompositor<TService, TImplementation> : FluentServiceConfigurator<TService, TImplementation,
        FluentCompositor<TService, TImplementation>>
    {
        internal FluentCompositor(Type serviceType, Type implementationType, RegistrationContext registrationContext) : base(serviceType, implementationType, registrationContext)
        { }
    }

    internal interface IFluentCompositor<TImplementation, TConfigurator>
    {
        /// <summary>
        /// Set a member (property / field) as a dependency should be filled by the container.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator InjectMember<TResult>(Expression<Func<TImplementation, TResult>> expression, object dependencyName = null);

        /// <summary>
        /// Sets a delegate which will be called when the container is being disposed.
        /// </summary>
        /// <param name="finalizer">The cleanup delegate.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithFinalizer(Action<TImplementation> finalizer);

        /// <summary>
        /// Sets a delegate which will be called when the service is being constructed.
        /// </summary>
        /// <param name="initializer">The initializer delegate.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithInitializer(Action<TImplementation, IDependencyResolver> initializer);

        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="singleFactory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithFactory(Func<TImplementation> singleFactory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="containerFactory">The container factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        TConfigurator WithFactory(Func<IDependencyResolver, TImplementation> containerFactory, bool isCompiledLambda = false);
    }
}
