using Stashbox.Utils;
using System;
using System.Collections.Generic;

namespace Stashbox.Registration
{
    internal class DecoratorRepository : IDecoratorRepository
    {
        private ImmutableTree<Type, ImmutableArray<Type, IServiceRegistration>> repository = ImmutableTree<Type, ImmutableArray<Type, IServiceRegistration>>.Empty;

        public void AddDecorator(Type type, IServiceRegistration serviceRegistration, bool remap, bool replace)
        {
            var newRepository = new ImmutableArray<Type, IServiceRegistration>(serviceRegistration.ImplementationType, serviceRegistration);

            if (remap)
                Swap.SwapValue(ref this.repository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, (oldValue, newValue) => newValue), type, newRepository, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            else
                Swap.SwapValue(ref this.repository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, (oldValue, newValue) => oldValue
                        .AddOrUpdate(t3.ImplementationType, t3, t4)), type, newRepository, serviceRegistration, replace);
        }

        public IEnumerable<IServiceRegistration> GetDecoratorsOrDefault(Type type) =>
             this.repository.GetOrDefault(type);
    }
}
