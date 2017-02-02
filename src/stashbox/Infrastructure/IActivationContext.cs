using Stashbox.Entity;

namespace Stashbox.Infrastructure
{
    internal interface IActivationContext
    {
        object Activate(ResolutionInfo resolutionInfo, TypeInformation typeInfo);
    }
}
