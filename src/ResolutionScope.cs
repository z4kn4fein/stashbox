using Stashbox.BuildUp.Expressions;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

namespace Stashbox
{
    internal sealed class ResolutionScope : IResolutionScope, IDependencyResolver
    {
        private class DelegateCache
        {
            public AvlTreeKeyValue<object, Func<IResolutionScope, object>> ServiceDelegates = AvlTreeKeyValue<object, Func<IResolutionScope, object>>.Empty;
            public AvlTreeKeyValue<object, Func<IResolutionScope, Delegate>> FactoryDelegates = AvlTreeKeyValue<object, Func<IResolutionScope, Delegate>>.Empty;
        }

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

        private readonly IResolverSelector resolverSelector;
        private readonly IServiceRegistrator serviceRegistrator;
        private readonly IExpressionBuilder expressionBuilder;
        private readonly IContainerContext containerContext;

        private int disposed;
        private DisposableItem rootItem;
        private FinalizableItem rootFinalizableItem;
        private AvlTree<object> scopedItems;
        private AvlTreeKeyValue<Type, object> scopedInstances;
        private AvlTree<ThreadLocal<bool>> circularDependencyBarrier = AvlTree<ThreadLocal<bool>>.Empty;
        
        private readonly DelegateCache delegateCache;

        /// <inheritdoc />
        public bool HasScopedInstances => !this.scopedInstances.IsEmpty;

        /// <inheritdoc />
        public IResolutionScope RootScope { get; }

        /// <inheritdoc />
        public object Name { get; }

        /// <inheritdoc />
        public IResolutionScope ParentScope { get; }

        private ResolutionScope(IResolverSelector resolverSelector, IServiceRegistrator serviceRegistrator,
            IExpressionBuilder expressionBuilder, IContainerContext containerContext,
            DelegateCache delegateCache, object name)
        {
            this.rootItem = DisposableItem.Empty;
            this.rootFinalizableItem = FinalizableItem.Empty;
            this.scopedItems = AvlTree<object>.Empty;
            this.scopedInstances = AvlTreeKeyValue<Type, object>.Empty;
            this.resolverSelector = resolverSelector;
            this.serviceRegistrator = serviceRegistrator;
            this.expressionBuilder = expressionBuilder;
            this.containerContext = containerContext;
            this.Name = name;
            this.delegateCache = delegateCache;
        }

        internal ResolutionScope(IResolverSelector resolverSelector, IServiceRegistrator serviceRegistrator,
            IExpressionBuilder expressionBuilder, IContainerContext containerContext)
            : this(resolverSelector, serviceRegistrator, expressionBuilder, containerContext,
                  new DelegateCache(), null)
        {
            this.RootScope = this;
        }

        private ResolutionScope(IResolverSelector resolverSelector, IServiceRegistrator serviceRegistrator, IExpressionBuilder expressionBuilder,
            IContainerContext containerContext, IResolutionScope rootScope, IResolutionScope parent, DelegateCache delegateCache, object name = null)
            : this(resolverSelector, serviceRegistrator, expressionBuilder, containerContext, delegateCache, name)
        {
            this.RootScope = rootScope;
            this.ParentScope = parent;
        }

        public object Resolve(Type typeFrom, bool nullResultAllowed = false, object[] dependencyOverrides = null)
        {
            var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefault(typeFrom);
            return cachedFactory != null ? cachedFactory(this) : this.Activate(ResolutionContext.New(this, nullResultAllowed, dependencyOverrides), typeFrom);
        }

        public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false, object[] dependencyOverrides = null)
        {
            var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefault(name);
            return cachedFactory != null ? cachedFactory(this) : this.Activate(ResolutionContext.New(this, nullResultAllowed, dependencyOverrides), typeFrom, name);
        }

