using Stashbox.Overrides;
using System.Collections.Generic;

namespace Stashbox.Entity
{
    public class ResolutionInfo
    {
        public OverrideManager OverrideManager { get; set; }
        public IEnumerable<object> FactoryParams { get; set; }
    }
}