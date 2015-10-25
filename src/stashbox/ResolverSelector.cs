using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Collections.Immutable;
using System.Linq;

namespace Stashbox
{
    internal class ResolverSelector : IResolverSelector
    {
        private readonly Ref<ImmutableHashSet<ResolverRegistration>> resolverRepository;
        private readonly DisposableReaderWriterLock readerWriterLock;

        public ResolverSelector()
        {
            this.readerWriterLock = new DisposableReaderWriterLock();
            this.resolverRepository = new Ref<ImmutableHashSet<ResolverRegistration>>(ImmutableHashSet<ResolverRegistration>.Empty);
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo)
        {
            //using (this.readerWriterLock.AquireReadLock())
            return this.resolverRepository.Value.Any(registration => registration.Predicate(containerContext, typeInfo));
        }

        public bool TryChooseResolver(IContainerContext containerContext, TypeInformation typeInfo, out Resolver resolver)
        {
            //using (this.readerWriterLock.AquireReadLock())
            //{
            var resolverFactory = this.resolverRepository.Value.FirstOrDefault(
                registration => registration.Predicate(containerContext, typeInfo));
            if (resolverFactory != null)
            {
                resolver = resolverFactory.ResolverFactory.Create(containerContext, typeInfo);
                return true;
            }

            resolver = null;
            return false;
            //}
        }

        public void AddResolver(ResolverRegistration resolverRegistration)
        {
            //using (this.readerWriterLock.AquireWriteLock())
            var newRepo = this.resolverRepository.Value.Add(resolverRegistration);

            if (!this.resolverRepository.TrySwapIfStillCurrent(this.resolverRepository.Value, newRepo))
                this.resolverRepository.Swap(_ => newRepo);
        }
    }
}
