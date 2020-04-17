#if IL_EMIT
using Stashbox.Entity;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class CompilerContext
    {
        public Expression[] DefinedVariables { get; }

        public Expression[] CapturedArguments { get; }

        public Expression[] StoredExpressions { get; }

        public DelegateTarget Target { get; }

        public CapturedArgumentsHolder CapturedArgumentsHolder { get; }

        public bool IsNestedLambda { get; }

        public LocalBuilder[] LocalBuilders { get; set; }

        public LocalBuilder CapturedArgumentsHolderVariable { get; set; }

        public LambdaExpression[] NestedLambdas { get; }

        public Expression[][] NestedLambdaVariables { get; }

        public bool HasClosure => this.Target != null;

        public bool HasCapturedVariablesArgument => this.CapturedArguments.Length > 0;

        public CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments,
            LambdaExpression[] nestedLambdas, Expression[][] nestedLambdaVariables, CapturedArgumentsHolder capturedArgumentsHolder)
            : this(target, definedVariables, storedExpressions, capturedArguments, nestedLambdas, nestedLambdaVariables, capturedArgumentsHolder, false)
        { }

        private CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments,
            LambdaExpression[] nestedLambdas, Expression[][] nestedLambdaVariables, CapturedArgumentsHolder capturedArgumentsHolder, bool isNestedLambda)
        {
            this.Target = target;
            this.DefinedVariables = definedVariables;
            this.StoredExpressions = storedExpressions;
            this.CapturedArgumentsHolder = capturedArgumentsHolder;
            this.IsNestedLambda = isNestedLambda;
            this.CapturedArguments = capturedArguments;
            this.NestedLambdas = nestedLambdas;
            this.NestedLambdaVariables = nestedLambdaVariables;
        }

        public CompilerContext CreateNew(Expression[] definedVariables, bool isNestedLambda) =>
            new CompilerContext(this.Target, definedVariables,
                this.StoredExpressions, this.CapturedArguments,
                this.NestedLambdas, this.NestedLambdaVariables,
                this.CapturedArgumentsHolder, isNestedLambda);
    }
}
#endif