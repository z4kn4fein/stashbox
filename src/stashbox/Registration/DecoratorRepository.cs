using System;
using Stashbox.Entity;
using Stashbox.Infrastructure.Registration;
using Stashbox.Utils;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a decorator repository.
    /// </summary>
    public class DecoratorRepository : IDecoratorRepository
    {
        private readonly ConcurrentTree<Type, ConcurrentOrderedKeyStore<Type, IServiceRegistration>> repository;

        /// <summary>
        /// Constructs a <see cref="DecoratorRepository"/>.
        /// </summary>
        public DecoratorRepository()
        {
            this.repository = ConcurrentTree<Type, ConcurrentOrderedKeyStore<Type, IServiceRegistration>>.Create();
        }

        /// <inheritdoc />
        public void AddDecorator(Type type, IServiceRegistration serviceRegistration, bool remap, bool replace)
        {
            var newRepository = new ConcurrentOrderedKeyStore<Type, IServiceRegistration>
            {
                {serviceRegistration.ImplementationType, serviceRegistration}
            };

            if (remap)
                this.repository.AddOrUpdate(type, newRepository, (oldValue, newValue) => newValue);
            else
                this.repository.AddOrUpdate(type, newRepository, (oldValue, newValue) => oldValue
                    .AddOrUpdate(serviceRegistration.ImplementationType, serviceRegistration, replace));
        }

        /// <inheritdoc />
        public KeyValue<Type, IServiceRegistration>[] GetDecoratorsOrDefault(Type type) =>
             this.repository.GetOrDefault(type)?.Repository;
    }
}
