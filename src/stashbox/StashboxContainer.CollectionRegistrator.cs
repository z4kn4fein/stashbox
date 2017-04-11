using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Utils;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IDependencyRegistrator RegisterTypesAs<TFrom>(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
             where TFrom : class =>
             this.RegisterTypesAs(typeof(TFrom), types, selector, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterTypesAs(Type typeFrom, IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(types, nameof(types));

            if (selector != null)
                types = types.Where(selector);

            foreach (var type in types.Where(t => t.IsValidForRegistration() && t.Implements(typeFrom)))
            {
                if (configurator == null)
                    this.RegisterType(typeFrom, type);
                else
                    this.RegisterType(typeFrom, type, configurator);
            }

            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterTypesAsSelf(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(types, nameof(types));

            if (selector != null)
                types = types.Where(selector);

            foreach (var type in types.Where(t => t.IsValidForRegistration()))
            {
                if (configurator == null)
                    this.RegisterType(type);
                else
                    this.RegisterType(type, configurator);
            }

            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterTypes(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(types, nameof(types));

            if (selector != null)
                types = types.Where(selector);

            foreach (var type in types.Where(t => t.IsValidForRegistration()))
                foreach (var interfaceType in type.GetRegisterableInterfaceTypes())
                {
                    if (configurator == null)
                        this.RegisterType(interfaceType, type);
                    else
                        this.RegisterType(interfaceType, type, configurator);
                }

            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterAssembly(Assembly assembly, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(assembly, nameof(assembly));

            return this.RegisterTypes(assembly.CollectExportedTypes(), selector, configurator);
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterAssemblies(IEnumerable<Assembly> assemblies, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies)
                this.RegisterAssembly(assembly, selector, configurator);

            return this;
        }

        /// <inheritdoc />
        public IDependencyRegistrator RegisterAssemblyContaining<TFrom>(Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
             where TFrom : class =>
             this.RegisterAssemblyContaining(typeof(TFrom), selector, configurator);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterAssemblyContaining(Type typeFrom, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));

            return this.RegisterAssembly(typeFrom.GetTypeInfo().Assembly, selector, configurator);
        }

        /// <inheritdoc />
        public IDecoratorRegistrator ComposeBy<TCompositionRoot>()
            where TCompositionRoot : ICompositionRoot, new() =>
            this.ComposeBy(typeof(TCompositionRoot));

        /// <inheritdoc />
        public IDecoratorRegistrator ComposeBy(Type compositionRootType)
        {
            Shield.EnsureNotNull(compositionRootType, nameof(compositionRootType));
            Shield.EnsureTrue(compositionRootType.IsCompositionRoot(), $"The given type {compositionRootType} doesn't implement ICompositionRoot.");

            var compositionRoot = (ICompositionRoot)Activator.CreateInstance(compositionRootType);
            compositionRoot.Compose(this);

            return this;
        }

        /// <inheritdoc />
        public IDecoratorRegistrator ComposeAssembly(Assembly assembly)
        {
            Shield.EnsureNotNull(assembly, nameof(assembly));

            var compositionRootTypes = assembly.CollectDefinedTypes().Where(type => !type.GetTypeInfo().IsAbstract && type.IsCompositionRoot());

            foreach (var compositionRootType in compositionRootTypes)
                this.ComposeBy(compositionRootType);

            return this;
        }

        /// <inheritdoc />
        public IDecoratorRegistrator ComposeAssemblies(IEnumerable<Assembly> assemblies)
        {
            Shield.EnsureNotNull(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies)
                this.ComposeAssembly(assembly);

            return this;
        }
    }
}
