using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Utils;
using System;
using System.Linq;
using System.Threading;

namespace Stashbox
{
    internal class ResolverSelector : IResolverSelector
    {
        private readonly ConcurrentTree<int, ResolverRegistration> resolverRepository;
        private int resolverCounter;
        public ResolverSelector()
        {
            this.resolverRepository = new ConcurrentTree<int, ResolverRegistration>();
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo)
        {
            return this.resolverRepository.Any(registration => registration.Predicate(containerContext, typeInfo));
        }

        public bool TryChooseResolver(IContainerContext containerContext, TypeInformation typeInfo, out Resolver resolver, Func<ResolverRegistration, bool> filter = null)
        {
            var resolverFactory = filter == null ? this.resolverRepository.FirstOrDefault(
                registration => registration.Predicate(containerContext, typeInfo)) :
                this.resolverRepository.Where(filter).FirstOrDefault(
                registration => registration.Predicate(containerContext, typeInfo));

            if (resolverFactory != null)
            {
                resolver = resolverFactory.ResolverFactory(containerContext, typeInfo);
                return true;
            }

            resolver = null;
            return false;
        }

        public void AddResolver(ResolverRegistration resolverRegistration)
        {
            this.resolverRepository.AddOrUpdate(Interlocked.Increment(ref resolverCounter), resolverRegistration);
        }

        public IResolverSelector CreateCopy()
        {
            var selector = new ResolverSelector();
            foreach (var resolverRegistration in resolverRepository)
                selector.AddResolver(resolverRegistration);

            return selector;
        }
    }
}
