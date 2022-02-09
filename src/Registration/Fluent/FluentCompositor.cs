using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox.Registration.Fluent
{
    internal class FluentCompositor<TService, TImplementation> : FluentServiceConfigurator<TService, TImplementation,
        FluentCompositor<TService, TImplementation>>
    {
        internal FluentCompositor(Type serviceType, Type implementationType, RegistrationContext registrationContext) : base(serviceType, implementationType, registrationContext)
        { }
    }

    internal class FluentCompositor : FluentServiceConfigurator<FluentCompositor>
    {
        internal FluentCompositor(Type serviceType, Type implementationType, RegistrationContext registrationContext) : base(serviceType, implementationType, registrationContext)
        { }
    }

    internal interface IFluentCompositor<TImplementation, TConfigurator>
    {
        /// <summary>
        /// Sets a member (property / field) as a dependency that should be filled by the container.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithDependencyBinding<TResult>(Expression<Func<TImplementation, TResult>> expression, object dependencyName = null);

        /// <summary>
        /// Sets a delegate which will be called when the container is being disposed.
        /// </summary>
        /// <param name="finalizer">The cleanup delegate.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithFinalizer(Action<TImplementation> finalizer);

        /// <summary>
        /// Sets a delegate which will be called when the service is being constructed.
        /// </summary>
        /// <param name="initializer">The initializer delegate.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithInitializer(Action<TImplementation, IDependencyResolver> initializer);

        /// <summary>
        /// Sets an async initializer delegate which will be called when <see cref="IDependencyResolver.InvokeAsyncInitializers"/> is called.
        /// </summary>
        /// <param name="initializer">The async initializer delegate.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithAsyncInitializer(
            Func<TImplementation, IDependencyResolver, CancellationToken, Task> initializer);

        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithFactory(Func<TImplementation> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a factory delegate for the registration that takes an <see cref="IDependencyResolver"/> as parameter.
        /// </summary>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithFactory(Func<IDependencyResolver, TImplementation> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithFactory<T1>(Func<T1, TImplementation> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithFactory<T1, T2>(Func<T1, T2, TImplementation> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithFactory<T1, T2, T3>(Func<T1, T2, T3, TImplementation> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TImplementation> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        TConfigurator WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TImplementation> factory, bool isCompiledLambda = false);
    }

    internal interface IFluentCompositor<TConfigurator>
    {
        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The container factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory(Func<IDependencyResolver, object> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory(Func<object> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1>(Func<T1, object> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2>(Func<T1, T2, object> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2, T3>(Func<T1, T2, T3, object> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object> factory, bool isCompiledLambda = false);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, object> factory, bool isCompiledLambda = false);
    }
}
