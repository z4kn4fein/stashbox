using Stashbox.Entity;
using Stashbox.Entity.Resolution;
using System;

namespace Stashbox.Infrastructure
{
    public interface IMetaInfoProvider
    {
        bool HasGenericTypeConstraints { get; }
        bool TryChooseConstructor(out ResolutionConstructor constructor, ResolutionInfo resolutionInfo);
        ResolutionMethod[] GetResolutionMethods(ResolutionInfo resolutionInfo);
        ResolutionMember[] GetResolutionMembers(ResolutionInfo resolutionInfo);
        bool ValidateGenericContraints(Type type);
    }
}
