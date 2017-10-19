#if NET45 || NET40 || NETSTANDARD1_3
using Stashbox.Entity;
using Stashbox.Utils;
using System;
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

        public Type[] ConcatDelegateTargetAndCapturedArgumentWithParameter(Type[] parameters)
        {
            var count = parameters.Length;
            if (count == 0 && this.HasCapturedVariablesArgument)
                return new[] { this.Target.TargetType, this.CapturedArgumentsHolder.TargetType };

            var indexOffset = this.HasCapturedVariablesArgument ? 2 : 1;

            var types = new Type[count + indexOffset];
            types[0] = this.Target.TargetType;

            if (this.HasCapturedVariablesArgument)
                types[1] = this.CapturedArgumentsHolder.TargetType;

            if (count == 1)
                types[indexOffset] = parameters[0];
            if (count > 1)
                Array.Copy(parameters, 0, types, indexOffset, count);

            return types;
        }

        public Type[] ConcatCapturedArgumentWithParameter(Type[] parameters)
        {
            var count = parameters.Length;
            if (count == 0)
                return new[] { this.CapturedArgumentsHolder.TargetType };

            var types = new Type[count + 1];
            types[0] = this.CapturedArgumentsHolder.TargetType;

            if (count == 1)
                types[1] = parameters[0];
            if (count > 1)
                Array.Copy(parameters, 0, types, 1, count);

            return types;
        }

        public Type[] ConcatCapturedArgumentWithParameterWithReturnType(Type[] parameters, Type returnType) =>
            Utils.ConcatCapturedArgumentWithParameterWithReturnType(parameters, this.CapturedArgumentsHolder.TargetType, returnType);

        public Type[] ConcatDelegateTargetWithParameter(Type[] parameters)
        {
            var count = parameters.Length;
            if (count == 0)
                return new[] { this.Target.TargetType };

            var types = new Type[count + 1];
            types[0] = this.Target.TargetType;

            if (count == 1)
                types[1] = parameters[0];
            if (count > 1)
                Array.Copy(parameters, 0, types, 1, count);

            return types;
        }
    }
}
#endif