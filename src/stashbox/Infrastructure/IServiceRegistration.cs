using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    public interface IServiceRegistration
    {
        object GetInstance(ResolutionInfo resolutionInfo);
        Expression GetExpression(ResolutionInfo resolutionInfo);
        bool IsUsableForCurrentContext(TypeInformation resolutionInfo);
        bool HasCondition { get; }
        void CleanUp();
    }
}
