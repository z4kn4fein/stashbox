using Stashbox.Entity;
using System;

namespace Stashbox.Infrastructure
{
    public interface IStashboxContainer : IDependencyRegistrator, IDependencyResolver
    {
        void RegisterBuildExtension(BuildExtension buildExtension);
        void RegisterResolver(Func<IBuilderContext, TypeInformation, bool> resolverPredicate, ResolverFactory factory);
    }
}
