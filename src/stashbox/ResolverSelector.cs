using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Stashbox
{
    internal class ResolverSelector : IResolverSelector
    {
        private readonly HashSet<ResolverRegistration> resolverRepository;
        private readonly DisposableReaderWriterLock readerWriterLock;

        public ResolverSelector()
        {
            this.readerWriterLock = new DisposableReaderWriterLock();
            this.resolverRepository = new HashSet<ResolverRegistration>();
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo)
        {
            using (this.readerWriterLock.AcquireReadLock())
                return this.resolverRepository.Any(registration => registration.Predicate(containerContext, typeInfo));
        }

        public bool TryChooseResolver(IContainerContext containerContext, TypeInformation typeInfo, out Resolver resolver)
        {
            using (this.readerWriterLock.AcquireReadLock())
            {
                var resolverFactory = this.resolverRepository.FirstOrDefault(
                    registration => registration.Predicate(containerContext, typeInfo));
                if (resolverFactory != null)
                {
                    resolver = resolverFactory.ResolverFactory.Create(containerContext, typeInfo);
                    return true;
                }

                resolver = null;
                return false;
            }
        }

        public void AddResolver(ResolverRegistration resolverRegistration)
        {
            using (this.readerWriterLock.AcquireWriteLock())
                this.resolverRepository.Add(resolverRegistration);
        }

        public IResolverSelector CreateCopy()
        {
            var selector = new ResolverSelector();
            using (this.readerWriterLock.AcquireReadLock())
                foreach (var resolverRegistration in resolverRepository)
                    selector.AddResolver(resolverRegistration);

            return selector;
        }
    }
}
