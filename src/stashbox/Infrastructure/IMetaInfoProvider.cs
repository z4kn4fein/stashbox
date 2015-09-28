using Stashbox.Entity;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    public interface IMetaInfoProvider
    {
        Type[] SensitivityList { get; }

        Type TypeTo { get; }

        bool TryChooseConstructor(out ResolutionConstructor constructor, ResolutionInfo resolutionInfo = null, HashSet<InjectionParameter> injectionParameters = null);
    }
}
