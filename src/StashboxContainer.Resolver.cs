using Stashbox.Exceptions;
using Stashbox.Expressions;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Stashbox
{
    public partial class StashboxContainer
    {
		/// <inheritdoc />
		public object? Resolve(Type typeFrom, bool nullResultAllowed, object[]? dependencyOverrides = null) =>
			nullResultAllowed ? this.ResolveOrDefault(typeFrom, dependencyOverrides) : this.Resolve(typeFrom, dependencyOverrides);

		/// <inheritdoc />
		public object? Resolve(Type typeFrom, object name, bool nullResultAllowed, object[]? dependencyOverrides = null) =>
			nullResultAllowed ? this.ResolveOrDefault(typeFrom, name, dependencyOverrides) : this.Resolve(typeFrom, name, dependencyOverrides);

		/// <inheritdoc />
		public object Resolve(Type typeFrom, object[]? dependencyOverrides = null)
		{
			this.ThrowIfDisposed();

			if (dependencyOverrides != null)
				return this.rootScope.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: dependencyOverrides);

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom);
			if (cachedFactory != null)
				return cachedFactory(this.rootScope, RequestContext.Empty);

			return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveService(typeFrom, name: null, dependencyOverrides: null);
		}

		/// <inheritdoc />
		public object Resolve(Type typeFrom, object name, object[]? dependencyOverrides = null)
		{
			this.ThrowIfDisposed();

			if (dependencyOverrides != null)
				return this.rootScope.BuildAndResolveService(typeFrom, name, dependencyOverrides: dependencyOverrides);

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByValue(name);
			if (cachedFactory != null)
				return cachedFactory(this.rootScope, RequestContext.Empty);

			return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByValue(name)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveService(typeFrom, name, dependencyOverrides: null);
		}

		/// <inheritdoc />
		public object? ResolveOrDefault(Type typeFrom, object[]? dependencyOverrides = null)
		{
			this.ThrowIfDisposed();

			if (dependencyOverrides != null)
				return this.rootScope.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: dependencyOverrides);

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom);
			if (cachedFactory != null)
				return cachedFactory(this.rootScope, RequestContext.Empty);

			return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveServiceOrDefault(typeFrom, name: null, dependencyOverrides: null);
		}

		/// <inheritdoc />
		public object? ResolveOrDefault(Type typeFrom, object name, object[]? dependencyOverrides = null)
		{
			this.ThrowIfDisposed();

			if (dependencyOverrides != null)
				return this.rootScope.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides: dependencyOverrides);

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByValue(name);
			if (cachedFactory != null)
				return cachedFactory(this.rootScope, RequestContext.Empty);

			return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByValue(name)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveServiceOrDefault(typeFrom, name, dependencyOverrides: null);
		}

		/// <inheritdoc />
		public object? GetService(Type serviceType)
		{
			this.ThrowIfDisposed();

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(serviceType);
			if (cachedFactory != null)
				return cachedFactory(this.rootScope, RequestContext.Empty);

			return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(serviceType)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveServiceOrDefault(serviceType, name: null, dependencyOverrides: null);
		}

		/// <inheritdoc />
		public IEnumerable<TKey> ResolveAll<TKey>(object[]? dependencyOverrides = null)
		{
			this.ThrowIfDisposed();

			var type = typeof(IEnumerable<TKey>);
			if (dependencyOverrides != null)
				return (IEnumerable<TKey>)this.rootScope.BuildAndResolveService(type, name: null, dependencyOverrides: dependencyOverrides);

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type);
			if (cachedFactory != null)
				return (IEnumerable<TKey>)cachedFactory(this.rootScope, RequestContext.Empty);

			return (IEnumerable<TKey>)(this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveService(type, name: null, dependencyOverrides: null));
		}

		/// <inheritdoc />
		public IEnumerable<object> ResolveAll(Type typeFrom, object[]? dependencyOverrides = null)
		{
			this.ThrowIfDisposed();

			var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
			if (dependencyOverrides != null)
				return (IEnumerable<object>)this.rootScope.BuildAndResolveService(type, name: null, dependencyOverrides: dependencyOverrides);

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type);
			if (cachedFactory != null)
				return (IEnumerable<object>)cachedFactory(this.rootScope, RequestContext.Empty);

			return (IEnumerable<object>)(this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveService(type, name: null, dependencyOverrides: null));
		}

		/// <inheritdoc />
		public Delegate ResolveFactory(Type typeFrom, object? name = null, params Type[] parameterTypes)
		{
			this.ThrowIfDisposed();

			var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByValue(key);
			if (cachedFactory != null)
				return (Delegate)cachedFactory(this.rootScope, RequestContext.Empty);

			return (Delegate?)this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(key)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveFactoryDelegate(typeFrom, parameterTypes, name, key);
		}

		/// <inheritdoc />
		public Delegate? ResolveFactoryOrDefault(Type typeFrom, object? name = null, params Type[] parameterTypes)
		{
			this.ThrowIfDisposed();

			var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByValue(key);
			if (cachedFactory != null)
				return (Delegate)cachedFactory(this.rootScope, RequestContext.Empty);

			return (Delegate?)this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(key)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveFactoryDelegateOrDefault(typeFrom, parameterTypes, name, key);
		}

		/// <inheritdoc />
		public TTo BuildUp<TTo>(TTo instance)
			where TTo : class
		{
			this.ThrowIfDisposed();

			var resolutionContext = ResolutionContext.BeginTopLevelContext(this.rootScope.GetActiveScopeNames(),
				this.ContainerContext, true);
			var expression = ExpressionFactory.ConstructBuildUpExpression(resolutionContext, instance.AsConstant(), typeof(TTo));
			return (TTo)expression.CompileDelegate(resolutionContext, this.ContainerContext.ContainerConfiguration)(this.rootScope,
				resolutionContext.RequestConfiguration.RequiresRequestContext ? RequestContext.Begin() : RequestContext.Empty);
		}

		/// <inheritdoc />
		public object Activate(Type type, params object[] arguments) =>
			this.rootScope.Activate(type, arguments);

		/// <inheritdoc />
		public bool CanResolve<TFrom>(object? name = null) =>
			this.CanResolve(typeof(TFrom), name);

		/// <inheritdoc />
		public bool CanResolve(Type typeFrom, object? name = null)
		{
			this.ThrowIfDisposed();
			Shield.EnsureNotNull(typeFrom, nameof(typeFrom));

			return this.ContainerContext.ResolutionStrategy
				.IsTypeResolvable(ResolutionContext.BeginTopLevelContext(this.rootScope.GetActiveScopeNames(), this.ContainerContext, false),
					new TypeInformation(typeFrom, name));
		}

		/// <inheritdoc />
		public ValueTask InvokeAsyncInitializers(CancellationToken token = default) =>
			this.rootScope.InvokeAsyncInitializers(token);

		/// <inheritdoc />
		public IDependencyResolver BeginScope(object? name = null, bool attachToParent = false)
			=> this.rootScope.BeginScope(name, attachToParent);

		/// <inheritdoc />
		public void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object? name = null) =>
			this.rootScope.PutInstanceInScope(typeFrom, instance, withoutDisposalTracking, name);
	}
}
