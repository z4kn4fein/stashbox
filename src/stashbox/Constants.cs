using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;

namespace Stashbox
{
    /// <summary>
    /// Holds the constant values used by the conainer.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The scope parameter expression.
        /// </summary>
        public static readonly ParameterExpression ScopeExpression = Expression.Parameter(typeof(IResolutionScope));

        internal static readonly MethodInfo AddDisposalMethod = typeof(IResolutionScope).GetSingleMethod("AddDisposableTracking");

        internal static readonly MethodInfo AddWithFinalizerMethod = typeof(IResolutionScope).GetSingleMethod("AddWithFinalizer");

        internal static readonly MethodInfo BuildExtensionMethod = typeof(IContainerExtensionManager).GetSingleMethod("ExecutePostBuildExtensions");

        internal static readonly MethodInfo GetScopedInstanceMethod = typeof(IResolutionScope).GetSingleMethod("GetScopedInstanceOrDefault");

        internal static readonly Type DisposableType = typeof(IDisposable);

        internal static readonly Type FuncType = typeof(Func<>);

        internal static readonly Type ResolverType = typeof(IDependencyResolver);

        internal static readonly Type[] EmptyTypes = new Type[0];

        internal static readonly Type ObjectType = typeof(object);

        internal static readonly ConstructorInfo ObjectConstructor = ObjectType.GetConstructor(EmptyTypes);

        internal static readonly Type CompositionRootType = typeof(ICompositionRoot);

        internal static readonly Type DependencyAttributeType = typeof(Attributes.DependencyAttribute);

        internal const MethodImplOptions Inline = (MethodImplOptions)256;
    }
}
