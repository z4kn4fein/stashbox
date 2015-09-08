
using System;
namespace Stashbox.Overrides
{
    public class TypeOverride : Override
    {
        public Type OverrideType { get; private set; }

        public TypeOverride(Type type, object value)
            : base(value)
        {
            this.OverrideType = type;
        }
    }
}
