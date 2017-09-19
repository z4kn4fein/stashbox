using System;
using System.Collections.Generic;
using Stashbox.Infrastructure;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public TKey Resolve<TKey>(bool nullResultAllowed = false) =>
            this.rootResolver.Resolve<TKey>(nullResultAllowed);

        /// <inheritdoc />
        public object Resolve(Type typeFrom, bool nullResultAllowed = false) =>
           this.rootResolver.Resolve(typeFrom, nullResultAllowed);

        /// <inheritdoc />
        public TKey Resolve<TKey>(object name, bool nullResultAllowed = false) =>
            this.rootResolver.Resolve<TKey>(name, nullResultAllowed);

        /// <inheritdoc />
        public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false) =>
            this.rootResolver.Resolve(typeFrom, name, nullResultAllowed);

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>() =>
            this.rootResolver.ResolveAll<TKey>();

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom) =>
            this.rootResolver.ResolveAll(typeFrom);

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes) =>
            this.rootResolver.ResolveFactory(typeFrom, name, nullResultAllowed, parameterTypes);

        /// <inheritdoc />
        public Func<TService> ResolveFactory<TService>(object name = null, bool nullResultAllowed = false) =>
            this.rootResolver.ResolveFactory<TService>(name, nullResultAllowed);

        /// <inheritdoc />
        public Func<T1, TService> ResolveFactory<T1, TService>(object name = null, bool nullResultAllowed = false) =>
            this.rootResolver.ResolveFactory<T1, TService>(name, nullResultAllowed);

        /// <inheritdoc />
        public Func<T1, T2, TService> ResolveFactory<T1, T2, TService>(object name = null, bool nullResultAllowed = false) =>
            this.rootResolver.ResolveFactory<T1, T2, TService>(name, nullResultAllowed);

        /// <inheritdoc />
        public Func<T1, T2, T3, TService> ResolveFactory<T1, T2, T3, TService>(object name = null, bool nullResultAllowed = false) =>
            this.rootResolver.ResolveFactory<T1, T2, T3, TService>(name, nullResultAllowed);

        /// <inheritdoc />
        public Func<T1, T2, T3, T4, TService> ResolveFactory<T1, T2, T3, T4, TService>(object name = null, bool nullResultAllowed = false) =>
            this.rootResolver.ResolveFactory<T1, T2, T3, T4, TService>(name, nullResultAllowed);

        /// <inheritdoc />
        public IDependencyResolver PutInstanceInScope<TFrom>(TFrom instance, bool withoutDisposalTracking = false) =>
            this.rootResolver.PutInstanceInScope(instance, withoutDisposalTracking);

        /// <inheritdoc />
        public IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false) =>
            this.rootResolver.PutInstanceInScope(typeFrom, instance, withoutDisposalTracking);

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance) =>
            this.rootResolver.BuildUp(instance);
    }
}
