using Stashbox.Exceptions;
using Stashbox.Expressions;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox
{
    internal partial class ResolutionScope
    {
        /// <inheritdoc />
        public object Resolve(Type typeFrom)
        {
            this.ThrowIfDisposed();

            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom);
            if (cachedFactory != null)
                return cachedFactory(this, RequestContext.Empty);

            return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: null);
        }

        /// <inheritdoc />
        public object Resolve(Type typeFrom, object[] dependencyOverrides)
        {
            this.ThrowIfDisposed();

            return this.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public object Resolve(Type typeFrom, object name)
        {
            this.ThrowIfDisposed();

            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByValue(name);
            if (cachedFactory != null)
                return cachedFactory(this, RequestContext.Empty);

            return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByValue(name)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveService(typeFrom, name, dependencyOverrides: null);
        }


        /// <inheritdoc />
        public object Resolve(Type typeFrom, object name, object[] dependencyOverrides)
        {
            this.ThrowIfDisposed();

            return this.BuildAndResolveService(typeFrom, name, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public object? ResolveOrDefault(Type typeFrom)
        {
            this.ThrowIfDisposed();

            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom);
            if (cachedFactory != null)
                return cachedFactory(this, RequestContext.Empty);

            return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: null);
        }

        /// <inheritdoc />
        public object? ResolveOrDefault(Type typeFrom, object[] dependencyOverrides)
        {
            this.ThrowIfDisposed();

            return this.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public object? ResolveOrDefault(Type typeFrom, object name)
        {
            this.ThrowIfDisposed();

            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByValue(name);
            if (cachedFactory != null)
                return cachedFactory(this, RequestContext.Empty);

            return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByValue(name)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides: null);
        }

        /// <inheritdoc />
        public object? ResolveOrDefault(Type typeFrom, object name, object[] dependencyOverrides)
        {
            this.ThrowIfDisposed();

            return this.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public object? GetService(Type serviceType)
        {
            this.ThrowIfDisposed();

            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(serviceType);
            if (cachedFactory != null)
                return cachedFactory(this, RequestContext.Empty);

            return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(serviceType)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveServiceOrDefault(serviceType, name: null, dependencyOverrides: null);
        }

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>()
        {
            this.ThrowIfDisposed();

            var type = typeof(IEnumerable<TKey>);
            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type);
            if (cachedFactory != null)
                return (IEnumerable<TKey>)cachedFactory(this, RequestContext.Empty);

            return (IEnumerable<TKey>)(this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveService(type, name: null, dependencyOverrides: null));
        }

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(object name)
        {
            this.ThrowIfDisposed();

            var type = typeof(IEnumerable<TKey>);
            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(name);
            if (cachedFactory != null)
                return (IEnumerable<TKey>)cachedFactory(this, RequestContext.Empty);

            return (IEnumerable<TKey>)(this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(name)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveService(type, name: name, dependencyOverrides: null));
        }

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides)
        {
            this.ThrowIfDisposed();

            var type = typeof(IEnumerable<TKey>);
            return (IEnumerable<TKey>)this.BuildAndResolveService(type, name: null, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(object name, object[] dependencyOverrides)
        {
            this.ThrowIfDisposed();

            return (IEnumerable<TKey>)this.BuildAndResolveService(typeof(IEnumerable<TKey>), name: name, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom)
        {
            this.ThrowIfDisposed();

            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type);
            if (cachedFactory != null)
                return (IEnumerable<object>)cachedFactory(this, RequestContext.Empty);

            return (IEnumerable<object>)(this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveService(type, name: null, dependencyOverrides: null));
        }

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, object name)
        {
            this.ThrowIfDisposed();

            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(name);
            if (cachedFactory != null)
                return (IEnumerable<object>)cachedFactory(this, RequestContext.Empty);

            return (IEnumerable<object>)(this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(name)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveService(type, name: name, dependencyOverrides: null));
        }

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides)
        {
            this.ThrowIfDisposed();

            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            return (IEnumerable<object>)this.BuildAndResolveService(type, name: null, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, object name, object[] dependencyOverrides)
        {
            this.ThrowIfDisposed();

            return (IEnumerable<object>)this.BuildAndResolveService(typeof(IEnumerable<>).MakeGenericType(typeFrom),
                name: name, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, object? name = null, params Type[] parameterTypes)
        {
            this.ThrowIfDisposed();

            var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByValue(key);
            if (cachedFactory != null)
                return (Delegate)cachedFactory(this, RequestContext.Empty);

            return (Delegate?)this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(key)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveFactoryDelegate(typeFrom, parameterTypes, name, key);
        }

        /// <inheritdoc />
        public Delegate? ResolveFactoryOrDefault(Type typeFrom, object? name = null, params Type[] parameterTypes)
        {
            this.ThrowIfDisposed();

            var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByValue(key);
            if (cachedFactory != null)
                return (Delegate)cachedFactory(this, RequestContext.Empty);

            return (Delegate?)this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(key)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveFactoryDelegateOrDefault(typeFrom, parameterTypes, name, key);
        }

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance)
            where TTo : class
        {
            this.ThrowIfDisposed();

            var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(),
                this.containerContext, this.ParentScope == null);
            var expression = ExpressionFactory.ConstructBuildUpExpression(resolutionContext, instance.AsConstant(), typeof(TTo));
            return (TTo)expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this,
                resolutionContext.RequestConfiguration.RequiresRequestContext
                ? RequestContext.Begin()
                : RequestContext.Empty);
        }

        /// <inheritdoc />
        public object Activate(Type type, params object[] arguments)
        {
            this.ThrowIfDisposed();

            if (!type.IsResolvableType())
                throw new ArgumentException($"The given type ({type.FullName}) could not be activated on the fly by the container.");

            var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext, this.ParentScope == null,
                arguments, this.lateKnownInstances);
            var expression = ExpressionFactory.ConstructExpression(resolutionContext, type);
            return expression?.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this,
                resolutionContext.RequestConfiguration.RequiresRequestContext
                ? RequestContext.Begin()
                : RequestContext.Empty) ?? throw new ResolutionFailedException(type);
        }

        /// <inheritdoc />
        public bool CanResolve<TFrom>(object? name = null) =>
            this.CanResolve(typeof(TFrom), name);

        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, object? name = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));

            return this.containerContext.ResolutionStrategy
                .IsTypeResolvable(ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext, false),
                    new TypeInformation(typeFrom, name));
        }

        /// <inheritdoc />
        public IDependencyResolver BeginScope(object? name = null, bool attachToParent = false)
        {
            this.ThrowIfDisposed();

            var scope = new ResolutionScope(this, this.containerContext, this.delegateCacheProvider, name);

            if (attachToParent)
                this.AddDisposableTracking(scope);

            return scope;
        }

        /// <inheritdoc />
        public void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object? name = null)
        {
            this.ThrowIfDisposed();

            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(instance, nameof(instance));

            var key = name ?? typeFrom;
            Swap.SwapValue(ref this.lateKnownInstances, (t1, t2, _, _, instances) =>
                instances.AddOrUpdate(t1, t2, false), key, instance, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            if (!withoutDisposalTracking && instance is IDisposable disposable)
                this.AddDisposableTracking(disposable);

            this.DelegateCache = new DelegateCache();
        }

        internal object BuildAndResolveService(Type type, object? name, object[]? dependencyOverrides)
        {
            var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
                    this.ParentScope == null, dependencyOverrides, this.lateKnownInstances);

            var serviceContext = this.containerContext.ResolutionStrategy
                .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
            if (serviceContext.IsEmpty())
                throw new ResolutionFailedException(type, name);

            var factory = serviceContext.ServiceExpression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
            return StoreAndInvokeServiceDelegate(name ?? type, factory, resolutionContext, dependencyOverrides);
        }

        internal object? BuildAndResolveServiceOrDefault(Type type, object? name, object[]? dependencyOverrides)
        {
            var resolutionContext = ResolutionContext.BeginNullableTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
                    this.ParentScope == null, dependencyOverrides, this.lateKnownInstances);

            var serviceContext = this.containerContext.ResolutionStrategy.BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
            if (serviceContext.IsEmpty())
                return null;

            var factory = serviceContext.ServiceExpression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
            return StoreAndInvokeServiceDelegate(name ?? type, factory, resolutionContext, dependencyOverrides);
        }

        internal Delegate BuildAndResolveFactoryDelegate(Type type, Type[] parameterTypes, object? name, string key)
        {
            var resolutionContext = ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
                    this.ParentScope == null, initialParameters: parameterTypes.AsParameters());

            var serviceContext = this.containerContext.ResolutionStrategy
                .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
            if (serviceContext.IsEmpty())
                throw new ResolutionFailedException(type, name);

            var expression = serviceContext.ServiceExpression.AsLambda(resolutionContext.ParameterExpressions
                .SelectMany(x => x.Select(i => i.I2)));

            var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
            return (Delegate)StoreAndInvokeServiceDelegate(key, factory, resolutionContext);
        }

        internal Delegate? BuildAndResolveFactoryDelegateOrDefault(Type type, Type[] parameterTypes, object? name, string key)
        {
            var resolutionContext = ResolutionContext.BeginNullableTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
                    this.ParentScope == null, initialParameters: parameterTypes.AsParameters());

            var serviceContext = this.containerContext.ResolutionStrategy
                .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
            if (serviceContext.IsEmpty())
                return null;

            var expression = serviceContext.ServiceExpression.AsLambda(resolutionContext.ParameterExpressions
                .SelectMany(x => x.Select(i => i.I2)));

            var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
            return (Delegate)StoreAndInvokeServiceDelegate(key, factory, resolutionContext);
        }

        private object StoreAndInvokeServiceDelegate(object key, Func<IResolutionScope, IRequestContext, object> factory,
            ResolutionContext resolutionContext, object[]? dependencyOverrides = null)
        {
            if (resolutionContext.RequestConfiguration.FactoryDelegateCacheEnabled)
                Swap.SwapValue(ref resolutionContext.RequestConfiguration.RequiresRequestContext
                ? ref this.DelegateCache.RequestContextAwareDelegates
                : ref this.DelegateCache.ServiceDelegates, (t1, t2, _, _, c) =>
                    c.AddOrUpdate(t1, t2, false), key, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return factory(this, resolutionContext.RequestConfiguration.RequiresRequestContext || dependencyOverrides != null
                ? RequestContext.FromOverrides(dependencyOverrides)
                : RequestContext.Empty);
        }
    }
}
