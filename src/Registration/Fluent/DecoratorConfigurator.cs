using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox.Registration.Fluent;

/// <summary>
/// Represents the fluent service decorator registration api.
/// </summary>
public class DecoratorConfigurator<TService, TImplementation> : BaseDecoratorConfigurator<DecoratorConfigurator<TService, TImplementation>>
    where TService : class
    where TImplementation : class, TService
{
    internal DecoratorConfigurator(Type serviceType, Type implementationType, object? name = null)
        : base(serviceType, implementationType, name)
    { }

    /// <summary>
    /// Sets a member (property / field) as a dependency that should be filled by the container.
    /// </summary>
    /// <param name="expression">The member expression.</param>
    /// <param name="dependencyName">The name of the dependency.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithDependencyBinding<TResult>(Expression<Func<TImplementation, TResult>> expression, object? dependencyName = null)
    {
        if (expression.Body is not MemberExpression memberExpression)
            throw new ArgumentException("The expression must be a member expression (Property or Field)",
                nameof(expression));

        this.Options ??= new Dictionary<RegistrationOption, object?>();
        if (this.Options.TryGetValue(RegistrationOption.DependencyBindings, out var value) && value is Dictionary<object, object?> bindings)
            bindings.Add(memberExpression.Member.Name, dependencyName);
        else
            this.Options[RegistrationOption.DependencyBindings] = new Dictionary<object, object?> { { memberExpression.Member.Name, dependencyName } };

        return this;
    }

    /// <summary>
    /// Sets a delegate which will be called when the container is being disposed.
    /// </summary>
    /// <param name="finalizer">The cleanup delegate.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithFinalizer(Action<TImplementation> finalizer)
    {
        Shield.EnsureNotNull(finalizer, nameof(finalizer));

        this.Options ??= new Dictionary<RegistrationOption, object?>();
        this.Options[RegistrationOption.Finalizer] = new Action<object>(o => finalizer((TImplementation)o));

        return this;
    }

    /// <summary>
    /// Sets a delegate which will be called when the service is being constructed.
    /// </summary>
    /// <param name="initializer">The initializer delegate.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithInitializer(Action<TImplementation, IDependencyResolver> initializer)
    {
        Shield.EnsureNotNull(initializer, nameof(initializer));

        this.Options ??= new Dictionary<RegistrationOption, object?>();
        this.Options[RegistrationOption.Initializer] = initializer;

        return this;
    }

    /// <summary>
    /// Sets an async initializer delegate which will be called when <see cref="IDependencyResolver.InvokeAsyncInitializers"/> is called.
    /// </summary>
    /// <param name="initializer">The async initializer delegate.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithAsyncInitializer(Func<TImplementation, IDependencyResolver, CancellationToken, Task> initializer)
    {
        Shield.EnsureNotNull(initializer, nameof(initializer));

        this.Options ??= new Dictionary<RegistrationOption, object?>();
        this.Options[RegistrationOption.AsyncInitializer] = new Func<object, IDependencyResolver, CancellationToken, Task>((o, r, t) => initializer((TImplementation)o, r, t));

        return this;
    }

    /// <summary>
    /// Sets a parameter-less factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithFactory(Func<TImplementation> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<TImplementation>.Type);

    /// <summary>
    /// Sets a factory delegate for the registration that takes an <see cref="IDependencyResolver"/> as parameter.
    /// </summary>
    /// <param name="factory">The factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithFactory(Func<IDependencyResolver, TImplementation> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<IDependencyResolver>.Type, TypeCache<TImplementation>.Type);

    /// <summary>
    /// Sets a parameterized factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The parameterized factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithFactory<T1>(Func<T1, TImplementation> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<T1>.Type, TypeCache<TImplementation>.Type);

    /// <summary>
    /// Sets a parameterized factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The parameterized factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithFactory<T1, T2>(Func<T1, T2, TImplementation> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<T1>.Type, TypeCache<T2>.Type, TypeCache<TImplementation>.Type);

    /// <summary>
    /// Sets a parameterized factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The parameterized factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithFactory<T1, T2, T3>(Func<T1, T2, T3, TImplementation> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<T1>.Type, TypeCache<T2>.Type, TypeCache<T3>.Type, TypeCache<TImplementation>.Type);

    /// <summary>
    /// Sets a parameterized factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The parameterized factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, TImplementation> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<T1>.Type, TypeCache<T2>.Type, TypeCache<T3>.Type, TypeCache<T4>.Type, TypeCache<TImplementation>.Type);

    /// <summary>
    /// Sets a parameterized factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The parameterized factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator<TService, TImplementation> WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, TImplementation> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<T1>.Type, TypeCache<T2>.Type, TypeCache<T3>.Type, TypeCache<T4>.Type, TypeCache<T5>.Type, TypeCache<TImplementation>.Type);
}

/// <summary>
/// Represents the fluent service decorator registration api.
/// </summary>
public class DecoratorConfigurator : BaseDecoratorConfigurator<DecoratorConfigurator>
{
    internal DecoratorConfigurator(Type serviceType, Type implementationType, object? name = null)
        : base(serviceType, implementationType, name)
    { }

    /// <summary>
    /// Sets a container factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The container factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator WithFactory(Func<IDependencyResolver, object> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<IDependencyResolver>.Type, TypeCache<object>.Type);

    /// <summary>
    /// Sets a parameter-less factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator WithFactory(Func<object> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<object>.Type);

    /// <summary>
    /// Sets a parameterized factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The parameterized factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator WithFactory<T1>(Func<T1, object> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<T1>.Type, TypeCache<object>.Type);

    /// <summary>
    /// Sets a parameterized factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The parameterized factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator WithFactory<T1, T2>(Func<T1, T2, object> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<T1>.Type, TypeCache<T2>.Type, TypeCache<object>.Type);

    /// <summary>
    /// Sets a parameterized factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The parameterized factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator WithFactory<T1, T2, T3>(Func<T1, T2, T3, object> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<T1>.Type, TypeCache<T2>.Type, TypeCache<T3>.Type, TypeCache<object>.Type);

    /// <summary>
    /// Sets a parameterized factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The parameterized factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator WithFactory<T1, T2, T3, T4>(Func<T1, T2, T3, T4, object> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<T1>.Type, TypeCache<T2>.Type, TypeCache<T3>.Type, TypeCache<T4>.Type, TypeCache<object>.Type);

    /// <summary>
    /// Sets a parameterized factory delegate for the registration.
    /// </summary>
    /// <param name="factory">The parameterized factory delegate.</param>
    /// <param name="isCompiledLambda">Flag that indicates the passed factory delegate is a compiled lambda from <see cref="Expression"/>.</param>
    /// <returns>The fluent configurator.</returns>
    public DecoratorConfigurator WithFactory<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, object> factory, bool isCompiledLambda = false) =>
        this.SetFactory(factory, isCompiledLambda, TypeCache<T1>.Type, TypeCache<T2>.Type, TypeCache<T3>.Type, TypeCache<T4>.Type, TypeCache<T5>.Type, TypeCache<object>.Type);
}