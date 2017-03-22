using System;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Lifetime;

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
        public static ParameterExpression ScopeExpression = Expression.Parameter(typeof(IResolutionScope));

        internal static MethodInfo AddDisposalMethod = typeof(IDisposableHandler).GetSingleMethod("AddDisposableTracking");

        internal static MethodInfo BuildExtensionMethod = typeof(IContainerExtensionManager).GetSingleMethod("ExecutePostBuildExtensions");

        internal static MethodInfo GetScopedValueMethod = typeof(ScopedLifetime).GetSingleMethod("GetScopedValue", true);

        internal static Type DisposableType = typeof(IDisposable);

        internal static Type FuncType = typeof(Func<>);
    }
}
