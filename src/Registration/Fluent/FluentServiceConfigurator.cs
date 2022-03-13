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
    public class FluentServiceConfigurator<TService, TImplementation, TConfigurator> : FluentServiceConfigurator<TConfigurator>, IFluentCompositor<TImplementation, TConfigurator>
        where TConfigurator : FluentServiceConfigurator<TService, TImplementation, TConfigurator>
        where TService : class
        where TImplementation : class, TService
    {
        internal FluentServiceConfigurator(Type serviceType, Type implementationType)
            : base(serviceType, implementationType)
        { }

        internal FluentServiceConfigurator(Type serviceType, Type implementationType, RegistrationContext registrationContext)
            : base(serviceType, implementationType, registrationContext)
        { }

        /// <inheritdoc />
        public TConfigurator WithDependencyBinding<TResult>(Expression<Func<TImplementation, TResult>> expression, object? dependencyName = null)
        {
            if (expression.Body is not MemberExpression memberExpression)
                throw new ArgumentException("The expression must be a member expression (Property or Field)",
                    nameof(expression));

            this.Context.DependencyBindings ??= new Dictionary<object, object?>();
            this.Context.DependencyBindings.Add(memberExpression.Member.Name, dependencyName);
            return (TConfigurator)this;

        }

        /// <inheritdoc />
        public TConfigurator WithFinalizer(Action<TImplementation> finalizer)
        {
            Shield.EnsureNotNull(finalizer, nameof(finalizer));

            this.Context.Finalizer = o => finalizer((TImplementation)o);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithInitializer(Action<TImplementation, IDependencyResolver> initializer)
        {
            Shield.EnsureNotNull(initializer, nameof(initializer));

            this.Context.Initializer = initializer;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithAsyncInitializer(Func<TImplementation, IDependencyResolver, CancellationToken, Task> initializer)
        {
            Shield.EnsureNotNull(initializer, nameof(initializer));

            this.Context.AsyncInitializer = (o, r, t) => initializer((TImplementation)o, r, t);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory(Func<TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory(Func<IDependencyResolver, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, Constants.ResolverType, typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory<T1>(Func<T1, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory<T1, T2>(Func<T1, T2, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory<T1, T2, T3>(Func<T1, T2, T3, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(TImplementation));
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TImplementation> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(TImplementation));
            return (TConfigurator)this;
        }
    }

    /// <summary>
    /// Represents the fluent service registration api.
    /// </summary>
    public class FluentServiceConfigurator<TConfigurator> : BaseFluentConfigurator<TConfigurator>, IFluentCompositor<TConfigurator>
        where TConfigurator : FluentServiceConfigurator<TConfigurator>
    {
        internal FluentServiceConfigurator(Type serviceType, Type implementationType)
            : base(serviceType, implementationType)
        { }

        internal FluentServiceConfigurator(Type serviceType, Type implementationType, RegistrationContext registrationContext)
            : base(serviceType, implementationType, registrationContext)
        { }

        /// <summary>
        /// Sets the metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithMetadata(object? metadata)
        {
            this.Context.Metadata = metadata;
            return (TConfigurator)this;
        }

        /// <summary>
        /// Sets the name of the registration.
        /// </summary>
        /// <param name="name">The name of the registration.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator WithName(object? name)
        {
            this.Context.Name = name;
            return (TConfigurator)this;
        }

        /// <summary>
        /// This registration is used as a logical scope for it's dependencies. Dependencies registered with the <see cref="BaseFluentConfigurator{TConfigurator}.InNamedScope"/> with the same name will be preferred during resolution.
        /// </summary>
        /// <param name="scopeName">The name of the scope. When the name is null, the type which defines the scope is used as name.</param>
        /// <returns>The fluent configurator.</returns>
        public TConfigurator DefinesScope(object? scopeName = null)
        {
            this.Context.DefinedScopeName = scopeName ?? this.ImplementationType;
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory(Func<IDependencyResolver, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, Constants.ResolverType, Constants.ObjectType);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory(Func<object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, Constants.ObjectType);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory<T1>(Func<T1, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), Constants.ObjectType);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory<T1, T2>(Func<T1, T2, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), Constants.ObjectType);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory<T1, T2, T3>(Func<T1, T2, T3, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), Constants.ObjectType);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(T4), Constants.ObjectType);
            return (TConfigurator)this;
        }

        /// <inheritdoc />
        public TConfigurator WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, object> factory, bool isCompiledLambda = false)
        {
            this.SetFactory(factory, isCompiledLambda, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), Constants.ObjectType);
            return (TConfigurator)this;
        }
    }
}
