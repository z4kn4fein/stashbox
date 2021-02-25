using Stashbox.Utils;
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
        public DecoratorConfigurator<TService, TImplementation> InjectMember<TResult>(Expression<Func<TImplementation, TResult>> expression, object dependencyName = null) =>
            this.WithDependencyBinding(expression, dependencyName);

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithDependencyBinding<TResult>(Expression<Func<TImplementation, TResult>> expression, object dependencyName = null)
        {
            this.compositor.WithDependencyBinding(expression, dependencyName);
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
        public DecoratorConfigurator<TService, TImplementation> WithFactory(Func<TImplementation> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithFactory(Func<IDependencyResolver, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithFactory<T1>(Func<T1, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithFactory<T1, T2>(Func<T1, T2, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithFactory<T1, T2, T3>(Func<T1, T2, T3, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
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
        /// <param name="factory">The container factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public DecoratorConfigurator WithFactory(Func<IDependencyResolver, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, Constants.ResolverType, Constants.ObjectType);
            return this;
        }
        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The configurator itself.</returns>
        public DecoratorConfigurator WithFactory(Func<object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, Constants.ObjectType);
            return this;
        }
    }
}
