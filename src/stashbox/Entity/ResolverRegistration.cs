using Stashbox.Infrastructure;
using System;

namespace Stashbox.Entity
{
    internal class ResolverRegistration
    {
        public Func<IContainerContext, TypeInformation, bool> Predicate { get; set; }
        public ResolverFactory ResolverFactory { get; set; }
    }
}
