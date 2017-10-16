using System.Linq.Expressions;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class LambdaContext
    {
        public Expression[] DefinedVariables { get; }

        public LambdaContext(Expression[] definedVariables)
        {
            this.DefinedVariables = definedVariables;
        }
    }
}
