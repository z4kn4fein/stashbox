
using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    public interface ILifetime
    {
        object GetInstance(IObjectBuilder objectBuilder, ResolutionInfo resolutionInfo, TypeInformation resolveType);
        Expression GetExpression(IObjectBuilder objectBuilder, Expression resolutionInfoExpression, TypeInformation resolveType);
        void CleanUp();
    }
}
