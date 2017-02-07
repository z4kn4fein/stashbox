using System;
using Stashbox.Entity;

namespace Stashbox.Infrastructure.Resolution
{
    internal interface IActivationContext
    {
        object Activate(ResolutionInfo resolutionInfo, TypeInformation typeInfo);

        Delegate ActivateFactory(ResolutionInfo resolutionInfo, TypeInformation typeInfo, Type parameterType);
    }
}
