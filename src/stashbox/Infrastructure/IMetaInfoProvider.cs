
using Stashbox.Entity;
using Stashbox.Overrides;
using System;

namespace Stashbox.Infrastructure
{
    public interface IMetaInfoProvider
    {
        Type TypeTo { get; }

        bool TryChooseConstructor(out ResolutionConstructor constructor, OverrideManager overrideManager = null);
    }
}
