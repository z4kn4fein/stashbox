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
		public object Resolve(Type typeFrom, bool nullResultAllowed = false, object[] dependencyOverrides = null)
        {
            this.ThrowIfDisposed();

            if (dependencyOverrides != null)
                return this.BuildAndResolveService(typeFrom, nullResultAllowed: nullResultAllowed, dependencyOverrides: dependencyOverrides);

            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom);
            if (cachedFactory != null)
                return cachedFactory(this, RequestContext.Empty);

            return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.Invoke(this, RequestContext.FromOverrides(dependencyOverrides)) ??
                this.BuildAndResolveService(typeFrom, nullResultAllowed: nullResultAllowed, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false, object[] dependencyOverrides = null)
        {
            this.ThrowIfDisposed();

            if (dependencyOverrides != null)
                return this.BuildAndResolveService(typeFrom, name, nullResultAllowed, dependencyOverrides);

            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByValue(name);
            if (cachedFactory != null)
                return cachedFactory(this, RequestContext.Empty);

            return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByValue(name)?.Invoke(this, RequestContext.FromOverrides(dependencyOverrides)) ??
                this.BuildAndResolveService(typeFrom, name, nullResultAllowed, dependencyOverrides);
        }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            this.ThrowIfDisposed();

            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(serviceType);
            if (cachedFactory != null)
                return cachedFactory(this, RequestContext.Empty);

            return this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(serviceType)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveService(serviceType, nullResultAllowed: true);
        }

        /// <inheritdoc />
        public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides = null)
        {
            this.ThrowIfDisposed();

            var type = typeof(IEnumerable<TKey>);
            if (dependencyOverrides != null)
                return (IEnumerable<TKey>)this.BuildAndResolveService(type, dependencyOverrides: dependencyOverrides);

            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type);
            if (cachedFactory != null)
                return (IEnumerable<TKey>)cachedFactory(this, RequestContext.Empty);

            return (IEnumerable<TKey>)this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.Invoke(this, RequestContext.FromOverrides(dependencyOverrides)) ??
                (IEnumerable<TKey>)this.BuildAndResolveService(type, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides = null)
        {
            this.ThrowIfDisposed();

            var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
            if (dependencyOverrides != null)
                return (IEnumerable<object>)this.BuildAndResolveService(type, dependencyOverrides: dependencyOverrides);

            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type);
            if (cachedFactory != null)
                return (IEnumerable<object>)cachedFactory(this, RequestContext.Empty);

            return (IEnumerable<object>)this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.Invoke(this, RequestContext.FromOverrides(dependencyOverrides)) ??
                (IEnumerable<object>)this.BuildAndResolveService(type, dependencyOverrides: dependencyOverrides);
        }

        /// <inheritdoc />
        public Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes)
        {
            this.ThrowIfDisposed();

            var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
            var cachedFactory = this.DelegateCache.ServiceDelegates.GetOrDefaultByValue(key);
            if (cachedFactory != null)
                return (Delegate)cachedFactory(this, RequestContext.Empty);

            return (Delegate)this.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(key)?.Invoke(this, RequestContext.Begin()) ??
                this.BuildAndResolveFactoryDelegate(typeFrom, parameterTypes, name, key, nullResultAllowed);
        }

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance)
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
                dependencyOverrides: this.ProcessDependencyOverrides(arguments));
            var expression = ExpressionFactory.ConstructExpression(resolutionContext, type);
            return expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this,
                resolutionContext.RequestConfiguration.RequiresRequestContext 
                ? RequestContext.Begin() 
                : RequestContext.Empty);
        }

        /// <inheritdoc />
        public bool CanResolve<TFrom>(object name = null) =>
            this.CanResolve(typeof(TFrom), name);

        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, object name = null)
        {
            this.ThrowIfDisposed();
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));

            return this.containerContext.ResolutionStrategy
                .IsTypeResolvable(ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext, false),
                    new TypeInformation(typeFrom, name));
        }

        /// <inheritdoc />
        public IDependencyResolver BeginScope(object name = null, bool attachToParent = false)
        {
            this.ThrowIfDisposed();

            var scope = new ResolutionScope(this, this.containerContext, this.delegateCacheProvider, name);

            if (attachToParent)
                this.AddDisposableTracking(scope);

            return scope;
        }

        /// <inheritdoc />
        public void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object name = null)
        {
            this.ThrowIfDisposed();

            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(instance, nameof(instance));

            var key = name ?? typeFrom;
            Swap.SwapValue(ref this.lateKnownInstances, (t1, t2, _, _, instances) =>
                instances.AddOrUpdate(t1, t2, false), key, instance, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            if (!withoutDisposalTracking && instance is IDisposable disposable)
                this.AddDisposableTracking(disposable);

            this.InvalidateDelegateCache();
        }

        internal object BuildAndResolveService(Type type, object name = null, bool nullResultAllowed = false, object[] dependencyOverrides = null)
        {
            var resolutionContext = nullResultAllowed
                ? ResolutionContext.BeginNullableTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
                    this.ParentScope == null, this.ProcessDependencyOverrides(dependencyOverrides))
                : ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
                    this.ParentScope == null, this.ProcessDependencyOverrides(dependencyOverrides));

            var expression = this.containerContext.ResolutionStrategy
                .BuildExpressionForType(resolutionContext, new TypeInformation(type, name))?.ServiceExpression;
            if (expression == null)
                return nullResultAllowed ? null : throw new ResolutionFailedException(type, name);

            var factory = expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);

            if (resolutionContext.RequestConfiguration.FactoryDelegateCacheEnabled)
                Swap.SwapValue(ref resolutionContext.RequestConfiguration.RequiresRequestContext
                ? ref this.DelegateCache.RequestContextAwareDelegates
                : ref this.DelegateCache.ServiceDelegates, (t1, t2, _, _, c) =>
                    c.AddOrUpdate(t1, t2, false), name ?? type, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return factory(this, resolutionContext.RequestConfiguration.RequiresRequestContext || dependencyOverrides != null 
                ? RequestContext.FromOverrides(dependencyOverrides) 
                : RequestContext.Empty);
        }

        internal Delegate BuildAndResolveFactoryDelegate(Type type, Type[] parameterTypes, object name, string key, bool nullResultAllowed)
        {
            var resolutionContext = nullResultAllowed
                ? ResolutionContext.BeginNullableTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
                    this.ParentScope == null, initialParameters: parameterTypes.AsParameters())
                : ResolutionContext.BeginTopLevelContext(this.GetActiveScopeNames(), this.containerContext,
                    this.ParentScope == null, initialParameters: parameterTypes.AsParameters());

            var initExpression = this.containerContext.ResolutionStrategy
                .BuildExpressionForType(resolutionContext, new TypeInformation(type, name))?.ServiceExpression;
            if (initExpression == null)
                return nullResultAllowed ? null : throw new ResolutionFailedException(type, name);

            var expression = initExpression.AsLambda(resolutionContext.ParameterExpressions.SelectMany(x => x.Select(i => i.I2)));

            var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);

            if (resolutionContext.RequestConfiguration.FactoryDelegateCacheEnabled)
                Swap.SwapValue(ref resolutionContext.RequestConfiguration.RequiresRequestContext
                ? ref this.DelegateCache.RequestContextAwareDelegates
                : ref this.DelegateCache.ServiceDelegates, (t1, t2, _, _, c) =>
                    c.AddOrUpdate(t1, t2, false), key, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return factory(this, resolutionContext.RequestConfiguration.RequiresRequestContext 
                ? RequestContext.Begin() 
                : RequestContext.Empty);
        }
    }
}
