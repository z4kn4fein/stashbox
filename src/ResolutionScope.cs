using Stashbox.Exceptions;
using Stashbox.Expressions;
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
            public ImmutableTree<object, Func<IResolutionScope, object>> ServiceDelegates = ImmutableTree<object, Func<IResolutionScope, object>>.Empty;
            public ImmutableTree<object, Func<IResolutionScope, Delegate>> FactoryDelegates = ImmutableTree<object, Func<IResolutionScope, Delegate>>.Empty;
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

        private readonly ResolutionStrategy resolutionStrategy;
        private readonly ExpressionFactory expressionFactory;
        private readonly IContainerContext containerContext;

        private int disposed;
        private DisposableItem rootItem = DisposableItem.Empty;
        private FinalizableItem rootFinalizableItem = FinalizableItem.Empty;
        private ImmutableTree<object> scopedItems = ImmutableTree<object>.Empty;
        private ImmutableTree<Type, ImmutableTree<object, object>> scopedInstances = ImmutableTree<Type, ImmutableTree<object, object>>.Empty;
        private ImmutableTree<ThreadLocal<bool>> circularDependencyBarrier = ImmutableTree<ThreadLocal<bool>>.Empty;

        private DelegateCache delegateCache;

        public object Name { get; }

        public IResolutionScope ParentScope { get; }

        private ResolutionScope(ResolutionStrategy resolutionStrategy,
            ExpressionFactory expressionBuilder, IContainerContext containerContext,
            DelegateCache delegateCache, object name)
        {
            this.resolutionStrategy = resolutionStrategy;
            this.expressionFactory = expressionBuilder;
            this.containerContext = containerContext;
            this.Name = name;
            this.delegateCache = delegateCache;
        }

        internal ResolutionScope(ResolutionStrategy resolutionStrategy,
            ExpressionFactory expressionBuilder, IContainerContext containerContext)
            : this(resolutionStrategy, expressionBuilder, containerContext,
                  new DelegateCache(), null)
        { }

        private ResolutionScope(ResolutionStrategy resolutionStrategy, ExpressionFactory expressionBuilder,
            IContainerContext containerContext, IResolutionScope parent, DelegateCache delegateCache, object name = null)
            : this(resolutionStrategy, expressionBuilder, containerContext, delegateCache, name)
        {
            this.ParentScope = parent;
        }

        public object Resolve(Type typeFrom, bool nullResultAllowed = false, object[] dependencyOverrides = null)
        {
            if (dependencyOverrides != null)
                return this.Activate(new ResolutionContext(this.GetActiveScopeNames(), this.containerContext,
                    this.resolutionStrategy, this == this.containerContext.RootScope, nullResultAllowed,
                    this.ProcessDependencyOverrides(dependencyOverrides)), typeFrom);

            var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefault(typeFrom);
            return cachedFactory != null
                ? cachedFactory(this)
                : this.Activate(new ResolutionContext(this.GetActiveScopeNames(), this.containerContext, this.resolutionStrategy,
                    this == this.containerContext.RootScope, nullResultAllowed,
                    this.ProcessDependencyOverrides(dependencyOverrides)), typeFrom);
        }

        public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false, object[] dependencyOverrides = null)
        {
            if (dependencyOverrides != null)
                return this.Activate(new ResolutionContext(this.GetActiveScopeNames(), this.containerContext, this.resolutionStrategy,
                    this == this.containerContext.RootScope, nullResultAllowed,
                    this.ProcessDependencyOverrides(dependencyOverrides)), typeFrom);

            var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefault(name);
            return cachedFactory != null
                ? cachedFactory(this)
                : this.Activate(new ResolutionContext(this.GetActiveScopeNames(), this.containerContext, this.resolutionStrategy,
                    this == this.containerContext.RootScope, nullResultAllowed,
                    this.ProcessDependencyOverrides(dependencyOverrides)), typeFrom, name);
        }

#if HAS_SERVICEPROVIDER
        public object GetService(Type serviceType) =>
            this.Resolve(serviceType, true);
