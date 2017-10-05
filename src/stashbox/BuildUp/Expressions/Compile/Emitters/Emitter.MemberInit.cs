#if NET45 || NET40 || NETSTANDARD1_3
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this MemberInitExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters)
        {
            if (!expression.NewExpression.TryEmit(generator, context, parameters))
                return false;

            var length = expression.Bindings.Count;
            for (var i = 0; i < length; i++)
            {
                var binding = expression.Bindings[i];
                if (binding.BindingType != MemberBindingType.Assignment)
                    return false;

                generator.Emit(OpCodes.Dup);

                if (!((MemberAssignment)binding).Expression.TryEmit(generator, context, parameters))
                    return false;

                if (binding.Member is PropertyInfo property)
                {
                    var setMethod = property.GetSetMethod();
                    if (setMethod == null)
                        return false;
                    generator.EmitMethod(setMethod);
                }
                else if (binding.Member is FieldInfo field)
                    generator.Emit(OpCodes.Stfld, field);
            }

            return true;
        }
    }
}
#endif
