using System;
using System.Linq.Expressions;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the fluent service decorator registration api.
    /// </summary>
    public class DecoratorConfigurator<TService, TImplementation> : BaseDecoratorConfigurator<DecoratorConfigurator<TService, TImplementation>>,
        IFluentCompositor<TImplementation, DecoratorConfigurator<TService, TImplementation>>
    {
        private readonly FluentCompositor<TService, TImplementation> compositor;

        internal DecoratorConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
            this.compositor = new FluentCompositor<TService, TImplementation>(serviceType, implementationType, this.Context);
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> InjectMember<TResult>(Expression<Func<TImplementation, TResult>> expression, object dependencyName = null)
        {
            this.compositor.InjectMember(expression, dependencyName);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithFinalizer(Action<TImplementation> finalizer)
        {
            this.compositor.WithFinalizer(finalizer);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithInitializer(Action<TImplementation, IDependencyResolver> initializer)
        {
            this.compositor.WithInitializer(initializer);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithFactory(Func<TImplementation> singleFactory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(singleFactory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithFactory(Func<IDependencyResolver, TImplementation> containerFactory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(containerFactory, isCompiledLambda);
            return this;
        }
    }

    /// <summary>
    /// Represents the fluent service decorator registration api.
    /// </summary>
    public class DecoratorConfigurator : BaseDecoratorConfigurator<DecoratorConfigurator>
    {
        internal DecoratorConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        { }

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="containerFactory">The container factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public DecoratorConfigurator WithFactory(Func<IDependencyResolver, object> containerFactory, bool isCompiledLambda = false)
        {
            this.Context.ContainerFactory = containerFactory;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
            return this;
        }

        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="singleFactory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public DecoratorConfigurator WithFactory(Func<object> singleFactory, bool isCompiledLambda = false)
        {
            this.Context.SingleFactory = singleFactory;
            this.Context.IsFactoryDelegateACompiledLambda = isCompiledLambda;
            return this;
        }
    }
}
