using Stashbox.Exceptions;
using Stashbox.Expressions;
using Stashbox.Resolution;
using Stashbox.Utils;
using Stashbox.Utils.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox
{
    internal partial class ResolutionScope
    {
        public object Resolve(Type typeFrom) => 
            this.Resolve(typeFrom, false, null);

        public object Resolve(Type typeFrom, bool nullResultAllowed = false, object[] dependencyOverrides = null)
        {
            this.ThrowIfDisposed();

            if (dependencyOverrides != null)
                return this.Activate(new ResolutionContext(this.GetActiveScopeNames(), this.containerContext,
                    this == this.containerContext.RootScope, nullResultAllowed, false,
                    this.ProcessDependencyOverrides(dependencyOverrides)), typeFrom);

            var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefaultByRef(typeFrom);
            return cachedFactory != null
                ? cachedFactory(this)
                : this.Activate(new ResolutionContext(this.GetActiveScopeNames(), this.containerContext,
                    this == this.containerContext.RootScope, nullResultAllowed, false,
                    this.ProcessDependencyOverrides(dependencyOverrides)), typeFrom);
        }

        public object Resolve(Type typeFrom, object name, bool nullResultAllowed = false, object[] dependencyOverrides = null)
        {
            this.ThrowIfDisposed();

            if (dependencyOverrides != null)
                return this.Activate(new ResolutionContext(this.GetActiveScopeNames(), this.containerContext,
                    this == this.containerContext.RootScope, nullResultAllowed, false,
                    this.ProcessDependencyOverrides(dependencyOverrides)), typeFrom, name);

            var cachedFactory = this.delegateCache.ServiceDelegates.GetOrDefaultByValue(name);
            return cachedFactory != null
                ? cachedFactory(this)
                : this.Activate(new ResolutionContext(this.GetActiveScopeNames(), this.containerContext,
                    this == this.containerContext.RootScope, nullResultAllowed, false,
                    this.ProcessDependencyOverrides(dependencyOverrides)), typeFrom, name);
        }

        public object GetService(Type serviceType) =>
            this.Resolve(serviceType, true);

        public IEnumerable<TKey> ResolveAll<TKey>(object[] dependencyOverrides = null) =>
            (IEnumerable<TKey>)this.Resolve(typeof(IEnumerable<TKey>), dependencyOverrides: dependencyOverrides);

        public IEnumerable<object> ResolveAll(Type typeFrom, object[] dependencyOverrides = null) =>
            (IEnumerable<object>)this.Resolve(typeof(IEnumerable<>).MakeGenericType(typeFrom), dependencyOverrides: dependencyOverrides);

        public Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes)
        {
            this.ThrowIfDisposed();

            var key = name ?? typeFrom;
            var cachedFactory = this.delegateCache.FactoryDelegates.GetOrDefaultByValue(key);
            return cachedFactory != null ? cachedFactory(this) : this.ActivateFactoryDelegate(typeFrom, parameterTypes, name, nullResultAllowed);
        }

        public TTo BuildUp<TTo>(TTo instance)
        {
            this.ThrowIfDisposed();

            var resolutionContext = new ResolutionContext(this.GetActiveScopeNames(), this.containerContext, this == this.containerContext.RootScope);
            var expression = ExpressionFactory.ConstructBuildUpExpression(resolutionContext, instance.AsConstant(), typeof(TTo));
            return (TTo)expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this);
        }

        public object Activate(Type type, params object[] arguments)
        {
            this.ThrowIfDisposed();

            if (!type.IsResolvableType())
                throw new ArgumentException($"The given type ({type.FullName}) could not be activated on the fly by the container.");

            var resolutionContext = new ResolutionContext(this.GetActiveScopeNames(), this.containerContext, this == this.containerContext.RootScope,
                dependencyOverrides: this.ProcessDependencyOverrides(arguments));
            var expression = ExpressionFactory.ConstructExpression(resolutionContext, type);
            return expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration)(this);
        }

        private object Activate(ResolutionContext resolutionContext, Type type, object name = null)
        {
            var expression = this.containerContext.ResolutionStrategy
                .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
            if (expression == null)
                if (resolutionContext.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type, name);

            var factory = expression.CompileDelegate(resolutionContext, this.containerContext.ContainerConfiguration);

            if (resolutionContext.FactoryDelegateCacheEnabled)
                Swap.SwapValue(ref this.delegateCache.ServiceDelegates, (t1, t2, t3, t4, c) =>
                    c.AddOrUpdate(t1, t2, false), name ?? type, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);

            return factory(this);
        }

        private Delegate ActivateFactoryDelegate(Type type, Type[] parameterTypes, object name, bool nullResultAllowed)
        {
            var resolutionContext = new ResolutionContext(this.GetActiveScopeNames(), this.containerContext,
                this == this.containerContext.RootScope, nullResultAllowed,
                    initialParameters: parameterTypes.AsParameters());

            var initExpression = this.containerContext.ResolutionStrategy
                .BuildExpressionForType(resolutionContext, new TypeInformation(type, name));
            if (initExpression == null)
                if (resolutionContext.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type, name);

            var expression = initExpression.AsLambda(resolutionContext.ParameterExpressions.SelectMany(x => x.Select(i => i.I2)));

            var factory = expression.CompileDynamicDelegate(resolutionContext, this.containerContext.ContainerConfiguration);
            Swap.SwapValue(ref this.delegateCache.FactoryDelegates, (t1, t2, t3, t4, c) =>
                c.AddOrUpdate(t1, t2, false), name ?? type, factory, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            return factory(this);
        }

        private HashTree<object, ConstantExpression> ProcessDependencyOverrides(object[] dependencyOverrides)
        {
            if (dependencyOverrides == null && this.scopedInstances.IsEmpty)
                return null;

            var result = new HashTree<object, ConstantExpression>();

            if (!this.scopedInstances.IsEmpty)
                foreach (var scopedInstance in this.scopedInstances.Walk())
                    result.Add(scopedInstance.Key, scopedInstance.Value.AsConstant(), false);

            if (dependencyOverrides == null) return result;

            foreach (var dependencyOverride in dependencyOverrides)
            {
                var type = dependencyOverride.GetType();
                var expression = dependencyOverride.AsConstant();

                result.Add(type, expression, false);

                foreach (var baseType in type.GetRegisterableInterfaceTypes().Concat(type.GetRegisterableBaseTypes()))
                    result.Add(baseType, expression, false);
            }

            return result;
        }
    }
}