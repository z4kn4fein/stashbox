using System;

namespace Stashbox.Entity.Resolution
{
    public class ResolutionProperty
    {
        public ResolutionTarget ResolutionTarget { get; set; }
        public Action<object, object> PropertySetter { get; set; }
    }
}
