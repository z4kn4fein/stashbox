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
                if (!typeFromInfo.IsGenericTypeDefinition) continue;

                var typeInfo = type.GetTypeInfo();

                foreach (var baseType in type.GetRegisterableBaseTypes().Where(baseType => baseType.GetTypeInfo().IsGenericType && baseType.GetGenericTypeDefinition() == typeFrom))
                    this.RegisterTypeAs(typeInfo.IsGenericTypeDefinition ? typeFrom : baseType, type, configurator);

                foreach (var interfaceType in type.GetRegisterableInterfaceTypes().Where(interfaceType => interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeFrom))
                    this.RegisterTypeAs(typeInfo.IsGenericTypeDefinition ? typeFrom : interfaceType, type, configurator);
            }

            return this;
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterTypes(IEnumerable<Type> types, Func<Type, bool> selector = null, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(types, nameof(types));

            if (selector != null)
                types = types.Where(selector);

            foreach (var type in types.Where(t => t.IsValidForRegistration()))
            {
                foreach (var interfaceType in type.GetRegisterableInterfaceTypes())
                    this.Register(interfaceType, type, configurator);

                foreach (var baseType in type.GetRegisterableBaseTypes())
                    this.Register(baseType, type, configurator);

                this.Register(type, configurator);
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
                this.Register(typeFrom, type);
            else
                this.Register(typeFrom, type, configurator);
        }
    }
}
