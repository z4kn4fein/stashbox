using Stashbox.Resolution;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils;

internal static class Constants
{
    public static readonly ParameterExpression ResolutionScopeParameter = TypeCache<IResolutionScope>.Type.AsParameter("scope");

    public static readonly ParameterExpression RequestContextParameter = TypeCache<IRequestContext>.Type.AsParameter("request");

    public static readonly MethodInfo AddDisposalMethod = TypeCache<IResolutionScope>.Type.GetMethod(nameof(IResolutionScope.AddDisposableTracking))!;

    public static readonly MethodInfo AddRequestContextAwareDisposalMethod = TypeCache<IResolutionScope>.Type.GetMethod(nameof(IResolutionScope.AddRequestContextAwareDisposableTracking))!;

    public static readonly MethodInfo GetOrAddScopedObjectMethod = TypeCache<IResolutionScope>.Type.GetMethod(nameof(IResolutionScope.GetOrAddScopedObject))!;

    public static readonly MethodInfo AddWithFinalizerMethod = TypeCache<IResolutionScope>.Type.GetMethod(nameof(IResolutionScope.AddWithFinalizer))!;

    public static readonly MethodInfo AddWithAsyncInitializerMethod = TypeCache<IResolutionScope>.Type.GetMethod(nameof(IResolutionScope.AddWithAsyncInitializer))!;

    public static readonly MethodInfo GetOrAddInstanceMethod = TypeCache<IInternalRequestContext>.Type.GetMethod(nameof(IInternalRequestContext.GetOrAddInstance))!;
        
    public static readonly MethodInfo ResolveMethod =
        TypeCache<IDependencyResolver>.Type.GetMethod(nameof(IDependencyResolver.Resolve), new[] { TypeCache<Type>.Type,
            TypeCache<object>.Type, TypeCache<object[]>.Type })!;

    public static readonly MethodInfo BeginScopeMethod = TypeCache<IDependencyResolver>.Type.GetMethod(nameof(IDependencyResolver.BeginScope))!;

    public const MethodImplOptions Inline = (MethodImplOptions)256;

    public const byte DelegatePlaceholder = 0;
}