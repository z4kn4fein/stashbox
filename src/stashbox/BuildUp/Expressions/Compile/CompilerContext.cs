#if NET45 || NET40 || NETSTANDARD1_3
using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
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

        public bool HasCapturedVariablesArgument { get; }

        public LocalBuilder[] LocalBuilders { get; set; }

        public bool HasClosure => this.Target != null;

        public bool HasCapturedVariablesArgumentConstructed => !this.hasCapturedVariablesArgumentConstructed.CompareExchange(false, true);

        public CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments, CapturedArgumentsHolder capturedArgumentsHolder, bool hasCapturedVariablesArgument = false)
            : this(target, definedVariables, storedExpressions, capturedArguments, capturedArgumentsHolder, hasCapturedVariablesArgument, false)
        { }

        private CompilerContext(DelegateTarget target, Expression[] definedVariables, Expression[] storedExpressions, Expression[] capturedArguments, CapturedArgumentsHolder capturedArgumentsHolder, bool hasCapturedVariablesArgument, bool hasCapturedVariablesArgumentConstructed)
        {
            this.hasCapturedVariablesArgumentConstructed = new AtomicBool(hasCapturedVariablesArgumentConstructed);
            this.Target = target;
            this.DefinedVariables = definedVariables;
            this.StoredExpressions = storedExpressions;
            this.HasCapturedVariablesArgument = hasCapturedVariablesArgument;
            this.CapturedArgumentsHolder = capturedArgumentsHolder;
            this.CapturedArguments = capturedArguments;
        }

        public CompilerContext CreateNew(Expression[] definedVariables, bool hasCapturedVariablesArgument) =>
            new CompilerContext(this.Target, definedVariables, this.StoredExpressions, this.CapturedArguments, this.CapturedArgumentsHolder, hasCapturedVariablesArgument, this.HasCapturedVariablesArgumentConstructed);

        //public Type[] ConcatDelegateTargetAndCapturedArgumentWithParameter(Type[] parameters)
        //{
        //    var count = parameters.Length;
        //    if (count == 0 && this.HasCapturedVariablesArgument)
        //        return new[] { this.Target.TargetType, this.CapturedArgumentsHolder.TargetType };

        //    var indexOffset = this.HasCapturedVariablesArgument ? 2 : 1;

        //    var types = new Type[count + indexOffset];
        //    types[0] = this.Target.TargetType;

        //    if (this.HasCapturedVariablesArgument)
        //        types[1] = this.CapturedArgumentsHolder.TargetType;

        //    if (count == 1)
        //        types[indexOffset] = parameters[0];
        //    if (count > 1)
        //        Array.Copy(parameters, 0, types, indexOffset, count);

        //    return types;
        //}

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
    }
}
#endif