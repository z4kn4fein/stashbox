using Stashbox.Entity;
using System;
using System.Collections.Generic;

namespace Stashbox.ContainerExtensions.MethodInjection
{
    public class MethodInfoCache
    {
        public HashSet<MethodInfoItem> Methods { get; set; }
    }

    public class MethodInfoItem
    {
        public HashSet<ResolutionParameter> Parameters { get; set; }
        public Action<object, object[]> MethodDelegate { get; set; }
    }
}
