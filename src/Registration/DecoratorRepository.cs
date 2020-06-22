using Stashbox.Utils;
using Stashbox.Utils.Data.Immutable;
using System;
using System.Collections.Generic;

namespace Stashbox.Registration
{
    internal class DecoratorRepository : IDecoratorRepository
    {
        private ImmutableTree<Type, IImmutableArray<Type, ServiceRegistration>> repository = ImmutableTree<Type, IImmutableArray<Type, ServiceRegistration>>.Empty;

        public void AddDecorator(Type type, ServiceRegistration serviceRegistration, bool remap, bool replace)
        {
            var newRepository = ImmutableArray<Type, ServiceRegistration>.Empty.Add(serviceRegistration.ImplementationType, serviceRegistration);

            if (remap)
                Swap.SwapValue(ref this.repository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, (oldValue, newValue) => newValue), type, newRepository, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            else
                Swap.SwapValue(ref this.repository, (t1, t2, t3, t4, repo) =>
                    repo.AddOrUpdate(t1, t2, (oldValue, newValue) => oldValue
                        .AddOrUpdate(t3.ImplementationType, t3, t4)), type, newRepository, serviceRegistration, replace);
        }

        public IEnumerable<ServiceRegistration> GetDecoratorsOrDefault(Type type) =>
             this.repository.GetOrDefault(type);
    }
}
