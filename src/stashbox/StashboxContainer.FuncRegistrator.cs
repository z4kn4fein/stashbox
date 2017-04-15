using System;
using Stashbox.Infrastructure;
using Stashbox.Registration;
using Stashbox.Utils;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IDependencyRegistrator RegisterFunc<TService>(Func<IDependencyResolver, TService> factory, string name = null) =>
            this.RegisterFuncInternal(factory, typeof(Func<TService>), name);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterFunc<T1, TService>(Func<T1, IDependencyResolver, TService> factory, string name = null) =>
            this.RegisterFuncInternal(factory, typeof(Func<T1, TService>), name);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterFunc<T1, T2, TService>(Func<T1, T2, IDependencyResolver, TService> factory, string name = null) =>
            this.RegisterFuncInternal(factory, typeof(Func<T1, T2, TService>), name);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterFunc<T1, T2, T3, TService>(Func<T1, T2, T3, IDependencyResolver, TService> factory, string name = null) =>
            this.RegisterFuncInternal(factory, typeof(Func<T1, T2, T3, TService>), name);

        /// <inheritdoc />
        public IDependencyRegistrator RegisterFunc<T1, T2, T3, T4, TService>(Func<T1, T2, T3, T4, IDependencyResolver, TService> factory, string name = null) =>
            this.RegisterFuncInternal(factory, typeof(Func<T1, T2, T3, T4, TService>), name);

        private IDependencyRegistrator RegisterFuncInternal(Delegate factory, Type factoryType, string name)
        {
            var internalFactoryType = factory.GetType();
            var regName = NameGenerator.GetRegistrationName(factoryType, internalFactoryType, name);

            var data = RegistrationContextData.New();
            data.FuncDelegate = factory;

            var registration = new ServiceRegistration(factoryType, internalFactoryType,
                this.ContainerContext.ReserveRegistrationNumber(),
                this.objectBuilderSelector.Get(ObjectBuilder.Func), data,
                false, false);

            this.registrationRepository.AddOrUpdateRegistration(factoryType, regName, false, false, registration);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, factoryType, internalFactoryType);
            return this;
        }
    }
}
