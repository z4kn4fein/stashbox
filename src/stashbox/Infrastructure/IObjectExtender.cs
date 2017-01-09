using Stashbox.Entity;
using Stashbox.Entity.Resolution;

namespace Stashbox.Infrastructure
{
    internal interface IObjectExtender
    {
        object FillResolutionMembers(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo);
        object FillResolutionMethods(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo);
        ResolutionMember[] GetResolutionMembers();
        void ServiceUpdated(RegistrationInfo registrationInfo);
    }
}
