using Stashbox.BuildUp;
using Stashbox.Registration;
using Stashbox.Resolution;
using System;
using System.Linq.Expressions;
using System.Threading;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a shared base class for scoped lifetimes.
    /// </summary>
    public abstract class ScopedLifetimeBase : LifetimeBase
    {
        private static int scopeIdCounter;

        /// <summary>
        /// The id of the scope.
        /// </summary>
        protected readonly int ScopeId = Interlocked.Increment(ref scopeIdCounter);

        /// <summary>
        /// The object used to synchronization.
        /// </summary>
        protected readonly object Sync = new object();

        /// <summary>
        /// Produces a factory expression to produce scoped instances.
        /// </summary>
        /// <param name="containerContext">The container context.</param>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="objectBuilder">The object builder.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <param name="resolveType">The resolve type.</param>
        /// <returns>The factory expression.</returns>
        public Expression GetFactoryExpression(IContainerContext containerContext, IServiceRegistration serviceRegistration, IObjectBuilder objectBuilder, ResolutionContext resolutionContext, Type resolveType)
        {
            var expr = base.GetExpression(containerContext, serviceRegistration, objectBuilder, resolutionContext, resolveType);
            return expr?.CompileDelegate(resolutionContext).AsConstant();//.AsLambda(resolutionContext.CurrentScopeParameter);
        }

        /// <summary>
        /// Stores the given expression in a local variable and saves it into the resolution context for further reuse.
        /// </summary>
        /// <param name="expression">The expression to store.</param>
        /// <param name="resolutionContext">The resolution context.</param>
        /// <param name="resolveType">The resolve type.</param>
        /// <returns>The local variable.</returns>
        public Expression StoreExpressionIntoLocalVariable(Expression expression, ResolutionContext resolutionContext, Type resolveType)
        {
            var variable = resolveType.AsVariable();
            resolutionContext.AddDefinedVariable(this.ScopeId, variable);
            resolutionContext.AddInstruction(variable.AssignTo(expression));
            return variable;
        }
    }
}
