using Stashbox.Entity;
using System;
using System.Collections.Generic;

namespace Stashbox.ContainerExtensions.PropertyInjection
{
    public class PropertyInfoCache
    {
        public HashSet<PropertyInfoItem> Properties { get; set; }
    }

    public class PropertyInfoItem
    {
        public ResolutionTarget ResolutionTarget { get; set; }
        public Action<object, object> PropertySetter { get; set; }
    }
}
