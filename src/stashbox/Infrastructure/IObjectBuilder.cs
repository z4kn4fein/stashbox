
using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    public interface IObjectBuilder
    {
        object BuildInstance(ResolutionInfo resolutionInfo);
        Expression GetExpression(ResolutionInfo resolutionInfo);
        void CleanUp();
    }
}
