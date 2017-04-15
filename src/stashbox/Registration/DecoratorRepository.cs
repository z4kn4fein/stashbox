using System;
using System.Threading;
using Stashbox.Infrastructure.Registration;
using Stashbox.Utils;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a decorator repository.
    /// </summary>
    public class DecoratorRepository : IDecoratorRepository
    {
        private int decoratorCounter;
        private readonly ConcurrentTree<Type, ConcurrentTree<IServiceRegistration>> repository;

        /// <summary>
        /// Constructs a <see cref="DecoratorRepository"/>.
        /// </summary>
        public DecoratorRepository()
        {
            this.repository = ConcurrentTree<Type, ConcurrentTree<IServiceRegistration>>.Create();
        }

        /// <inheritdoc />
        public void AddDecorator(Type type, IServiceRegistration serviceRegistration, bool replace)
        {
            var newRepository = ConcurrentTree<IServiceRegistration>.Create();
            newRepository.AddOrUpdate(Interlocked.Increment(ref this.decoratorCounter), serviceRegistration);

            if (replace)
                this.repository.AddOrUpdate(type, newRepository,
                    (oldValue, newValue) => oldValue.HasMultipleItems ? oldValue.AddOrUpdate(Interlocked.Increment(ref this.decoratorCounter), serviceRegistration,
                        (oldReg, newReg) => newReg) : newValue);
            else
                this.repository.AddOrUpdate(type, newRepository, (oldValue, newValue) => oldValue
                    .AddOrUpdate(Interlocked.Increment(ref this.decoratorCounter), serviceRegistration));
        }

        /// <inheritdoc />
        public ConcurrentTree<IServiceRegistration> GetDecoratorsOrDefault(Type type) =>
             this.repository.GetOrDefault(type);
    }
}
