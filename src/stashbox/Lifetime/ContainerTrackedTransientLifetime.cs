using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.Lifetime
{
    /// <summary>
    /// Represents a tracked transient lifetime.
    /// </summary>
    public class ContainerTrackedTransientLifetime : LifetimeBase
    {
        private readonly MethodInfo trackMethodInfo;

        /// <summary>
        /// Constructs a <see cref="ContainerTrackedTransientLifetime"/>
        /// </summary>
        public ContainerTrackedTransientLifetime()
        {
            this.trackMethodInfo = this.GetType().GetTypeInfo().GetDeclaredMethod("AddTransientObjectTracking");
        }

        /// <inheritdoc />
        public override object GetInstance(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return this.AddTransientObjectTracking(containerContext, base.GetInstance(containerContext, objectBuilder, resolutionInfo, resolveType));
        }

        /// <inheritdoc />
        public override Expression GetExpression(IContainerContext containerContext, IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            var expression = base.GetExpression(containerContext, objectBuilder, resolutionInfo, resolutionInfoExpression, resolveType);
            var call = Expression.Call(Expression.Constant(this), this.trackMethodInfo, Expression.Constant(containerContext), expression);
            return Expression.Convert(call, resolveType.Type);
        }

        /// <inheritdoc />
        public override ILifetime Create()
        {
            return new ContainerTrackedTransientLifetime();
        }

        private object AddTransientObjectTracking(IContainerContext containerContext, object instance)
        {
            containerContext.TrackedTransientObjects.Add(instance);
            return instance;
        }
    }
}
