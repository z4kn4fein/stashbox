using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System;
using System.Linq;

namespace Stashbox
{
    public class ResolverSelector : IResolverSelector
    {
        private readonly ConcurrentKeyValueStore<Func<IBuilderContext, TypeInformation, bool>, ResolverFactory> resolverRepository;

        public ResolverSelector()
        {
            this.resolverRepository = new ConcurrentKeyValueStore<Func<IBuilderContext, TypeInformation, bool>, ResolverFactory>();
        }

        public bool CanResolve(IBuilderContext builderContext, TypeInformation typeInfo)
        {
            return this.resolverRepository.Keys.Any(predicate => predicate(builderContext, typeInfo));
        }

        public bool TryChooseResolver(IBuilderContext builderContext, TypeInformation typeInfo, out Resolver resolver)
        {
            var key = this.resolverRepository.Keys.First(predicate => predicate(builderContext, typeInfo));
            ResolverFactory resolverFactory;
            if (key != null && this.resolverRepository.TryGet(key, out resolverFactory))
            {
                resolver = resolverFactory.Create(builderContext, typeInfo);
                return true;
            }

            resolver = null;
            return false;
        }

        public void AddResolverStrategy(Func<IBuilderContext, TypeInformation, bool> resolverPredicate, ResolverFactory factory)
        {
            this.resolverRepository.Add(resolverPredicate, factory);
        }
    }
}
