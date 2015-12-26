using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    public abstract class Resolver
    {
        protected readonly IContainerContext BuilderContext;
        protected readonly TypeInformation TypeInfo;

        protected Resolver(IContainerContext containerContext, TypeInformation typeInfo)
        {
            this.BuilderContext = containerContext;
            this.TypeInfo = typeInfo;
        }

        public abstract object Resolve(ResolutionInfo resolutionInfo);
        public abstract Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression);
    }
}
