using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class DelegateTarget
    {
        public FieldInfo[] Fields { get; }

        public Type TargetType { get; }

        public object Target { get; }

        public DelegateTarget(FieldInfo[] fields, Type targetType, object target)
        {
            this.Fields = fields;
            this.TargetType = targetType;
            this.Target = target;
        }
    }

    internal class CompilerContext
    {
        public Expression[] ClosureExpressions { get; }

        public DelegateTarget Target { get; }

        public CompilerContext(Expression[] closureExpressions, DelegateTarget target)
        {
            this.ClosureExpressions = closureExpressions;
            this.Target = target;
        }

        public bool HasClosure => Target != null;
    }
}
