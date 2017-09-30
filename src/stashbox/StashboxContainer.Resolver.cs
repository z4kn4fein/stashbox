using System;
using System.Collections.Generic;
using Stashbox.Infrastructure;

namespace Stashbox
{
    public partial class StashboxContainer
    {
        /// <inheritdoc />
        public object Resolve(Type typeFrom, bool nullResultAllowed = false) =>
           this.rootResolver.Resolve(typeFrom, nullResultAllowed);
        
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
        public IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false) =>
            this.rootResolver.PutInstanceInScope(typeFrom, instance, withoutDisposalTracking);

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance) =>
            this.rootResolver.BuildUp(instance);
    }
}
