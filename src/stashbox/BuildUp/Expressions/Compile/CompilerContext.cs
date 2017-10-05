using System.Linq.Expressions;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class DelegateTarget
    {
        public FieldInfo[] Fields { get; }

        public object Target { get; }

        public DelegateTarget(FieldInfo[] fields, object target)
        {
            this.Fields = fields;
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
    }
}
