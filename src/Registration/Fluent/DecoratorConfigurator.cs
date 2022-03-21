using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the fluent service decorator registration api.
    /// </summary>
    public class DecoratorConfigurator<TService, TImplementation> : BaseDecoratorConfigurator<DecoratorConfigurator<TService, TImplementation>>,
        IFluentCompositor<TImplementation, DecoratorConfigurator<TService, TImplementation>>
        where TService : class
        where TImplementation : class, TService
    {
        private readonly FluentCompositor<TService, TImplementation> compositor;

        internal DecoratorConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
            this.compositor = new FluentCompositor<TService, TImplementation>(serviceType, implementationType, this.Context);
        }

        /// <inheritdoc />
        public DecoratorConfigurator<TService, TImplementation> WithDependencyBinding<TResult>(Expression<Func<TImplementation, TResult>> expression, object? dependencyName = null)
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
        public DecoratorConfigurator<TService, TImplementation> WithAsyncInitializer(Func<TImplementation, IDependencyResolver, CancellationToken, Task> initializer)
        {
            this.compositor.WithAsyncInitializer(initializer);
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
    public class DecoratorConfigurator : BaseDecoratorConfigurator<DecoratorConfigurator>, IFluentCompositor<DecoratorConfigurator>
    {
        private readonly FluentCompositor compositor;

        internal DecoratorConfigurator(Type serviceType, Type implementationType) : base(serviceType, implementationType)
        {
            this.compositor = new FluentCompositor(serviceType, implementationType, this.Context);
        }

        /// <inheritdoc />
        public DecoratorConfigurator WithFactory(Func<IDependencyResolver, object> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator WithFactory(Func<object> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator WithFactory<T1>(Func<T1, object> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator WithFactory<T1, T2>(Func<T1, T2, object> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator WithFactory<T1, T2, T3>(Func<T1, T2, T3, object> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }

        /// <inheritdoc />
        public DecoratorConfigurator WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, object> factory, bool isCompiledLambda = false)
        {
            this.compositor.WithFactory(factory, isCompiledLambda);
            return this;
        }
    }
}
