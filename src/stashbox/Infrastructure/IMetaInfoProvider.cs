using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    internal interface IMetaInfoProvider
    {
        Type TypeTo { get; }
        bool HasInjectionMethod { get; }
        bool HasInjectionMembers { get; }
        bool HasGenericTypeConstraints { get; }
        HashSet<Type> SensitivityList { get; }
        bool TryChooseConstructor(out ResolutionConstructor constructor, ResolutionInfo resolutionInfo = null, InjectionParameter[] injectionParameters = null);
        IEnumerable<ResolutionMethod> GetResolutionMethods(ResolutionInfo resolutionInfo = null, InjectionParameter[] injectionParameters = null);
        IEnumerable<ResolutionMember> GetResolutionMembers(ResolutionInfo resolutionInfo = null, InjectionParameter[] injectionParameters = null);
        bool ValidateGenericContraints(TypeInformation typeInformation);
    }
}
