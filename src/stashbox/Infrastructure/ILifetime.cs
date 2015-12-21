
using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    public interface ILifetime
    {
        object GetInstance(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo);
        Expression GetExpression(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo);
        void CleanUp();
    }
}
