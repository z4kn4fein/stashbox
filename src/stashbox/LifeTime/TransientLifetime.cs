using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using System.Linq.Expressions;

namespace Stashbox.Lifetime
{
    public class TransientLifetime : ILifetime
    {
        public object GetInstance(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo)
        {
            Shield.EnsureNotNull(objectBuilder);
            return objectBuilder.BuildInstance(resolutionInfo);
        }

        public void CleanUp()
        {
        }

        public Expression GetExpression(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo)
        {
            return objectBuilder.GetExpression(resolutionInfo);
        }
    }
}
