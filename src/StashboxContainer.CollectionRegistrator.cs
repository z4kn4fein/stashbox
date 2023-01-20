using Stashbox.Registration.Fluent;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox;

public partial class StashboxContainer
{
    /// <inheritdoc />
    public IStashboxContainer RegisterTypesAs(Type typeFrom,
        IEnumerable<Type> types,
        Func<Type, bool>? selector = null,
        Action<RegistrationConfigurator>? configurator = null)
    {
        Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
        Shield.EnsureNotNull(types, nameof(types));

        types = selector != null ? types.Where(selector).Where(t => t.IsResolvableType()) : types.Where(t => t.IsResolvableType());

        foreach (var type in types)
        {
            if (type.ImplementsWithoutGenericCheck(typeFrom))
            {
                this.RegisterTypeAs(typeFrom, type, configurator);
                continue;
            }

            if (!typeFrom.IsGenericTypeDefinition) continue;

            var serviceTypes = type.GetRegisterableBaseTypes().Concat(type.GetRegisterableInterfaceTypes())
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeFrom);

            foreach (var service in serviceTypes)
                this.RegisterTypeAs(type.IsGenericTypeDefinition ? typeFrom : service, type, configurator);
        }

        return this;
    }

    /// <inheritdoc />
    public IStashboxContainer RegisterTypes(IEnumerable<Type> types,
        Func<Type, bool>? selector = null,
        Func<Type, Type, bool>? serviceTypeSelector = null,
        bool registerSelf = true,
        Action<RegistrationConfigurator>? configurator = null)
    {
        Shield.EnsureNotNull(types, nameof(types));

        types = selector != null
            ? types.Where(t => t.IsResolvableType()).Where(selector)
            : types.Where(t => t.IsResolvableType());

        foreach (var type in types)
        {
            var serviceTypes = type.GetRegisterableBaseTypes().Concat(type.GetRegisterableInterfaceTypes());

            if (serviceTypeSelector != null)
                serviceTypes = serviceTypes.Where(t => serviceTypeSelector(type, t));

            if (type.IsGenericTypeDefinition)
                serviceTypes = serviceTypes.Where(t =>
                {
                    if (!t.IsGenericType) return false;
                    return type.GetGenericArguments().Length == t.GetGenericArguments().Length;
                }).Select(t => t.GetGenericTypeDefinition());

            foreach (var service in serviceTypes)
                _ = configurator switch
                {
                    null => this.Register(service, type),
                    _ => this.Register(service, type, configurator)
                };

            if (registerSelf)
                _ = configurator switch
                {
                    null => this.Register(type),
                    _ => this.Register(type, configurator)
                };
        }

        return this;
    }

    /// <inheritdoc />
    public IStashboxContainer ComposeBy(Type compositionRootType, params object[] compositionRootArguments)
    {
        this.ThrowIfDisposed();
        Shield.EnsureNotNull(compositionRootType, nameof(compositionRootType));
        Shield.EnsureTrue(compositionRootType.IsCompositionRoot(), $"The given type {compositionRootType} doesn't implement ICompositionRoot.");

        var compositionRoot = (ICompositionRoot)this.Activate(compositionRootType, compositionRootArguments);
        compositionRoot.Compose(this);

        return this;
    }

    /// <inheritdoc />
    public IStashboxContainer ComposeBy(ICompositionRoot compositionRoot)
    {
        this.ThrowIfDisposed();
        Shield.EnsureNotNull(compositionRoot, nameof(compositionRoot));

        compositionRoot.Compose(this);
        return this;
    }

    private void RegisterTypeAs(Type typeFrom, Type type, Action<RegistrationConfigurator>? configurator)
    {
        _ = configurator switch
        {
            null => this.Register(typeFrom, type),
            _ => this.Register(typeFrom, type, configurator)
        };
    }
}