using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Utils;

namespace Stashbox
{
    internal class ResolutionScope : IResolutionScope, IDependencyResolver
    {
        private class DisposableItem
        {
            public IDisposable Item;
            public DisposableItem Next;

            public static readonly DisposableItem Empty = new DisposableItem();
        }

        private class FinalizableItem
        {
            public object Item;
            public Action<object> Finalizer;
            public FinalizableItem Next;

            public static readonly FinalizableItem Empty = new FinalizableItem();
        }

        private readonly IActivationContext activationContext;
        private readonly IServiceRegistrator serviceRegistrator;
        private readonly IExpressionBuilder expressionBuilder;

        private readonly AtomicBool disposed;
        private DisposableItem rootItem;
        private FinalizableItem rootFinalizableItem;
        private readonly ConcurrentTree<object, object> scopedItems;
        private readonly ConcurrentTree<Type, object> scopedInstances;

        /// <inheritdoc />
        public bool HasScopedInstances => !this.scopedInstances.IsEmpty;

        /// <inheritdoc />
        public IResolutionScope RootScope { get; }

        /// <inheritdoc />
        public object Name { get; }

        public ResolutionScope(IActivationContext activationContext, IServiceRegistrator serviceRegistrator,
            IExpressionBuilder expressionBuilder, object name = null)
        {
            this.disposed = new AtomicBool();
            this.rootItem = DisposableItem.Empty;
            this.rootFinalizableItem = FinalizableItem.Empty;
            this.scopedItems = new ConcurrentTree<object, object>();
            this.scopedInstances = new ConcurrentTree<Type, object>();
            this.activationContext = activationContext;
            this.serviceRegistrator = serviceRegistrator;
            this.expressionBuilder = expressionBuilder;
            this.Name = name;
            this.RootScope = this;
        }

        public ResolutionScope(IActivationContext activationContext, IServiceRegistrator serviceRegistrator,
            IExpressionBuilder expressionBuilder, IResolutionScope rootScope, object name = null)
            : this(activationContext, serviceRegistrator, expressionBuilder, name)
        {
            this.RootScope = rootScope;
        }
        
        public object Resolve(Type typeFrom, bool nullResultAllowed = false) =>
            this.activationContext.Activate(typeFrom, this, nullResultAllowed);
        
        public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false) =>
            this.activationContext.Activate(typeFrom, this, name, nullResultAllowed);

        public IEnumerable<TKey> ResolveAll<TKey>() =>
            (IEnumerable<TKey>)this.activationContext.Activate(typeof(IEnumerable<TKey>), this);

        public IEnumerable<object> ResolveAll(Type typeFrom) =>
            (IEnumerable<object>)this.activationContext.Activate(typeof(IEnumerable<>).MakeGenericType(typeFrom), this);

        public Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes) =>
            this.activationContext.ActivateFactory(typeFrom, parameterTypes, this, name, nullResultAllowed);

        public IDependencyResolver BeginScope(object name = null) => new ResolutionScope(this.activationContext, this.serviceRegistrator,
            this.expressionBuilder, this.RootScope, name);

        public IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(instance, nameof(instance));

            this.AddScopedInstance(typeFrom, instance);
            if (!withoutDisposalTracking && instance is IDisposable disposable)
                this.AddDisposableTracking(disposable);

            return this;
        }

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance)
        {
            var typeTo = instance.GetType();
            var registration = this.serviceRegistrator.PrepareContext(typeTo, typeTo);
            var expr = this.expressionBuilder.CreateFillExpression(registration.CreateServiceRegistration(false),
                Expression.Constant(instance), ResolutionInfo.New(this), typeTo);
            var factory = expr.CompileDelegate(Constants.ScopeExpression);
            return (TTo)factory(this);
        }

        /// <inheritdoc />
        public TDisposable AddDisposableTracking<TDisposable>(TDisposable disposable)
            where TDisposable : IDisposable
        {

            var item = new DisposableItem { Item = disposable, Next = this.rootItem };
            var current = this.rootItem;
            Swap.SwapValue(ref this.rootItem, current, item, root =>
                new DisposableItem { Item = disposable, Next = root });

            return disposable;
        }
        
        /// <inheritdoc />
        public void AddScopedInstance(Type key, object value) =>
            this.scopedInstances.AddOrUpdate(key, value);

        /// <inheritdoc />
        public object GetScopedInstanceOrDefault(Type key) =>
            this.scopedInstances.GetOrDefault(key);

        /// <inheritdoc />
        public TService AddWithFinalizer<TService>(TService finalizable, Action<TService> finalizer)
        {
            var item = new FinalizableItem { Item = finalizable, Finalizer = f => finalizer((TService)f), Next = this.rootFinalizableItem };
            var current = this.rootFinalizableItem;
            Swap.SwapValue(ref this.rootFinalizableItem, current, item, root =>
                new FinalizableItem { Item = finalizable, Finalizer = f => finalizer((TService)f), Next = root });

            return finalizable;
        }

        public object GetOrAddScopedItem(object key, Func<IResolutionScope, object> factory)
        {
            var item = this.scopedItems.GetOrDefault(key);
            if (item != null) return item;

            lock (key)
            {
                item = this.scopedItems.GetOrDefault(key);
                if (item != null) return item;

                item = factory(this);
                this.scopedItems.AddOrUpdate(key, item);
            }

            return item;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the scope.
        /// </summary>
        /// <param name="disposing">Indicates the scope is disposing or not.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed.CompareExchange(false, true) || !disposing) return;

            var rootFinalizable = this.rootFinalizableItem;
            while (!ReferenceEquals(rootFinalizable, FinalizableItem.Empty))
            {
                rootFinalizable.Finalizer(rootFinalizable.Item);
                rootFinalizable = rootFinalizable.Next;
            }

            var root = this.rootItem;
            while (!ReferenceEquals(root, DisposableItem.Empty))
            {
                root.Item.Dispose();
                root = root.Next;
            }
        }
    }
}
