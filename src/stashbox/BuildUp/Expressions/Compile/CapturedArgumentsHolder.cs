using System;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class CapturedArgumentsHolder
    {
        public FieldInfo[] Fields { get; }

        public Type TargetType { get; }

        public CapturedArgumentsHolder(FieldInfo[] fields, Type targetType)
        {
            this.Fields = fields;
            this.TargetType = targetType;
        }
    }
}
