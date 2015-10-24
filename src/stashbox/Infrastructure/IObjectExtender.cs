using Stashbox.Entity;

namespace Stashbox.Infrastructure
{
    internal interface IObjectExtender
    {
        object ExtendObject(object instance, IContainerContext containerContext, ResolutionInfo resolutionInfo);
    }
}
