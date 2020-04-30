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

        private readonly IResolverSupportedResolutionStrategy resolutionStrategy;
        private readonly IExpressionBuilder expressionBuilder;
        private readonly IContainerContext containerContext;

        private int disposed;
        private DisposableItem rootItem = DisposableItem.Empty;
        private FinalizableItem rootFinalizableItem = FinalizableItem.Empty;
        private ImmutableTree<object> scopedItems = ImmutableTree<object>.Empty;
        private ImmutableTree<Type, ImmutableTree<object, object>> scopedInstances = ImmutableTree<Type, ImmutableTree<object, object>>.Empty;
        private ImmutableTree<ThreadLocal<bool>> circularDependencyBarrier = ImmutableTree<ThreadLocal<bool>>.Empty;

        private readonly DelegateCache delegateCache;

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
            return cachedFactory != null
                ? cachedFactory(this)
                : this.Activate(ResolutionContext.New(this.GetActiveScopeNames(), this.containerContext,
                    nullResultAllowed, this.ProcessDependencyOverrides(dependencyOverrides)), typeFrom);
        }

        public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false, object[] dependencyOverrides = null)
        {
            var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefault(name);
            return cachedFactory != null
                ? cachedFactory(this)
                : this.Activate(ResolutionContext.New(this.GetActiveScopeNames(), this.containerContext,
                    nullResultAllowed, this.ProcessDependencyOverrides(dependencyOverrides)), typeFrom, name);
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
            var scope = new ResolutionScope(this.resolutionStrategy, this.expressionBuilder,
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

            return this;
        }

        public TTo BuildUp<TTo>(TTo instance)
        {
            var typeTo = instance.GetType();
            var resolutionContext = ResolutionContext.New(this.GetActiveScopeNames(), this.containerContext);
            var expression = this.expressionBuilder.ConstructBuildUpExpression(this.containerContext,
                RegistrationContext.Empty,
                resolutionContext,
                instance.AsConstant(),
                typeTo);
            return (TTo)expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this);
        }

        public object Activate(Type type, params object[] arguments)
        {
            if (!type.IsResolvableType())
                throw new ArgumentException($"The given type ({type.FullName}) could not be activated on the fly by the container.");

            var resolutionContext = ResolutionContext.New(this.GetActiveScopeNames(),
                this.containerContext, dependencyOverrides: this.ProcessDependencyOverrides(arguments));
            var expression = this.expressionBuilder.ConstructExpression(
                this.containerContext,
                RegistrationContext.Empty,
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

        public List<object> GetActiveScopeNames()
        {
            var names = new List<object>();
            IResolutionScope current = this;

            while (current != null)
            {
                if (current.Name != null)
                    names.Add(current.Name);

                current = current.ParentScope;
            }

            return names;
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
            if (type == Constants.ResolverType)
                return this;

#if HAS_SERVICEPROVIDER
            if (type == Constants.ServiceProviderType)
                return this;
#endif

            var exprOverride = resolutionContext.GetExpressionOverrideOrDefault(type, name);
            if (exprOverride != null && exprOverride is ConstantExpression constantExpression)
                return constantExpression.Value;

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

                return registrationFactory(this);
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

            return factory(this);
        }

        private Delegate ActivateFactoryDelegate(Type type,
            Type[] parameterTypes,
            object name,
            bool nullResultAllowed)
        {
            var resolutionContext = ResolutionContext.New(this.GetActiveScopeNames(), this.containerContext, nullResultAllowed);
            resolutionContext.AddParameterExpressions(parameterTypes.Select(p => p.AsParameter()).ToArray());

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

            var expression = initExpression.AsLambda(resolutionContext.ParameterExpressions.SelectMany(x => x.Select(i => i.Value)));

            var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
            Swap.SwapValue(ref this.delegateCache.FactoryDelegates, (t1, t2, t3, t4, c) =>
                c.AddOrUpdate(t1, t2), name ?? type, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            return factory(this);
        }

        private HashTree<Type, HashTree<object, Expression>> ProcessDependencyOverrides(object[] dependencyOverrides)
        {
            if (dependencyOverrides == null && this.scopedInstances.IsEmpty)
                return null;

            var result = HashTree<Type, HashTree<object, Expression>>.Empty;

            if (!this.scopedInstances.IsEmpty)
            {
                foreach (var scopedInstance in this.scopedInstances.Walk())
                {
                    var scopedInstanceSubTree = HashTree<object, Expression>.Empty;
                    foreach (var namedScopedInstace in scopedInstance.Value.Walk())
                        scopedInstanceSubTree.Add(namedScopedInstace.Key, namedScopedInstace.Value.AsConstant());

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
                    subtree.Add(type, expression);

                foreach (var baseType in type.GetRegisterableInterfaceTypes().Concat(type.GetRegisterableBaseTypes()))
                {
                    subtree = result.GetOrDefault(baseType);
                    if (subtree == null)
                        result.Add(baseType, new HashTree<object, Expression>(baseType, expression));
                    else
                        subtree.Add(baseType, expression);
                }
            }

            return result;
        }
    }
}
