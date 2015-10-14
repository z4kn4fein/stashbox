using Stashbox.Entity;
using System;
namespace Stashbox.Infrastructure
{
    public interface IServiceRegistration
    {
        object GetInstance(ResolutionInfo resolutionInfo);
        bool IsUsable(ResolutionInfo resolutionInfo);
        bool IsUsableAtPlanBuilding(Type targetConditionType);
        void CleanUp();
    }
}
