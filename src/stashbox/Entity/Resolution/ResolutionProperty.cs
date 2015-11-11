using System;
using System.Reflection;

namespace Stashbox.Entity.Resolution
{
    public class ResolutionProperty
    {
        public ResolutionTarget ResolutionTarget { get; set; }
        public Action<object, object> PropertySetter { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
    }
}
