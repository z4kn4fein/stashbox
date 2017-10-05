using System.Linq.Expressions;

namespace Stashbox.BuildUp.Expressions.Compile
{
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
