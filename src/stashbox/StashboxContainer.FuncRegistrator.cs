using System;
using Stashbox.Infrastructure;
using Stashbox.Registration;

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

            var data = RegistrationContextData.New();
            data.Name = name;
            data.FuncDelegate = factory;

            var registration = new ServiceRegistration(factoryType, internalFactoryType,
                this.ContainerContext, this.objectBuilderSelector.Get(ObjectBuilder.Func),
                data, false, false);

            this.registrationRepository.AddOrUpdateRegistration(registration, false, false);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registration);
            return this;
        }
    }
}
