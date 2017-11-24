using System;
using System.Linq.Expressions;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class LambdaContext
    {
        public Expression[] DefinedVariables { get; }

        public Type StoredLambdaType { get; set; }

        public LambdaContext(Expression[] definedVariables)
        {
            this.DefinedVariables = definedVariables;
        }
    }
}
