using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public object Resolve(Type typeFrom, bool nullResultAllowed = false, object[] dependencyOverrides = null) =>
            this.ContainerContext.RootScope.Resolve(typeFrom, nullResultAllowed, dependencyOverrides);

        /// <inheritdoc />
        public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false, object[] dependencyOverrides = null) =>
            this.ContainerContext.RootScope.Resolve(typeFrom, name, nullResultAllowed, dependencyOverrides);

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides = null) =>
            this.ContainerContext.RootScope.ResolveAll<TKey>(dependencyOverrides);

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides = null) =>
            this.ContainerContext.RootScope.ResolveAll(typeFrom, dependencyOverrides);

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes) =>
            this.ContainerContext.RootScope.ResolveFactory(typeFrom, name, nullResultAllowed, parameterTypes);

        /// <inheritdoc />
        public IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object name = null) =>
            this.ContainerContext.RootScope.PutInstanceInScope(typeFrom, instance, withoutDisposalTracking, name);

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance) =>
            this.ContainerContext.RootScope.BuildUp(instance);

        /// <inheritdoc />
        public object Activate(Type type, params object[] arguments) =>
            this.ContainerContext.RootScope.Activate(type, arguments);

        /// <inheritdoc />
        public ValueTask InvokeAsyncInitializers(CancellationToken token = default) =>
            this.ContainerContext.RootScope.InvokeAsyncInitializers(token);

        /// <inheritdoc />
        public object GetService(Type serviceType) =>
            this.ContainerContext.RootScope.GetService(serviceType);
    }
}
