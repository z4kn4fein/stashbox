using System;
using System.Collections.Generic;

namespace Stashbox.Entity
{
    public class TypeInformation
    {
        public Type Type { get; set; }
        public Type ParentType { get; set; }
        public string DependencyName { get; set; }
        public HashSet<Attribute> CustomAttributes { get; set; }
    }
}