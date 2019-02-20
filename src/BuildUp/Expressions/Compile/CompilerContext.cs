#if IL_EMIT
using Stashbox.Entity;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class CompilerContext
    {
        private int hasCapturedVariablesArgumentConstructed;

        public Expression[] DefinedVariables { get; }

        public Expression[] CapturedArguments { get; }

        public Expression[] StoredExpressions { get; }

        public DelegateTarget Target { get; }

        public CapturedArgumentsHolder CapturedArgumentsHolder { get; }

        public bool IsNestedLambda { get; }

        public LocalBuilder[] LocalBuilders { get; set; }

        public LocalBuilder CapturedArgumentsHolderVariable { get; set; }

        public KeyValue<LambdaExpression, Expression[]>[] NestedLambdas { get; }

        public bool HasClosure => this.Target != null;

        public bool HasCapturedVariablesArgument => this.CapturedArguments.Length > 0;

        public bool HasCapturedVariablesArgumentConstructed => Interlocked.CompareExchange(ref this.hasCapturedVariablesArgumentConstructed, 1, 0) != 0;

        public CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments,
            KeyValue<LambdaExpression, Expression[]>[] nestedLambdas, CapturedArgumentsHolder capturedArgumentsHolder)
            : this(target, definedVariables, storedExpressions, capturedArguments, nestedLambdas, capturedArgumentsHolder, false, 0)
        { }

        private CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments,
            KeyValue<LambdaExpression, Expression[]>[] nestedLambdas, CapturedArgumentsHolder capturedArgumentsHolder, bool isNestedLambda, int hasCapturedVariablesArgumentConstructed)
        {
            this.hasCapturedVariablesArgumentConstructed = hasCapturedVariablesArgumentConstructed;
            this.Target = target;
            this.DefinedVariables = definedVariables;
            this.StoredExpressions = storedExpressions;
            this.CapturedArgumentsHolder = capturedArgumentsHolder;
            this.IsNestedLambda = isNestedLambda;
            this.CapturedArguments = capturedArguments;
            this.NestedLambdas = nestedLambdas;
        }

        public CompilerContext CreateNew(Expression[] definedVariables, bool isNestedLambda) =>
            new CompilerContext(this.Target, definedVariables, this.StoredExpressions, this.CapturedArguments, this.NestedLambdas,
                this.CapturedArgumentsHolder, isNestedLambda, this.hasCapturedVariablesArgumentConstructed);
    }
}
#endif