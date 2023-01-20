using Stashbox.Registration;
using System;
using System.Collections.Generic;
using Stashbox.Utils;

namespace Stashbox;

public partial class StashboxContainer
{
    /// <inheritdoc />
    public IStashboxContainer RegisterFunc<TService>(Func<IDependencyResolver, TService> factory, object? name = null) =>
        this.RegisterFuncInternal(factory, TypeCache<Func<TService>>.Type, name);

    /// <inheritdoc />
    public IStashboxContainer RegisterFunc<T1, TService>(Func<T1, IDependencyResolver, TService> factory, object? name = null) =>
        this.RegisterFuncInternal(factory, TypeCache<Func<T1, TService>>.Type, name);

    /// <inheritdoc />
    public IStashboxContainer RegisterFunc<T1, T2, TService>(Func<T1, T2, IDependencyResolver, TService> factory, object? name = null) =>
        this.RegisterFuncInternal(factory, TypeCache<Func<T1, T2, TService>>.Type, name);

    /// <inheritdoc />
    public IStashboxContainer RegisterFunc<T1, T2, T3, TService>(Func<T1, T2, T3, IDependencyResolver, TService> factory, object? name = null) =>
        this.RegisterFuncInternal(factory, TypeCache<Func<T1, T2, T3, TService>>.Type, name);

    /// <inheritdoc />
    public IStashboxContainer RegisterFunc<T1, T2, T3, T4, TService>(Func<T1, T2, T3, T4, IDependencyResolver, TService> factory, object? name = null) =>
        this.RegisterFuncInternal(factory, TypeCache<Func<T1, T2, T3, T4, TService>>.Type, name);

    private IStashboxContainer RegisterFuncInternal(Delegate factory, Type factoryType, object? name)
    {
        this.ThrowIfDisposed();

        var registration = new ServiceRegistration(factoryType, name, this.ContainerContext.ContainerConfiguration.DefaultLifetime, false, new Dictionary<RegistrationOption, object?>
        {
            { RegistrationOption.RegistrationTypeOptions, factory }
        });
        this.ContainerContext.RegistrationRepository.AddOrUpdateRegistration(registration, factoryType);
        return this;
    }
}