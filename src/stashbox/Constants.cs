using System;
using System.Linq.Expressions;
using System.Reflection;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Lifetime;

namespace Stashbox
{
    internal static class Constants
    {
        public static ParameterExpression ScopeExpression = Expression.Parameter(typeof(IResolutionScope));

        public static MethodInfo AddDisposalMethod = typeof(IDisposableHandler).GetSingleMethod("AddDisposableTracking");

        public static MethodInfo BuildExtensionMethod = typeof(IContainerExtensionManager).GetSingleMethod("ExecutePostBuildExtensions");

        public static MethodInfo GetScopedValueMethod = typeof(ScopedLifetime).GetSingleMethod("GetScopedValue", true);

        public static Type DisposableType = typeof(IDisposable);

        public static Type FuncType = typeof(Func<>);
    }
}
