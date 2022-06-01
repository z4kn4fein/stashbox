using Stashbox.Lifetime;
using Stashbox.Registration.ServiceRegistrations;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox.Registration.Fluent
{
    /// <summary>
    /// Represents the generic fluent service registration api.
    /// </summary>
    public class FluentServiceConfigurator<TService, TImplementation, TConfigurator> : FluentServiceConfigurator<TConfigurator>
        where TConfigurator : FluentServiceConfigurator<TService, TImplementation, TConfigurator>
        where TService : class
        where TImplementation : class, TService
    {
        internal FluentServiceConfigurator(Type serviceType, Type implementationType, object? name,
            LifetimeDescriptor lifetimeDescriptor, bool isDecorator)
            : base(serviceType, implementationType, name, lifetimeDescriptor, isDecorator)
        { }

        /// <summary>
        /// Sets a member (property / field) as a dependency that should be filled by the container.
        /// </summary>
        /// <param name="expression">The member expression.</param>
        /// <param name="dependencyName">The name of the dependency.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithDependencyBinding<TResult>(Expression<Func<TImplementation, TResult>> expression, object? dependencyName = null)
        {
            if (expression.Body is not MemberExpression memberExpression)
                throw new ArgumentException("The expression must be a member expression (Property or Field)",
                    nameof(expression));

            var registration = RegistrationFactory.EnsureComplex(this.Registration);
            registration.DependencyBindings ??= new Dictionary<object, object?>();
            registration.DependencyBindings.Add(memberExpression.Member.Name, dependencyName);
            this.Registration = registration;

            return (TConfigurator)this;

        }

        /// <summary>
        /// Sets a delegate which will be called when the container is being disposed.
        /// </summary>
        /// <param name="finalizer">The cleanup delegate.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFinalizer(Action<TImplementation> finalizer)
        {
            Shield.EnsureNotNull(finalizer, nameof(finalizer));

            var registration = RegistrationFactory.EnsureComplex(this.Registration);
            registration.Finalizer = o => finalizer((TImplementation)o);
            this.Registration = registration;

            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a delegate which will be called when the service is being constructed.
        /// </summary>
        /// <param name="initializer">The initializer delegate.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithInitializer(Action<TImplementation, IDependencyResolver> initializer)
        {
            Shield.EnsureNotNull(initializer, nameof(initializer));

            var registration = RegistrationFactory.EnsureComplex(this.Registration);
            registration.Initializer = initializer;
            this.Registration = registration;

            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets an async initializer delegate which will be called when <see cref="IDependencyResolver.InvokeAsyncInitializers"/> is called.
        /// </summary>
        /// <param name="initializer">The async initializer delegate.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithAsyncInitializer(Func<TImplementation, IDependencyResolver, CancellationToken, Task> initializer)
        {
            Shield.EnsureNotNull(initializer, nameof(initializer));

            var registration = RegistrationFactory.EnsureComplex(this.Registration);
            registration.AsyncInitializer = (o, r, t) => initializer((TImplementation)o, r, t);
            this.Registration = registration;

            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory(Func<TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a factory delegate for the registration that takes an <see cref="IDependencyResolver"/> as parameter.
        /// </summary>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory(Func<IDependencyResolver, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, Constants.ResolverType, typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1>(Func<T1, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2>(Func<T1, T2, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2, T3>(Func<T1, T2, T3, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(TImplementation));
            return (TConfigurator)this;
        }
    }

    /// <summary>
    /// Represents the fluent service registration api.
    /// </summary>
    public class FluentServiceConfigurator<TConfigurator> : BaseFluentConfigurator<TConfigurator>
        where TConfigurator : FluentServiceConfigurator<TConfigurator>
    {
        internal FluentServiceConfigurator(Type serviceType, Type implementationType, object? name,
            LifetimeDescriptor lifetimeDescriptor, bool isDecorator)
            : base(serviceType, implementationType, name, lifetimeDescriptor, isDecorator)
        { }

        /// <summary>
        /// Indicates that the service's resolution should be handled by a dynamic <see cref="IDependencyResolver.Resolve(Type)"/> call on the current <see cref="IDependencyResolver"/> instead of a pre-built instantiation expression.
        /// </summary>
        /// <returns></returns>
        public TConfigurator WithDynamicResolution()
        {
            var registration = RegistrationFactory.EnsureComplex(this.Registration);
            registration.IsResolutionCallRequired = true;
            this.Registration = registration;

            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets the metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithMetadata(object? metadata)
        {
            var registration = RegistrationFactory.EnsureComplex(this.Registration);
            registration.Metadata = metadata;
            this.Registration = registration;

            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets the name of the registration.
        /// </summary>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithName(object? name)
        {
            this.Registration.Name = name;

            return (TConfigurator)this;
        }

        /// <summary>
        /// This registration is used as a logical scope for it's dependencies. Dependencies registered with the <see cref="BaseFluentConfigurator{TConfigurator}.InNamedScope"/> with the same name will be preferred during resolution.
        /// </summary>
        /// <param name="scopeName">The name of the scope. When the name is null, the type which defines the scope is used as name.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator DefinesScope(object? scopeName = null)
        {
            var registration = RegistrationFactory.EnsureComplex(this.Registration);
            registration.DefinedScopeName = scopeName ?? this.ImplementationType;
            this.Registration = registration;

            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets a container factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The container factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory(Func<IDependencyResolver, object> factory, bool isCompiledLambda = false) =>
            this.SetFactory(factory, isCompiledLambda, Constants.ResolverType, Constants.ObjectType);

        /// <summary>
        /// Sets a parameter-less factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory(Func<object> factory, bool isCompiledLambda = false) =>
            this.SetFactory(factory, isCompiledLambda, Constants.ObjectType);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1>(Func<T1, object> factory, bool isCompiledLambda = false) =>
            this.SetFactory(factory, isCompiledLambda, typeof(T1), Constants.ObjectType);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2>(Func<T1, T2, object> factory, bool isCompiledLambda = false) =>
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), Constants.ObjectType);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2, T3>(Func<T1, T2, T3, object> factory, bool isCompiledLambda = false) =>
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), Constants.ObjectType);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object> factory, bool isCompiledLambda = false) =>
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(T4), Constants.ObjectType);

        /// <summary>
        /// Sets a parameterized factory delegate for the registration.
        /// </summary>
        /// <param name="factory">The parameterized factory delegate.</param>
        /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, object> factory, bool isCompiledLambda = false) =>
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), Constants.ObjectType);
    }
}
