using System;
using System.Collections.Generic;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public object Resolve(Type typeFrom, bool nullResultAllowed = false, object[] dependencyOverrides = null) =>
           this.rootResolver.Resolve(typeFrom, nullResultAllowed, dependencyOverrides);

        /// <inheritdoc />
        public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false, object[] dependencyOverrides = null) =>
            this.rootResolver.Resolve(typeFrom, name, nullResultAllowed, dependencyOverrides);

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides = null) =>
            this.rootResolver.ResolveAll<TKey>(dependencyOverrides);

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides = null) =>
            this.rootResolver.ResolveAll(typeFrom, dependencyOverrides);

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes) =>
            this.rootResolver.ResolveFactory(typeFrom, name, nullResultAllowed, parameterTypes);

        /// <inheritdoc />
        public IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false) =>
            this.rootResolver.PutInstanceInScope(typeFrom, instance, withoutDisposalTracking);

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance) =>
            this.rootResolver.BuildUp(instance);

        /// <inheritdoc />
        public object Activate(Type type, params object[] arguments) =>
            this.rootResolver.Activate(type, arguments);

#if HAS_SERVICEPROVIDER
        /// <inheritdoc />
        public object GetService(Type serviceType) =>
            this.rootResolver.GetService(serviceType);
#endif
    }
}
