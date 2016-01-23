using Stashbox.Entity;
using Stashbox.Infrastructure.ContainerExtension;
using System;

namespace Stashbox.Infrastructure
{
    public interface IStashboxContainer : IDependencyRegistrator, IDependencyResolver, IDisposable
    {
        void RegisterExtension(IContainerExtension containerExtension);
        void RegisterResolver(Func<IContainerContext, TypeInformation, bool> resolverPredicate,
            Func<IContainerContext, TypeInformation, Resolver> factory);
        IStashboxContainer CreateChildContainer();
        IStashboxContainer ParentContainer { get; }
        IContainerContext ContainerContext { get; }
        bool IsRegistered<TFrom>(string name = null);
        bool IsRegistered(Type typeFrom, string name = null);
    }
}
