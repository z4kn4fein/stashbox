
using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    public interface ILifetime
    {
        object GetInstance(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType);
        Expression GetExpression(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType);
        void CleanUp();
    }
}
