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
		public object Resolve(Type typeFrom, bool nullResultAllowed = false, object[] dependencyOverrides = null)
		{
			this.ThrowIfDisposed();

			if (dependencyOverrides != null)
				return this.rootScope.BuildAndResolveService(typeFrom, nullResultAllowed: nullResultAllowed, dependencyOverrides: dependencyOverrides);

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom);
			if (cachedFactory != null)
				return cachedFactory(this.rootScope, RequestContext.Empty);

			return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(typeFrom)?.Invoke(this.rootScope, RequestContext.FromOverrides(dependencyOverrides)) ??
				this.rootScope.BuildAndResolveService(typeFrom, nullResultAllowed: nullResultAllowed, dependencyOverrides: dependencyOverrides);
		}

		/// <inheritdoc />
		public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false, object[] dependencyOverrides = null)
		{
			this.ThrowIfDisposed();

			if (dependencyOverrides != null)
				return this.rootScope.BuildAndResolveService(typeFrom, name, nullResultAllowed, dependencyOverrides);

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByValue(name);
			if (cachedFactory != null)
				return cachedFactory(this.rootScope, RequestContext.Empty);

			return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByValue(name)?.Invoke(this.rootScope, RequestContext.FromOverrides(dependencyOverrides)) ??
				this.rootScope.BuildAndResolveService(typeFrom, name, nullResultAllowed, dependencyOverrides);
		}

		/// <inheritdoc />
		public object GetService(Type serviceType)
		{
			this.ThrowIfDisposed();

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(serviceType);
			if (cachedFactory != null)
				return cachedFactory(this.rootScope, RequestContext.Empty);

			return this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(serviceType)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveService(serviceType, nullResultAllowed: true);
		}

		/// <inheritdoc />
		public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides = null)
		{
			this.ThrowIfDisposed();

			var type = typeof(IEnumerable<TKey>);
			if (dependencyOverrides != null)
				return (IEnumerable<TKey>)this.rootScope.BuildAndResolveService(type, dependencyOverrides: dependencyOverrides);

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type);
			if (cachedFactory != null)
				return (IEnumerable<TKey>)cachedFactory(this.rootScope, RequestContext.Empty);

			return (IEnumerable<TKey>)this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.Invoke(this.rootScope, RequestContext.FromOverrides(dependencyOverrides)) ??
				(IEnumerable<TKey>)this.rootScope.BuildAndResolveService(type, dependencyOverrides: dependencyOverrides);
		}

		/// <inheritdoc />
		public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides = null)
		{
			this.ThrowIfDisposed();

			var type = typeof(IEnumerable<>).MakeGenericType(typeFrom);
			if (dependencyOverrides != null)
				return (IEnumerable<object>)this.rootScope.BuildAndResolveService(type, dependencyOverrides: dependencyOverrides);

			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByRef(type);
			if (cachedFactory != null)
				return (IEnumerable<object>)cachedFactory(this.rootScope, RequestContext.Empty);

			return (IEnumerable<object>)this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(type)?.Invoke(this.rootScope, RequestContext.FromOverrides(dependencyOverrides)) ??
				(IEnumerable<object>)this.rootScope.BuildAndResolveService(type, dependencyOverrides: dependencyOverrides);
		}

		/// <inheritdoc />
		public Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes)
		{
			this.ThrowIfDisposed();

			var key = $"{name ?? ""}{string.Join("", parameterTypes.Append(typeFrom).Select(t => t.FullName))}";
			var cachedFactory = this.rootScope.DelegateCache.ServiceDelegates.GetOrDefaultByValue(key);
			if (cachedFactory != null)
				return (Delegate)cachedFactory(this.rootScope, RequestContext.Empty);

			return (Delegate)this.rootScope.DelegateCache.RequestContextAwareDelegates.GetOrDefaultByRef(key)?.Invoke(this.rootScope, RequestContext.Begin()) ??
				this.rootScope.BuildAndResolveFactoryDelegate(typeFrom, parameterTypes, name, key, nullResultAllowed);
		}

		/// <inheritdoc />
		public TTo BuildUp<TTo>(TTo instance)
		{
			this.ThrowIfDisposed();

			var resolutionContext = ResolutionContext.BeginTopLevelContext(this.rootScope.GetActiveScopeNames(),
				this.ContainerContext, true);
			var expression = ExpressionFactory.ConstructBuildUpExpression(resolutionContext, instance.AsConstant(), typeof(TTo));
			return (TTo)expression.CompileDelegate(resolutionContext, this.ContainerContext.ContainerConfiguration)(this.rootScope,
				resolutionContext.RequestConfiguration.RequiresRequestContext ? RequestContext.Begin() : RequestContext.Empty);
		}

		/// <inheritdoc />
		public object Activate(Type type, params object[] arguments)
		{
			this.ThrowIfDisposed();

			if (!type.IsResolvableType())
				throw new ArgumentException($"The given type ({type.FullName}) could not be activated on the fly by the container.");

			var resolutionContext = ResolutionContext.BeginTopLevelContext(this.rootScope.GetActiveScopeNames(), this.ContainerContext, true,
				dependencyOverrides: this.rootScope.ProcessDependencyOverrides(arguments));
			var expression = ExpressionFactory.ConstructExpression(resolutionContext, type);
			return expression.CompileDelegate(resolutionContext, this.ContainerContext.ContainerConfiguration)(this.rootScope,
				resolutionContext.RequestConfiguration.RequiresRequestContext ? RequestContext.Begin() : RequestContext.Empty);
		}

		/// <inheritdoc />
		public bool CanResolve<TFrom>(object name = null) =>
			this.CanResolve(typeof(TFrom), name);

		/// <inheritdoc />
		public bool CanResolve(Type typeFrom, object name = null)
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
		public IDependencyResolver BeginScope(object name = null, bool attachToParent = false)
			=> this.rootScope.BeginScope(name, attachToParent);

		/// <inheritdoc />
		public void PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false, object name = null) =>
			this.rootScope.PutInstanceInScope(typeFrom, instance, withoutDisposalTracking, name);
	}
}
