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
        bool TryChooseConstructor(out ResolutionConstructor constructor, InjectionParameter[] injectionParameters = null);
        IEnumerable<ResolutionMethod> GetResolutionMethods(InjectionParameter[] injectionParameters = null);
        IEnumerable<ResolutionMember> GetResolutionMembers(InjectionParameter[] injectionParameters = null);
        bool ValidateGenericContraints(TypeInformation typeInformation);
    }
}