        public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides = null) =>
            (IEnumerable<TKey>)this.Resolve(typeof(IEnumerable<TKey>), dependencyOverrides: dependencyOverrides);

        public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides = null) =>
            (IEnumerable<object>)this.Resolve(typeof(IEnumerable<>).MakeGenericType(typeFrom), dependencyOverrides: dependencyOverrides);

        public Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes)
        {
            var key = name ?? typeFrom;
            var cachedFactory = this.delegateCache.FactoryDelegates.GetOrDefault(key);
            return cachedFactory != null ? cachedFactory(this) : this.ActivateFactoryDelegate(typeFrom, parameterTypes, this, name, nullResultAllowed);
        }

        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false)
        {
            var scope = new ResolutionScope(this.resolverSelector, this.serviceRegistrator,
                this.expressionBuilder, this.containerContext, this.RootScope, this,
                this.delegateCache, name);

            return attachToParent ? this.AddDisposableTracking(scope) : scope;
        }

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
            var resolutionContext = ResolutionContext.New(this);
            var expr = this.expressionBuilder.CreateFillExpression(this.containerContext, registration.CreateServiceRegistration(false),
                instance.AsConstant(), resolutionContext, typeTo);
            var factory = expr.CompileDelegate(resolutionContext);
            return (TTo)factory(this);
        }

        /// <inheritdoc />
        public TDisposable AddDisposableTracking<TDisposable>(TDisposable disposable)
            where TDisposable : IDisposable
        {
            Swap.SwapValue(ref this.rootItem, root =>
                new DisposableItem { Item = disposable, Next = root });

            return disposable;
        }

        /// <inheritdoc />
        public void AddScopedInstance(Type key, object value) =>
            Swap.SwapValue(ref this.scopedInstances, instances => instances.AddOrUpdate(key, value));

        /// <inheritdoc />
        public object GetScopedInstanceOrDefault(Type key) =>
            this.scopedInstances.GetOrDefault(key);

        /// <inheritdoc />
        public TService AddWithFinalizer<TService>(TService finalizable, Action<TService> finalizer)
        {
            Swap.SwapValue(ref this.rootFinalizableItem, root =>
                new FinalizableItem { Item = finalizable, Finalizer = f => finalizer((TService)f), Next = root });

            return finalizable;
        }

        /// <inheritdoc />
        public object GetOrAddScopedItem(int key, object sync, Func<IResolutionScope, object> factory)
        {
            var item = this.scopedItems.GetOrDefault(key);
            if (item != null) return item;

            lock (sync)
            {
                item = this.scopedItems.GetOrDefault(key);
                if (item != null) return item;

                item = factory(this);
                Swap.SwapValue(ref this.scopedItems, items => items.AddOrUpdate(key, item));

                return item;
            }
        }

        /// <inheritdoc />
        public void InvalidateDelegateCache()
        {
            this.delegateCache.ServiceDelegates = AvlTreeKeyValue<object, Func<IResolutionScope, object>>.Empty;
            this.delegateCache.FactoryDelegates = AvlTreeKeyValue<object, Func<IResolutionScope, Delegate>>.Empty;
        }

        /// <inheritdoc />
        public ISet<object> GetActiveScopeNames()
        {
            var set = new HashSet<object>();
            IResolutionScope current = this;

            while (current != null)
            {
                if (current.Name != null)
                    set.Add(current.Name);

                current = current.ParentScope;
            }

            return set.Count > 0 ? set : null;
        }

        public void CheckRuntimeCircularDependencyBarrier(int key, Type type)
        {
            var check = this.circularDependencyBarrier.GetOrDefault(key);
            if (check != null && check.Value)
                throw new CircularDependencyException(type);

            Swap.SwapValue(ref this.circularDependencyBarrier, barrier => barrier.AddOrUpdate(key, new ThreadLocal<bool>(), (old, @new) =>
                { old.Value = true; return old; }));
        }

        public void ResetRuntimetCircularDependencyBarrier(int key)
        {
            var check = this.circularDependencyBarrier.GetOrDefault(key);
            if (check != null)
                check.Value = false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
                return;

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
            
            if(this.circularDependencyBarrier.IsEmpty)
                return;

            foreach (var threadLocal in this.circularDependencyBarrier.Walk())
                threadLocal.Dispose();
        }
        
        private object Activate(ResolutionContext resolutionContext, Type type, object name = null)
        {
            if (type == Constants.ResolverType)
                return resolutionContext.ResolutionScope;

            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(type, resolutionContext, name);
            if (registration != null)
            {
                var ragistrationFactory = registration.GetExpression(this.containerContext, resolutionContext, type)?.CompileDelegate(resolutionContext);
                if (ragistrationFactory == null)
                    return null;

                if (resolutionContext.ShouldCacheFactoryDelegate)
                    Swap.SwapValue(ref this.delegateCache.ServiceDelegates, c => c.AddOrUpdate(name ?? type, ragistrationFactory));

                return ragistrationFactory(resolutionContext.ResolutionScope);
            }

            var expr = this.resolverSelector.GetResolverExpression(this.containerContext, new TypeInformation { Type = type, DependencyName = name }, resolutionContext);
            if (expr == null)
                if (resolutionContext.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type);

            var factory = expr.CompileDelegate(resolutionContext);

            if (resolutionContext.ShouldCacheFactoryDelegate)
                Swap.SwapValue(ref this.delegateCache.ServiceDelegates, c => c.AddOrUpdate(name ?? type, factory));

            return factory(resolutionContext.ResolutionScope);
        }

        private Delegate ActivateFactoryDelegate(Type type, Type[] parameterTypes, IResolutionScope resolutionScope, object name, bool nullResultAllowed)
        {
            var resolutionContext = ResolutionContext.New(resolutionScope, nullResultAllowed);
            resolutionContext.AddParameterExpressions(type, parameterTypes.Select(p => p.AsParameter()).ToArray());

            var typeInfo = new TypeInformation { Type = type, DependencyName = name };
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo, resolutionContext);

            var initExpression = registration == null ?
                this.resolverSelector.GetResolverExpression(this.containerContext, typeInfo, resolutionContext) :
                registration.GetExpression(this.containerContext, resolutionContext, type);

            if (initExpression == null)
                if (resolutionContext.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type);

            var expression = initExpression.AsLambda(resolutionContext.ParameterExpressions.SelectMany(x => x));

            var factory = expression.CompileDynamicDelegate(resolutionContext);
            Swap.SwapValue(ref this.delegateCache.FactoryDelegates, c => c.AddOrUpdate(name ?? type, factory));
            return factory(resolutionScope);
        }
    }
}
