using Stashbox.Registration;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer RegisterTypesAs(Type typeFrom, IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(types, nameof(types));

            if (selector != null)
                types = types.Where(selector);

            foreach (var type in types.Where(t => t.IsValidForRegistration()))
            {
                if (type.Implements(typeFrom))
                {
                    this.RegisterTypeAs(typeFrom, type, configurator);
                    continue;
                }

                var typeFromInfo = typeFrom.GetTypeInfo();
                var typeInfo = type.GetTypeInfo();

                if (!typeFromInfo.IsGenericTypeDefinition) continue;

                if (typeInfo.BaseType.GetTypeInfo().IsGenericType && typeInfo.BaseType.GetGenericTypeDefinition() == typeFrom)
                {
                    this.RegisterTypeAs(typeInfo.IsGenericTypeDefinition ? typeFrom : typeInfo.BaseType, type, configurator);
                    continue;
                }

                foreach (var impl in typeInfo.ImplementedInterfaces)
                    if (impl.GetTypeInfo().IsGenericType && impl.GetGenericTypeDefinition() == typeFrom)
                        this.RegisterTypeAs(typeInfo.IsGenericTypeDefinition ? typeFrom : impl, type, configurator);
            }

            return this;
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterTypes(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(types, nameof(types));

            if (selector != null)
                types = types.Where(selector);

            var validTypes = types.Where(t => t.IsValidForRegistration());
            foreach (var type in validTypes)
            {
                foreach (var interfaceType in type.GetRegisterableInterfaceTypes())
                    this.RegisterType(interfaceType, type, configurator);

                foreach (var baseType in type.GetRegisterableBaseTypes())
                    this.RegisterType(baseType, type, configurator);

                this.RegisterType(type, configurator);
            }

            return this;
        }

        /// <inheritdoc />
        public IStashboxContainer ComposeBy(Type compositionRootType)
        {
            Shield.EnsureNotNull(compositionRootType, nameof(compositionRootType));
            Shield.EnsureTrue(compositionRootType.IsCompositionRoot(), $"The given type {compositionRootType} doesn't implement ICompositionRoot.");

            var compositionRoot = (ICompositionRoot)Activator.CreateInstance(compositionRootType);
            compositionRoot.Compose(this);

            return this;
        }

        private void RegisterTypeAs(Type typeFrom, Type type, Action<IFluentServiceRegistrator> configurator)
        {
            if (configurator == null)
                this.RegisterType(typeFrom, type);
            else
                this.RegisterType(typeFrom, type, configurator);
        }
    }
}
