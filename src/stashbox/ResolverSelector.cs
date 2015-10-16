using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq;

namespace Stashbox
{
    public class ResolverSelector : IResolverSelector
    {
        private readonly ConcurrentKeyValueStore<Func<IContainerContext, TypeInformation, bool>, ResolverFactory> resolverRepository;

        public ResolverSelector()
        {
            this.resolverRepository = new ConcurrentKeyValueStore<Func<IContainerContext, TypeInformation, bool>, ResolverFactory>();
        }

        public bool CanResolve(IContainerContext containerContext, TypeInformation typeInfo)
        {
            return this.resolverRepository.Keys.Any(predicate => predicate(containerContext, typeInfo));
        }

        public bool TryChooseResolver(IContainerContext containerContext, TypeInformation typeInfo, out Resolver resolver)
        {
            var key = this.resolverRepository.Keys.FirstOrDefault(predicate => predicate(containerContext, typeInfo));
            ResolverFactory resolverFactory;
            if (key != null && this.resolverRepository.TryGet(key, out resolverFactory))
            {
                resolver = resolverFactory.Create(containerContext, typeInfo);
                return true;
            }

            resolver = null;
            return false;
        }

        public void AddResolver(Func<IContainerContext, TypeInformation, bool> resolverPredicate, ResolverFactory factory)
        {
            this.resolverRepository.Add(resolverPredicate, factory);
        }
    }
}
