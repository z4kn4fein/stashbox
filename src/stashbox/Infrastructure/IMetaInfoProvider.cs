using Stashbox.Entity;
using Stashbox.Overrides;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    public interface IMetaInfoProvider
    {
        Type[] SensitivityList { get; }

        Type TypeTo { get; }

        bool TryChooseConstructor(out ResolutionConstructor constructor, OverrideManager overrideManager = null, IEnumerable<InjectionParameter> injectionParameters = null);
    }
}
