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
    internal class ResolutionScope : ResolutionScopeBase, IDependencyResolver
    {
        private readonly IActivationContext activationContext;
        private readonly IServiceRegistrator serviceRegistrator;
        private readonly IExpressionBuilder expressionBuilder;
        private readonly IResolutionScope rootScope;

        public ResolutionScope(IActivationContext activationContext, IServiceRegistrator serviceRegistrator,
            IExpressionBuilder expressionBuilder, IResolutionScope rootScope)
        {
            this.activationContext = activationContext;
            this.serviceRegistrator = serviceRegistrator;
            this.expressionBuilder = expressionBuilder;
            this.rootScope = rootScope;
        }

        public TKey Resolve<TKey>(bool nullResultAllowed = false) =>
            (TKey)this.activationContext.Activate(typeof(TKey), this, nullResultAllowed);

        public object Resolve(Type typeFrom, bool nullResultAllowed = false) =>
            this.activationContext.Activate(typeFrom, this, nullResultAllowed);

        public TKey Resolve<TKey>(object name, bool nullResultAllowed = false) =>
            (TKey)this.activationContext.Activate(typeof(TKey), this, name, nullResultAllowed);

        public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false) =>
            this.activationContext.Activate(typeFrom, this, name, nullResultAllowed);

        public IEnumerable<TKey> ResolveAll<TKey>() =>
            (IEnumerable<TKey>)this.activationContext.Activate(typeof(IEnumerable<TKey>), this);

        public IEnumerable<object> ResolveAll(Type typeFrom) =>
            (IEnumerable<object>)this.activationContext.Activate(typeof(IEnumerable<>).MakeGenericType(typeFrom), this);

        public Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes) =>
            this.activationContext.ActivateFactory(typeFrom, parameterTypes, this, name, nullResultAllowed);

        public Func<TService> ResolveFactory<TService>(object name = null, bool nullResultAllowed = false) =>
            this.ResolveFactory(typeof(TService), name, nullResultAllowed) as Func<TService>;

        public Func<T1, TService> ResolveFactory<T1, TService>(object name = null, bool nullResultAllowed = false) =>
            this.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1)) as Func<T1, TService>;

        public Func<T1, T2, TService> ResolveFactory<T1, T2, TService>(object name = null, bool nullResultAllowed = false) =>
            this.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1), typeof(T2)) as Func<T1, T2, TService>;

        public Func<T1, T2, T3, TService> ResolveFactory<T1, T2, T3, TService>(object name = null, bool nullResultAllowed = false) =>
            this.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1), typeof(T2), typeof(T3)) as Func<T1, T2, T3, TService>;

        public Func<T1, T2, T3, T4, TService> ResolveFactory<T1, T2, T3, T4, TService>(object name = null, bool nullResultAllowed = false) =>
            this.ResolveFactory(typeof(TService), name, nullResultAllowed, typeof(T1), typeof(T2), typeof(T3), typeof(T4)) as Func<T1, T2, T3, T4, TService>;

        public IDependencyResolver BeginScope() => new ResolutionScope(this.activationContext, this.serviceRegistrator,
            this.expressionBuilder, this.rootScope);

        public IDependencyResolver PutInstanceInScope<TFrom>(TFrom instance, bool withoutDisposalTracking = false) =>
            this.PutInstanceInScope(typeof(TFrom), instance, withoutDisposalTracking);

        public IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false)
        {
            Shield.EnsureNotNull(typeFrom, nameof(typeFrom));
            Shield.EnsureNotNull(instance, nameof(instance));

            base.AddScopedInstance(typeFrom, instance);
            if (!withoutDisposalTracking && instance is IDisposable disposable)
                base.AddDisposableTracking(disposable);

            return this;
        }

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance)
        {
            var typeTo = instance.GetType();
            var registration = this.serviceRegistrator.PrepareContext(typeTo, typeTo);
            var expr = this.expressionBuilder.CreateFillExpression(registration.CreateServiceRegistration(false),
                Expression.Constant(instance), ResolutionInfo.New(this, this.rootScope), typeTo);
            var factory = expr.CompileDelegate(Constants.ScopeExpression);
            return (TTo)factory(this);
        }
    }
}
