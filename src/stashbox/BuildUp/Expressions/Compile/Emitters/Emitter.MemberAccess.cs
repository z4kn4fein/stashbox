#if NET45 || NET40 || IL_EMIT
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.BuildUp.Expressions.Compile.Emitters
{
    internal static partial class Emitter
    {
        private static bool TryEmit(this MemberExpression expression, ILGenerator generator, CompilerContext context, params ParameterExpression[] parameters) =>
            expression.Expression.TryEmit(generator, context, parameters) && expression.Member.EmitMemberAccess(generator);

        private static bool EmitMemberAssign(this MemberInfo member, ILGenerator generator)
        {
            if (member is PropertyInfo property)
            {
                var setMethod = property.GetSetterMethodOrDefault(true);
                if (setMethod == null)
                    return false;
                generator.EmitMethod(setMethod);
            }
            else if (member is FieldInfo field)
                generator.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);

            return true;
        }

        private static bool EmitMemberAccess(this MemberInfo member, ILGenerator generator)
        {
            if (member is PropertyInfo property)
            {
                var getMethod = property.GetGetterMethodOrDefault(true);
                if (getMethod == null)
                    return false;
                generator.EmitMethod(getMethod);
            }
            else if (member is FieldInfo field)
                generator.Emit(field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);

            return true;
        }
    }
}
#endif
