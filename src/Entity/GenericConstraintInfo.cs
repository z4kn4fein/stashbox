using System;
using System.Reflection;

namespace Stashbox.Entity
{
    internal class GenericConstraintInfo
    {
        public Type[] TypeConstraints { get; set; }

        public GenericParameterAttributes GenericParameterConstraints { get; set; }
    }
}
