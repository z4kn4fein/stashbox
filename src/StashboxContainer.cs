using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data;
using Stashbox.Utils.Data.Concurrent;

namespace Stashbox;

/// <summary>
/// Represents the Stashbox dependency injection container.
/// </summary>
public sealed partial class StashboxContainer : IStashboxContainer
{
    private int disposed;
    private readonly ContainerConfigurator containerConfigurator;
    private readonly ResolutionScope rootScope;
    private readonly Action<IStashboxContainer>? onDisposed;
    private readonly ConcurrentList<IStashboxContainer> childContainers = new();

#if NET46_OR_GREATER
    /// <inheritdoc/>
    public IReadOnlyCollection<IStashboxContainer> ChildContainers => childContainers;
#else
    /// <inheritdoc/>
    public IEnumerable<IStashboxContainer> ChildContainers => childContainers;
#endif

    /// <summary>
    /// Constructs a <see cref="StashboxContainer"/>.
    /// </summary>
    public StashboxContainer(Action<ContainerConfigurator>? config = null)
        : this(null, new ResolutionStrategy(), new ContainerConfigurator(), config, null)
    { }

    private StashboxContainer(IStashboxContainer? parentContainer, IResolutionStrategy resolutionStrategy,
        ContainerConfigurator containerConfigurator, Action<ContainerConfigurator>? config = null,
        Action<IStashboxContainer>? onDisposed = null)
    {
        this.ContainerContext = new ContainerContext(parentContainer?.ContainerContext, resolutionStrategy, containerConfigurator.ContainerConfiguration);
        this.rootScope = (ResolutionScope)this.ContainerContext.RootScope;
        this.containerConfigurator = containerConfigurator;
        this.onDisposed = onDisposed;

        config?.Invoke(this.containerConfigurator);
    }

    /// <inheritdoc />
    public IContainerContext ContainerContext { get; }

    /// <inheritdoc />
    public void RegisterResolver(IResolver resolver)
    {
        this.ThrowIfDisposed();
        Shield.EnsureNotNull(resolver, nameof(resolver));

        this.ContainerContext.ResolutionStrategy.RegisterResolver(resolver);
    }

    /// <inheritdoc />
    public bool IsRegistered<TFrom>(object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default) =>
        this.IsRegistered(TypeCache<TFrom>.Type, name, resolutionBehavior);

    /// <inheritdoc />
    public bool IsRegistered(Type typeFrom, object? name = null, ResolutionBehavior resolutionBehavior = ResolutionBehavior.Default)
    {
        this.ThrowIfDisposed();
        Shield.EnsureNotNull(typeFrom, nameof(typeFrom));

        return this.ContainerContext.RegistrationRepository.ContainsRegistration(typeFrom, name, false);
    }

    /// <inheritdoc />
    public void Validate()
    {
        this.ThrowIfDisposed();

        var exceptions = new ExpandableArray<Exception>();

        foreach (var serviceRegistration in this.ContainerContext.RegistrationRepository
                     .GetRegistrationMappings()
                     .Where(reg => !reg.Key.IsOpenGenericType()))
        {
            try
            {
                this.ContainerContext.ResolutionStrategy.BuildExpressionForRegistration(serviceRegistration.Value,
                    ResolutionContext.BeginValidationContext(this.ContainerContext),
                    new TypeInformation(serviceRegistration.Key, serviceRegistration.Value.Name));
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Length > 0)
            throw new AggregateException("Container validation failed. See the inner exceptions for details.", exceptions);
    }

    /// <inheritdoc />
    public IStashboxContainer CreateChildContainer(Action<ContainerConfigurator>? config = null)
    {
        this.ThrowIfDisposed();

        var childContainer = new StashboxContainer(this, this.ContainerContext.ResolutionStrategy,
            new ContainerConfigurator(this.ContainerContext.ContainerConfiguration.Clone()), config,
            x =>
            {
                if (disposed != 1)
                    childContainers.Remove(x);
            });

        childContainers.Add(childContainer);

        return childContainer;
    }

    /// <inheritdoc />
    public void Configure(Action<ContainerConfigurator> config)
    {
        this.ThrowIfDisposed();
        Shield.EnsureNotNull(config, "The config parameter cannot be null!");

        config.Invoke(this.containerConfigurator);
        this.containerConfigurator.ContainerConfiguration.ConfigurationChangedEvent?
            .Invoke(this.containerConfigurator.ContainerConfiguration);

        this.ContainerContext.RootScope.InvalidateDelegateCache();
    }

    /// <inheritdoc />
    public IEnumerable<KeyValuePair<Type, ServiceRegistration>> GetRegistrationMappings()
    {
        this.ThrowIfDisposed();

        return this.ContainerContext.RegistrationRepository.GetRegistrationMappings();
    }

    /// <inheritdoc />
    public IEnumerable<RegistrationDiagnosticsInfo> GetRegistrationDiagnostics()
    {
        this.ThrowIfDisposed();

        foreach (var reg in this.ContainerContext.RegistrationRepository.GetRegistrationMappings())
            yield return new RegistrationDiagnosticsInfo(reg.Key, reg.Value.ImplementationType, reg.Value.Name);
    }

    private void ThrowIfDisposed([CallerMemberName] string caller = "<unknown>")
    {
        if (this.disposed == 1)
            Shield.ThrowDisposedException(this.GetType().FullName, caller);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
        {
            return;
        }

        if (ContainerContext.ContainerConfiguration.DisposeChildContainers)
        {
            foreach (var childContainer in childContainers)
            {
                childContainer.Dispose();
            }
        }

        this.ContainerContext.RootScope.Dispose();
        childContainers.Clear();

        onDisposed?.Invoke(this);
    }

#if HAS_ASYNC_DISPOSABLE
    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
        {
            return;
        }

        if (ContainerContext.ContainerConfiguration.DisposeChildContainers)
        {
            foreach (var childContainer in childContainers)
            {
                await childContainer.DisposeAsync();
            }
        }

        await this.ContainerContext.RootScope.DisposeAsync();
        childContainers.Clear();

        onDisposed?.Invoke(this);
    }
#endif
}