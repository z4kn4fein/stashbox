using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    internal interface IMetaInfoProvider
    {
        Type TypeTo { get; }
        bool HasGenericTypeConstraints { get; }
        bool TryChooseConstructor(out ResolutionConstructor constructor, ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null);
        ResolutionMethod[] GetResolutionMethods(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null);
        ResolutionMember[] GetResolutionMembers(ResolutionInfo resolutionInfo, InjectionParameter[] injectionParameters = null);
        bool ValidateGenericContraints(TypeInformation typeInformation);
    }
}
