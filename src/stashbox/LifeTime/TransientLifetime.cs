using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    public class TransientLifetime : ILifetime
    {
        public object GetInstance(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return objectBuilder.BuildInstance(resolutionInfo, resolveType);
        }
        public Expression GetExpression(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            return objectBuilder.GetExpression(resolutionInfo, resolutionInfoExpression, resolveType);
        }

        public void CleanUp()
        {
        }
    }
}
