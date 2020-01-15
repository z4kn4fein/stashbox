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

        private readonly IResolverSupportedResolutionStrategy resolutionStrategy;
        private readonly IExpressionBuilder expressionBuilder;
        private readonly IContainerContext containerContext;

        private int disposed;
        private DisposableItem rootItem = DisposableItem.Empty;
        private FinalizableItem rootFinalizableItem = FinalizableItem.Empty;
        private AvlTree<object> scopedItems = AvlTree<object>.Empty;
        private AvlTreeKeyValue<Type, AvlTreeKeyValue<object, object>> scopedInstances = AvlTreeKeyValue<Type, AvlTreeKeyValue<object, object>>.Empty;
        private AvlTree<ThreadLocal<bool>> circularDependencyBarrier = AvlTree<ThreadLocal<bool>>.Empty;

        private readonly DelegateCache delegateCache;

        public bool HasScopedInstances => !this.scopedInstances.IsEmpty;

        public object Name { get; }

        public IResolutionScope ParentScope { get; }

        private ResolutionScope(IResolverSupportedResolutionStrategy resolutionStrategy,
            IExpressionBuilder expressionBuilder, IContainerContext containerContext,
            DelegateCache delegateCache, object name)
        {
            this.resolutionStrategy = resolutionStrategy;
            this.expressionBuilder = expressionBuilder;
            this.containerContext = containerContext;
            this.Name = name;
            this.delegateCache = delegateCache;
        }

        internal ResolutionScope(IResolverSupportedResolutionStrategy resolutionStrategy,
            IExpressionBuilder expressionBuilder, IContainerContext containerContext)
            : this(resolutionStrategy, expressionBuilder, containerContext,
                  new DelegateCache(), null)
        { }

        private ResolutionScope(IResolverSupportedResolutionStrategy resolutionStrategy, IExpressionBuilder expressionBuilder,
            IContainerContext containerContext, IResolutionScope parent, DelegateCache delegateCache, object name = null)
            : this(resolutionStrategy, expressionBuilder, containerContext, delegateCache, name)
        {
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
            return cachedFactory != null ? cachedFactory(this) : this.ActivateFactoryDelegate(typeFrom, parameterTypes, this, name, nullResultAllowed);
        }

        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false)
        {
            var scope = new ResolutionScope(this.resolutionStrategy, this.expressionBuilder,
                this.containerContext, this, this.delegateCache, name);

            return attachToParent ? this.AddDisposableTracking(scope) : scope;
        }

        public IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object name = null)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(instance, nameof(instance));

            this.AddScopedInstance(typeFrom, instance, name);
            if (!withoutDisposalTracking && instance is IDisposable disposable)
                this.AddDisposableTracking(disposable);

            return this;
        }

        public TTo BuildUp<TTo>(TTo instance)
        {
            var typeTo = instance.GetType();
            var resolutionContext = ResolutionContext.New(this);
            var metaInfo = MetaInformation.GetOrCreateMetaInfo(typeTo);
            var expression = this.expressionBuilder.CreateBasicFillExpression(this.containerContext,
                metaInfo.SelectInjectionMembers(RegistrationContext.Empty,
                    this.containerContext.ContainerConfiguration),
                metaInfo.GetInjectionMethods(RegistrationContext.Empty, this.containerContext.ContainerConfiguration),
                instance.AsConstant(),
                resolutionContext,
                typeTo);
            return (TTo)expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this);
        }

        public object Activate(Type type, params object[] arguments)
        {
            if (!type.IsResolvableType())
                throw new ArgumentException($"The given type ({type.FullName}) could not be activated on the fly by the container.");

            var resolutionContext = ResolutionContext.New(this, dependencyOverrides: arguments);
            var metaInfo = MetaInformation.GetOrCreateMetaInfo(type);
            var expression = this.expressionBuilder.CreateBasicExpression(this.containerContext,
                metaInfo.GetConstructors(RegistrationContext.Empty, this.containerContext.ContainerConfiguration),
                metaInfo.SelectInjectionMembers(RegistrationContext.Empty,
                    this.containerContext.ContainerConfiguration),
                metaInfo.GetInjectionMethods(RegistrationContext.Empty, this.containerContext.ContainerConfiguration),
                resolutionContext,
                type);
            return expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this);
        }

        public TDisposable AddDisposableTracking<TDisposable>(TDisposable disposable)
            where TDisposable : IDisposable
        {
            Swap.SwapValue(ref this.rootItem, (t1, t2, t3, t4, root) =>
                new DisposableItem { Item = t1, Next = root }, disposable, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return disposable;
        }

        public void AddScopedInstance(Type key, object value, object name = null)
        {
            var subKey = name ?? key;
            var newRepo = AvlTreeKeyValue<object, object>.Empty.AddOrUpdate(subKey, value);
            Swap.SwapValue(ref this.scopedInstances, (t1, t2, t3, t4, instances) =>
                instances.AddOrUpdate(t1, t3,
                    (old, @new) => old.AddOrUpdate(t4, t2, true)), key, value, newRepo, subKey);
        }

        public object GetScopedInstanceOrDefault(Type key, object name = null) =>
            this.scopedInstances.GetOrDefault(key)?.GetOrDefault(name ?? key);

        public TService AddWithFinalizer<TService>(TService finalizable, Action<TService> finalizer)
        {
            Swap.SwapValue(ref this.rootFinalizableItem, (t1, t2, t3, t4, root) =>
                new FinalizableItem { Item = t1, Finalizer = f => t2((TService)f), Next = root }, finalizable, finalizer, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return finalizable;
        }

        public object GetOrAddScopedItem(int key, object sync, Func<IResolutionScope, object> factory)
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
            this.delegateCache.ServiceDelegates = AvlTreeKeyValue<object, Func<IResolutionScope, object>>.Empty;
            this.delegateCache.FactoryDelegates = AvlTreeKeyValue<object, Func<IResolutionScope, Delegate>>.Empty;
        }

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

            Swap.SwapValue(ref this.circularDependencyBarrier, (t1, t2, t3, t4, barrier) => barrier.AddOrUpdate(t1, new ThreadLocal<bool>(), (old, @new) =>
                { old.Value = true; return old; }), key, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
        }

        public void ResetRuntimetCircularDependencyBarrier(int key)
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
            if (type == Constants.ResolverType)
                return resolutionContext.ResolutionScope;

#if HAS_SERVICEPROVIDER
            if (type == Constants.ServiceProviderType)
                return resolutionContext.ResolutionScope;
#endif

            if (resolutionContext.ResolutionScope.HasScopedInstances)
            {
                var scopedInstance = resolutionContext.ResolutionScope
                    .GetScopedInstanceOrDefault(type, name);
                if (scopedInstance != null)
                    return scopedInstance;
            }

            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(type, resolutionContext, name);
            if (registration != null)
            {
                var registrationFactory = registration.GetExpression(this.containerContext, resolutionContext, type)?
                    .CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
                if (registrationFactory == null)
                    return null;

                if (resolutionContext.ShouldCacheFactoryDelegate)
                    Swap.SwapValue(ref this.delegateCache.ServiceDelegates, (t1, t2, t3, t4, c) =>
                    c.AddOrUpdate(t1, t2), name ?? type, registrationFactory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

                return registrationFactory(resolutionContext.ResolutionScope);
            }

            var expr = this.resolutionStrategy.BuildResolutionExpressionUsingResolvers(this.containerContext,
                new TypeInformation { Type = type, DependencyName = name }, resolutionContext);
            if (expr == null)
                if (resolutionContext.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type);

            var factory = expr.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);

            if (resolutionContext.ShouldCacheFactoryDelegate)
                Swap.SwapValue(ref this.delegateCache.ServiceDelegates, (t1, t2, t3, t4, c) =>
                c.AddOrUpdate(t1, t2), name ?? type, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return factory(resolutionContext.ResolutionScope);
        }

        private Delegate ActivateFactoryDelegate(Type type,
            Type[] parameterTypes,
            IResolutionScope resolutionScope,
            object name,
            bool nullResultAllowed)
        {
            var resolutionContext = ResolutionContext.New(resolutionScope, nullResultAllowed);
            resolutionContext.AddParameterExpressions(type, parameterTypes.Select(p => p.AsParameter()).ToArray());

            var typeInfo = new TypeInformation { Type = type, DependencyName = name };
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo, resolutionContext);

            var initExpression = registration == null ?
                this.resolutionStrategy.BuildResolutionExpressionUsingResolvers(this.containerContext, typeInfo, resolutionContext) :
                registration.GetExpression(this.containerContext, resolutionContext, type);

            if (initExpression == null)
                if (resolutionContext.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type);

            var expression = initExpression.AsLambda(resolutionContext.ParameterExpressions.SelectMany(x => x));

            var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
            Swap.SwapValue(ref this.delegateCache.FactoryDelegates, (t1, t2, t3, t4, c) =>
                c.AddOrUpdate(t1, t2), name ?? type, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            return factory(resolutionScope);
        }
    }
}
