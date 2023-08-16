using Stashbox.Configuration;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Stashbox.Utils.Data.Immutable;

namespace Stashbox;

/// <summary>
/// Represents the Stashbox dependency injection container.
/// </summary>
public sealed partial class StashboxContainer : IStashboxContainer
{
    private sealed class ChildContainerStore
    {
        public ImmutableTree<object, IStashboxContainer> ChildContainers =
            ImmutableTree<object, IStashboxContainer>.Empty;
    }

    private static int globalContainerId = int.MinValue;

    private int disposed;
    private readonly ChildContainerStore childContainerStore;
    private readonly ChildContainerStore? directParentChildContainerStore;
    private readonly ContainerConfigurator containerConfigurator;
    private readonly ResolutionScope rootScope;
    private readonly object containerId;
    private readonly bool shouldDisposeWithParent;

    /// <summary>
    /// Constructs a <see cref="StashboxContainer"/>.
    /// </summary>
    public StashboxContainer(Action<ContainerConfigurator>? config = null)
        : this(null, new ResolutionStrategy(), new ContainerConfigurator(), ReserveContainerId(), false, config)
    {
    }

    private StashboxContainer(StashboxContainer? parentContainer, IResolutionStrategy resolutionStrategy,
        ContainerConfigurator containerConfigurator, object containerId, bool shouldDisposeWithParent,
        Action<ContainerConfigurator>? config = null)
    {
        this.childContainerStore = new ChildContainerStore();
        this.directParentChildContainerStore = parentContainer?.childContainerStore;
        this.ContainerContext = new ContainerContext(parentContainer?.ContainerContext, resolutionStrategy,
            containerConfigurator.ContainerConfiguration);
        this.rootScope = (ResolutionScope)this.ContainerContext.RootScope;
        this.containerConfigurator = containerConfigurator;
        this.containerId = containerId;
        this.shouldDisposeWithParent = shouldDisposeWithParent;
        config?.Invoke(this.containerConfigurator);
    }

    /// <inheritdoc />
    public IContainerContext ContainerContext { get; }

    /// <inheritdoc />
    public IEnumerable<ReadOnlyKeyValue<object, IStashboxContainer>> ChildContainers =>
        this.childContainerStore.ChildContainers.Walk();

    /// <inheritdoc />
    public void RegisterResolver(IResolver resolver)
    {
        this.ThrowIfDisposed();
        Shield.EnsureNotNull(resolver, nameof(resolver));

        this.ContainerContext.ResolutionStrategy.RegisterResolver(resolver);
    }

    /// <inheritdoc />
    public bool IsRegistered<TFrom>(object? name = null) =>
        this.IsRegistered(TypeCache<TFrom>.Type, name);

    /// <inheritdoc />
    public bool IsRegistered(Type typeFrom, object? name = null)
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
                    ResolutionContext.BeginValidationContext(this.ContainerContext, ResolutionBehavior.Default),
                    new TypeInformation(serviceRegistration.Key, serviceRegistration.Value.Name));
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        foreach (var child in this.childContainerStore.ChildContainers.Walk())
        {
            try
            {
                child.Value.Validate();
            }
            catch (AggregateException ex)
            {
                exceptions.Add(new AggregateException(
                    $"Child container validation failed for '{child.Key}'. See the inner exceptions for details.",
                    ex.InnerExceptions));
            }
        }

        if (exceptions.Length > 0)
            throw new AggregateException("Container validation failed. See the inner exceptions for details.",
                exceptions);
    }

    /// <inheritdoc />
    public IStashboxContainer CreateChildContainer(Action<ContainerConfigurator>? config = null,
        bool attachToParent = true) =>
        this.CreateChildContainer(ReserveContainerId(), config == null ? null : cont => cont.Configure(config),
            attachToParent);

    /// <inheritdoc />
    public IStashboxContainer CreateChildContainer(object identifier, Action<IStashboxContainer>? config = null,
        bool attachToParent = true)
    {
        this.ThrowIfDisposed();

        var child = new StashboxContainer(this, this.ContainerContext.ResolutionStrategy,
            new ContainerConfigurator(this.ContainerContext.ContainerConfiguration.Clone()), identifier,
            attachToParent);

        config?.Invoke(child);

        Swap.SwapValue(ref this.childContainerStore.ChildContainers, (t1, t2, _, _, childStore) =>
                childStore.AddOrUpdate(t1, t2, false, false),
            identifier, child, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

        return child;
    }

    /// <inheritdoc />
    public IStashboxContainer? GetChildContainer(object identifier) =>
        this.childContainerStore.ChildContainers.GetOrDefaultByValue(identifier);

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
            return;

        this.RemoveSelfFromParentChildContainers();
        this.DisposeChildContainers();
        this.rootScope.Dispose();
    }

#if HAS_ASYNC_DISPOSABLE
    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
            return;
        
        this.RemoveSelfFromParentChildContainers();
        await this.DisposeChildContainersAsync().ConfigureAwait(false);
        await this.rootScope.DisposeAsync().ConfigureAwait(false);
    }
    
    private async ValueTask DisposeChildContainersAsync()
    {
        foreach (var childContainer in this.childContainerStore.ChildContainers.Walk())
        {
            if (childContainer.Value is StashboxContainer { shouldDisposeWithParent: true } container)
                await container.DisposeAsync().ConfigureAwait(false);
        }
    }
#endif

    private void DisposeChildContainers()
    {
        foreach (var childContainer in this.childContainerStore.ChildContainers.Walk())
        {
            if (childContainer.Value is StashboxContainer { shouldDisposeWithParent: true } container)
                container.Dispose();
        }
    }
    
    private void RemoveSelfFromParentChildContainers()
    {
        if (this.directParentChildContainerStore != null)
            Swap.SwapValue(ref this.directParentChildContainerStore.ChildContainers, (t1, _, _, _, childRepo) =>
                    childRepo.Remove(t1, false),
                this.containerId, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder,
                Constants.DelegatePlaceholder);
    }

    private static int ReserveContainerId() =>
        Interlocked.Increment(ref globalContainerId);
}