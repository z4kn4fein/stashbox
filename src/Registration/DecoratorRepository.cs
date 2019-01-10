using Stashbox.Entity;
using Stashbox.Utils;
using System;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a decorator repository.
    /// </summary>
    public class DecoratorRepository : IDecoratorRepository
    {
        private AvlTreeKeyValue<Type, ArrayStoreKeyed<Type, IServiceRegistration>> repository;

        /// <summary>
        /// Constructs a <see cref="DecoratorRepository"/>.
        /// </summary>
        public DecoratorRepository()
        {
            this.repository = AvlTreeKeyValue<Type, ArrayStoreKeyed<Type, IServiceRegistration>>.Empty;
        }

        /// <inheritdoc />
        public void AddDecorator(Type type, IServiceRegistration serviceRegistration, bool remap, bool replace)
        {
            var newRepository = new ArrayStoreKeyed<Type, IServiceRegistration>(serviceRegistration.ImplementationType, serviceRegistration);

            if (remap)
                Swap.SwapValue(ref this.repository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, (oldValue, newValue) => newValue), type, newRepository, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            else
                Swap.SwapValue(ref this.repository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, (oldValue, newValue) => oldValue
                        .AddOrUpdate(t3.ImplementationType, t3, t4)), type, newRepository, serviceRegistration, replace);
        }

        /// <inheritdoc />
        public KeyValue<Type, IServiceRegistration>[] GetDecoratorsOrDefault(Type type) =>
             this.repository.GetOrDefault(type)?.Repository;
    }
}
