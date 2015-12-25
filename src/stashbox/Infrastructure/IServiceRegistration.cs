using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    public interface IServiceRegistration
    {
        object GetInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType);
        Expression GetExpression(Expression resolutionInfoExpression, TypeInformation resolveType);
        bool IsUsableForCurrentContext(TypeInformation resolutionInfo);
        bool HasCondition { get; }
        void CleanUp();
    }
}
