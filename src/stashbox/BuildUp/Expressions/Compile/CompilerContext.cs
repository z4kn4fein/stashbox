#if NET45 || NET40 || NETSTANDARD1_3
using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Stashbox.Entity;
using Stashbox.Utils;

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

        public LocalBuilder[] LocalBuilders { get; set; }

        public KeyValue<LambdaExpression, Expression[]>[] NestedLambdas { get; }

        public bool HasClosure => this.Target != null;

        public bool HasCapturedVariablesArgument => this.CapturedArguments.Length > 0;

        public bool HasCapturedVariablesArgumentConstructed => !this.hasCapturedVariablesArgumentConstructed.CompareExchange(false, true);

        public CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments, KeyValue<LambdaExpression, Expression[]>[] nestedLambdas, 
            CapturedArgumentsHolder capturedArgumentsHolder) 
            : this(target, definedVariables, storedExpressions, capturedArguments, nestedLambdas, capturedArgumentsHolder, false)
        { }
        
        private CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments, KeyValue<LambdaExpression, Expression[]>[] nestedLambdas,
            CapturedArgumentsHolder capturedArgumentsHolder, bool hasCapturedVariablesArgumentConstructed)
        {
            this.hasCapturedVariablesArgumentConstructed = new AtomicBool(hasCapturedVariablesArgumentConstructed);
            this.Target = target;
            this.DefinedVariables = definedVariables;
            this.StoredExpressions = storedExpressions;
            this.CapturedArgumentsHolder = capturedArgumentsHolder;
            this.CapturedArguments = capturedArguments;
            this.NestedLambdas = nestedLambdas;
        }

        public CompilerContext CreateNew(Expression[] definedVariables) =>
            new CompilerContext(this.Target, definedVariables, this.StoredExpressions, this.CapturedArguments, this.NestedLambdas, 
                this.CapturedArgumentsHolder);

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

        public Type[] ConcatCapturedArgumentWithParameterWithReturnType(Type[] parameters, Type returnType)
        {
            var count = parameters.Length;
            if (count == 0)
                return new[] { this.CapturedArgumentsHolder.TargetType, returnType };

            var types = new Type[count + 2];
            types[0] = this.CapturedArgumentsHolder.TargetType;
            types[1] = returnType;

            if (count == 1)
                types[2] = parameters[0];
            if (count > 1)
                Array.Copy(parameters, 0, types, 2, count);

            return types;
        }

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