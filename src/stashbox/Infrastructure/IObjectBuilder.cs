
using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    public interface IObjectBuilder
    {
        object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType);
        Expression GetExpression(Expression resolutionInfoExpression, TypeInformation resolveType);
        void CleanUp();
    }
}
