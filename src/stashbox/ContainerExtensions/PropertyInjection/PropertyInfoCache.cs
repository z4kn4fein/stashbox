using Stashbox.Infrastructure;
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
        public Resolver Resolver { get; set; }
        public string DependencyName { get; set; }
        public object PropertyValue { get; set; }
        public Type PropertyType { get; set; }
        public Action<object, object> PropertySetter { get; set; }
    }
}