#endif

        public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides = null) =>
            (IEnumerable<TKey>)this.Resolve(typeof(IEnumerable<TKey>), dependencyOverrides: dependencyOverrides);

        public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides = null) =>
            (IEnumerable<object>)this.Resolve(typeof(IEnumerable<>).MakeGenericType(typeFrom), dependencyOverrides: dependencyOverrides);

        public Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes)
        {
            var key = name ?? typeFrom;
            var cachedFactory = this.delegateCache.FactoryDelegates.GetOrDefault(key);
            return cachedFactory != null ? cachedFactory(this) : this.ActivateFactoryDelegate(typeFrom, parameterTypes, name, nullResultAllowed);
        }

        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false)
        {
            var scope = new ResolutionScope(this.resolutionStrategy, this.expressionFactory,
                this.containerContext, this, this.delegateCache, name);

            return attachToParent ? this.AddDisposableTracking(scope) : scope;
        }

        public IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object name = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(instance, nameof(instance));

            var subKey = name ?? typeFrom;
            var newRepo = ImmutableTree<object, object>.Empty.AddOrUpdate(subKey, instance);
            Swap.SwapValue(ref this.scopedInstances, (t1, t2, t3, t4, instances) =>
                instances.AddOrUpdate(t1, t3,
                    (old, @new) => old.AddOrUpdate(t4, t2, true)), typeFrom, instance, newRepo, subKey);

            if (!withoutDisposalTracking && instance is IDisposable disposable)
                this.AddDisposableTracking(disposable);

            this.delegateCache = new DelegateCache();

            return this;
        }

        public TTo BuildUp<TTo>(TTo instance)
        {
            var typeTo = instance.GetType();
            var resolutionContext = new ResolutionContext(this.GetActiveScopeNames(), this.containerContext,
                this.resolutionStrategy, this == this.containerContext.RootScope);
            var expression = this.expressionFactory.ConstructBuildUpExpression(resolutionContext, instance.AsConstant(), typeTo);
            return (TTo)expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this);
        }

        public object Activate(Type type, params object[] arguments)
        {
            if (!type.IsResolvableType())
                throw new ArgumentException($"The given type ({type.FullName}) could not be activated on the fly by the container.");

            var resolutionContext = new ResolutionContext(this.GetActiveScopeNames(), this.containerContext,
                this.resolutionStrategy, this == this.containerContext.RootScope,
                dependencyOverrides: this.ProcessDependencyOverrides(arguments));
            var expression = this.expressionFactory.ConstructExpression(resolutionContext, type);
            return expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this);
        }

        public TDisposable AddDisposableTracking<TDisposable>(TDisposable disposable)
            where TDisposable : IDisposable
        {
            Swap.SwapValue(ref this.rootItem, (t1, t2, t3, t4, root) =>
                new DisposableItem { Item = t1, Next = root }, disposable, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return disposable;
        }

        public TService AddWithFinalizer<TService>(TService finalizable, Action<TService> finalizer)
        {
            Swap.SwapValue(ref this.rootFinalizableItem, (t1, t2, t3, t4, root) =>
                new FinalizableItem { Item = t1, Finalizer = f => t2((TService)f), Next = root }, finalizable, finalizer, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return finalizable;
        }

        public object GetOrAddScopedObject(int key, object sync, Func<IResolutionScope, object> factory)
        {
            var item = this.scopedItems.GetOrDefault(key);
            if (item != null) return item;

            lock (sync)
            {
                item = this.scopedItems.GetOrDefault(key);
                if (item != null) return item;

                item = factory(this);
                Swap.SwapValue(ref this.scopedItems, (t1, t2, t3, t4, items) =>
                    items.AddOrUpdate(t1, t2), key, item, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

                return item;
            }
        }

        public void InvalidateDelegateCache()
        {
            this.delegateCache.ServiceDelegates = ImmutableTree<object, Func<IResolutionScope, object>>.Empty;
            this.delegateCache.FactoryDelegates = ImmutableTree<object, Func<IResolutionScope, Delegate>>.Empty;
        }

        public IEnumerable<object> GetActiveScopeNames()
        {
            IResolutionScope current = this;
            while (current != null)
            {
                if (current.Name != null)
                    yield return current.Name;

                current = current.ParentScope;
            }
        }

        public void CheckRuntimeCircularDependencyBarrier(int key, Type type)
        {
            var check = this.circularDependencyBarrier.GetOrDefault(key);
            if (check != null && check.Value)
                throw new CircularDependencyException(type);

            Swap.SwapValue(ref this.circularDependencyBarrier, (t1, t2, t3, t4, barrier) => barrier.AddOrUpdate(t1, new ThreadLocal<bool>(), (old, @new) =>
                { old.Value = true; return old; }), key, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
        }

        public void ResetRuntimeCircularDependencyBarrier(int key)
        {
            var check = this.circularDependencyBarrier.GetOrDefault(key);
            if (check != null)
                check.Value = false;
        }

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

            if (this.circularDependencyBarrier.IsEmpty)
                return;

            foreach (var threadLocal in this.circularDependencyBarrier.Walk())
                threadLocal.Value.Dispose();
        }

        private object Activate(ResolutionContext resolutionContext, Type type, object name = null)
        {
            var expression = this.resolutionStrategy.BuildExpressionForTopLevelRequest(type, name, resolutionContext);
            if (expression == null)
                if (resolutionContext.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type);

            var factory = expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);

            if (resolutionContext.FactoryDelegateCacheEnabled)
                Swap.SwapValue(ref this.delegateCache.ServiceDelegates, (t1, t2, t3, t4, c) =>
                    c.AddOrUpdate(t1, t2), name ?? type, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return factory(this);
        }

        private Delegate ActivateFactoryDelegate(Type type, Type[] parameterTypes, object name, bool nullResultAllowed)
        {
            var resolutionContext = new ResolutionContext(this.GetActiveScopeNames(), this.containerContext,
                    this.resolutionStrategy, this == this.containerContext.RootScope, nullResultAllowed)
                .BeginContextWithFunctionParameters(parameterTypes.Select(p => p.AsParameter()));

            var initExpression = this.resolutionStrategy.BuildExpressionForTopLevelRequest(type, name, resolutionContext);
            if (initExpression == null)
                if (resolutionContext.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type);

            var expression = initExpression.AsLambda(resolutionContext.ParameterExpressions.SelectMany(x => x.Select(i => i.I2)));

            var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
            Swap.SwapValue(ref this.delegateCache.FactoryDelegates, (t1, t2, t3, t4, c) =>
                c.AddOrUpdate(t1, t2), name ?? type, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            return factory(this);
        }

        private HashTree<Type, HashTree<object, Expression>> ProcessDependencyOverrides(object[] dependencyOverrides)
        {
            if (dependencyOverrides == null && this.scopedInstances.IsEmpty)
                return null;

            var result = new HashTree<Type, HashTree<object, Expression>>();

            if (!this.scopedInstances.IsEmpty)
            {
                foreach (var scopedInstance in this.scopedInstances.Walk())
                {
                    var scopedInstanceSubTree = new HashTree<object, Expression>();
                    foreach (var namedScopedInstance in scopedInstance.Value.Walk())
                        scopedInstanceSubTree.Add(namedScopedInstance.Key, namedScopedInstance.Value.AsConstant(), false);

                    result.Add(scopedInstance.Key, scopedInstanceSubTree);
                }
            }

            if (dependencyOverrides == null) return result;

            foreach (var dependencyOverride in dependencyOverrides)
            {
                var type = dependencyOverride.GetType();
                var expression = dependencyOverride.AsConstant();

                var subtree = result.GetOrDefault(type);
                if (subtree == null)
                    result.Add(type, new HashTree<object, Expression>(type, expression));
                else
                    subtree.Add(type, expression, false);

                foreach (var baseType in type.GetRegisterableInterfaceTypes().Concat(type.GetRegisterableBaseTypes()))
                {
                    subtree = result.GetOrDefault(baseType);
                    if (subtree == null)
                        result.Add(baseType, new HashTree<object, Expression>(baseType, expression));
                    else
                        subtree.Add(baseType, expression, false);
                }
            }

            return result;
        }
    }
}
