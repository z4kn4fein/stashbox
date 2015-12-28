
using Stashbox.Entity;
using System.Linq.Expressions;

namespace Stashbox.Infrastructure
{
    public interface IObjectBuilder
    {
        object BuildInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType);
        Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType);
        void ServiceUpdated(RegistrationInfo registrationInfo);
        void CleanUp();
    }
}
