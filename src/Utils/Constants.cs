using Stashbox.ContainerExtension;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Stashbox.Utils
{
    /// <summary>
    /// Holds the constant values used by the conainer.
    /// </summary>
    public static class Constants
    {
        internal static readonly Type ResolutionScopeType = typeof(IResolutionScope);

        internal static readonly Type ResolverType = typeof(IDependencyResolver);

        internal static readonly ParameterExpression ResolutionScopeParameter = ResolutionScopeType.AsParameter("scope");

        internal static readonly MethodInfo AddDisposalMethod = ResolutionScopeType.GetSingleMethod("AddDisposableTracking");

        internal static readonly MethodInfo GetOrAddScopedItemMethod = ResolutionScopeType.GetSingleMethod("GetOrAddScopedItem");

        internal static readonly MethodInfo AddWithFinalizerMethod = ResolutionScopeType.GetSingleMethod("AddWithFinalizer");

        internal static readonly PropertyInfo RootScopeProperty = ResolutionScopeType.GetSingleProperty("RootScope");

        internal static readonly MethodInfo GetScopedInstanceMethod = ResolutionScopeType.GetSingleMethod("GetScopedInstanceOrDefault");

        internal static readonly MethodInfo CheckRuntimeCircularDependencyBarrierMethod = ResolutionScopeType.GetSingleMethod("CheckRuntimeCircularDependencyBarrier");

        internal static readonly MethodInfo ResetRuntimetCircularDependencyBarrierMethod = ResolutionScopeType.GetSingleMethod("ResetRuntimetCircularDependencyBarrier");

        internal static readonly MethodInfo BuildExtensionMethod = typeof(IContainerExtensionManager).GetSingleMethod("ExecutePostBuildExtensions");

        internal static readonly MethodInfo BeginScopeMethod = ResolverType.GetSingleMethod("BeginScope");

        internal static readonly Type DisposableType = typeof(IDisposable);

        internal static readonly Type FuncType = typeof(Func<>);

        internal static readonly Type[] EmptyTypes = new Type[0];

        internal static readonly Expression[] EmptyExpressions = new Expression[0];

        internal static readonly Type ObjectType = typeof(object);

        internal static readonly ConstructorInfo ObjectConstructor = ObjectType.GetConstructor(EmptyTypes);

        internal static readonly Type CompositionRootType = typeof(ICompositionRoot);

        internal static readonly Type DependencyAttributeType = typeof(Attributes.DependencyAttribute);

        internal static readonly Type InjectionAttributeType = typeof(Attributes.InjectionMethodAttribute);

        internal const MethodImplOptions Inline = (MethodImplOptions)256;

        internal const byte DelegatePlaceholder = 0;
    }
}
