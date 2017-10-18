using System;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class DelegateTarget
    {
        public FieldInfo[] Fields { get; }

        public Type TargetType { get; }

        public object Target { get; }

        public DelegateTarget(FieldInfo[] fields,Type targetType, object target)
        {
            this.Fields = fields;
            this.TargetType = targetType;
            this.Target = target;
        }
    }
}