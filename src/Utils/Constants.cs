using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    internal static class Constants
    {
        public static readonly Type ResolutionScopeType = typeof(IResolutionScope);

#if HAS_SERVICEPROVIDER
        public static readonly Type ServiceProviderType = typeof(IServiceProvider);
#endif

        public static readonly Type ResolverType = typeof(IDependencyResolver);

        public static readonly ParameterExpression ResolutionScopeParameter = ResolutionScopeType.AsParameter("scope");

        public static readonly MethodInfo AddDisposalMethod = ResolutionScopeType.GetSingleMethod(nameof(IResolutionScope.AddDisposableTracking));

        public static readonly MethodInfo GetOrAddScopedObjectMethod = ResolutionScopeType.GetSingleMethod(nameof(IResolutionScope.GetOrAddScopedObject));

        public static readonly MethodInfo AddWithFinalizerMethod = ResolutionScopeType.GetSingleMethod(nameof(IResolutionScope.AddWithFinalizer));

        public static readonly MethodInfo CheckRuntimeCircularDependencyBarrierMethod = ResolutionScopeType.GetSingleMethod(nameof(IResolutionScope.CheckRuntimeCircularDependencyBarrier));

        public static readonly MethodInfo ResetRuntimeCircularDependencyBarrierMethod = ResolutionScopeType.GetSingleMethod(nameof(IResolutionScope.ResetRuntimeCircularDependencyBarrier));

        public static readonly MethodInfo BeginScopeMethod = ResolverType.GetSingleMethod(nameof(IDependencyResolver.BeginScope));

        public static readonly Type DisposableType = typeof(IDisposable);

#if HAS_ASYNC_DISPOSABLE
        public static readonly Type AsyncDisposableType = typeof(IAsyncDisposable);
#endif

        public static readonly Type FuncType = typeof(Func<>);

        public static readonly Type[] EmptyTypes = EmptyArray<Type>();

        public static readonly Type ObjectType = typeof(object);

        public static readonly Type CompositionRootType = typeof(ICompositionRoot);

        public static readonly Type DependencyAttributeType = typeof(Attributes.DependencyAttribute);

        public static readonly Type InjectionAttributeType = typeof(Attributes.InjectionMethodAttribute);

        public const MethodImplOptions Inline = (MethodImplOptions)256;

        public const byte DelegatePlaceholder = 0;

        public static T[] EmptyArray<T>() => InternalArrayHelper<T>.Empty;
    }
}
