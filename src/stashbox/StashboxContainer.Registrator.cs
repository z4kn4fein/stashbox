using Stashbox.Registration;
using Stashbox.Utils;
using System;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public IStashboxContainer RegisterType<TFrom, TTo>(Action<IFluentServiceRegistrator<TTo>> configurator = null)
            where TFrom : class
            where TTo : class, TFrom
        {
            var context = this.ServiceRegistrator.PrepareContext<TTo>(typeof(TFrom), typeof(TTo));
            configurator?.Invoke(context);
            return context.Register();
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterType<TFrom>(Type typeTo, Action<IFluentServiceRegistrator<TFrom>> configurator = null)
            where TFrom : class
        {
            var context = this.ServiceRegistrator.PrepareContext<TFrom>(typeof(TFrom), typeTo);
            configurator?.Invoke(context);
            return context.Register();
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterType(Type typeFrom, Type typeTo, Action<IFluentServiceRegistrator> configurator = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(typeTo, nameof(typeTo));

            var context = this.ServiceRegistrator.PrepareContext(typeFrom, typeTo);
            configurator?.Invoke(context);
            return context.Register();
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterType<TTo>(Action<IFluentServiceRegistrator<TTo>> configurator = null)
             where TTo : class
        {
            var type = typeof(TTo);
            var context = this.ServiceRegistrator.PrepareContext<TTo>(type, type);
            configurator?.Invoke(context);
            return context.Register();
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterType(Type typeTo, Action<IFluentServiceRegistrator> configurator = null) =>
            this.RegisterType(typeTo, typeTo, configurator);

        /// <inheritdoc />
        public IStashboxContainer RegisterInstanceAs<TFrom>(TFrom instance, object name = null, bool withoutDisposalTracking = false,
            Action<TFrom> finalizerDelegate = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            return this.RegisterType<TFrom>(instance.GetType(), context =>
            {
                context.WithFinalizer(finalizerDelegate).WithInstance(instance).WithName(name);
                if (withoutDisposalTracking)
                    context.WithoutDisposalTracking();
            });
        }

        /// <inheritdoc />
        public IStashboxContainer RegisterInstance(Type serviceType, object instance, object name = null, bool withoutDisposalTracking = false)
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            return this.RegisterType(serviceType, instance.GetType(), context =>
            {
                context.WithInstance(instance).WithName(name);
                if (withoutDisposalTracking)
                    context.WithoutDisposalTracking();
            });
        }

        /// <inheritdoc />
        public IStashboxContainer WireUpAs<TFrom>(TFrom instance, object name = null, bool withoutDisposalTracking = false,
            Action<TFrom> finalizerDelegate = null) where TFrom : class
        {
            Shield.EnsureNotNull(instance, nameof(instance));

            this.WireUpInternal(instance, name, typeof(TFrom), instance.GetType(), withoutDisposalTracking, finalizerDelegate);
            return this;
        }

        /// <inheritdoc />
        public IStashboxContainer WireUp(Type serviceType, object instance, object name = null, bool withoutDisposalTracking = false)
        {
            Shield.EnsureNotNull(instance, nameof(instance));
            Shield.EnsureNotNull(serviceType, nameof(serviceType));

            this.WireUpInternal(instance, name, serviceType, instance.GetType(), withoutDisposalTracking);
            return this;
        }

        private void WireUpInternal(object instance, object keyName, Type typeFrom, Type typeTo, bool withoutDisposalTracking, object finalizerDelelgate = null)
        {
            var data = RegistrationContextData.New();
            data.Name = keyName;
            data.ExistingInstance = instance;
            data.Finalizer = finalizerDelelgate;

            var registration = new ServiceRegistration(typeFrom, typeTo,
                this.ContainerContext.ContainerConfigurator, this.objectBuilderSelector.Get(ObjectBuilder.WireUp),
                data, false, !withoutDisposalTracking);

            this.registrationRepository.AddOrUpdateRegistration(registration, false, false);
            this.containerExtensionManager.ExecuteOnRegistrationExtensions(this.ContainerContext, registration);
        }
    }
}
