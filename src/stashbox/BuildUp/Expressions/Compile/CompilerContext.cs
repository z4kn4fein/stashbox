#if NET45 || NET40 || NETSTANDARD1_3
using Stashbox.Entity;
using Stashbox.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class CompilerContext
    {
        private readonly AtomicBool hasCapturedVariablesArgumentConstructed;

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

        public bool HasCapturedVariablesArgumentConstructed => !this.hasCapturedVariablesArgumentConstructed.CompareExchange(false, true);

        public CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments,
            KeyValue<LambdaExpression, Expression[]>[] nestedLambdas, CapturedArgumentsHolder capturedArgumentsHolder)
            : this(target, definedVariables, storedExpressions, capturedArguments, nestedLambdas, capturedArgumentsHolder, false, false)
        { }

        private CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments,
            KeyValue<LambdaExpression, Expression[]>[] nestedLambdas, CapturedArgumentsHolder capturedArgumentsHolder, bool isNestedLambda, bool hasCapturedVariablesArgumentConstructed)
        {
            this.hasCapturedVariablesArgumentConstructed = new AtomicBool(hasCapturedVariablesArgumentConstructed);
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
                this.CapturedArgumentsHolder, isNestedLambda, this.hasCapturedVariablesArgumentConstructed.Value);
    }
}
#endif