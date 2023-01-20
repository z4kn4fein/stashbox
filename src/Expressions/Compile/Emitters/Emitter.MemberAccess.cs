using Stashbox.Expressions.Compile.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.Expressions.Compile.Emitters;

internal static partial class Emitter
{
    private static bool TryEmit(this MemberExpression expression, ILGenerator generator, CompilerContext context,
        params ParameterExpression[] parameters)
    {
        if (expression.Expression == null)
            return false;

        return expression.Expression.TryEmit(generator, context, parameters) && expression.Member.EmitMemberAccess(generator);
    }


    private static bool EmitMemberAssign(this MemberInfo member, ILGenerator generator)
    {
        switch (member)
        {
            case PropertyInfo property:
            {
                var setMethod = property.GetSetMethod(true);
                if (setMethod == null)
                    return false;
                generator.EmitMethod(setMethod);
                break;
            }
            case FieldInfo field:
                generator.Emit(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);
                break;
        }

        return true;
    }

    private static bool EmitMemberAccess(this MemberInfo member, ILGenerator generator)
    {
        switch (member)
        {
            case PropertyInfo property:
            {
                var getMethod = property.GetGetMethod(true);
                if (getMethod == null)
                    return false;
                generator.EmitMethod(getMethod);
                break;
            }
            case FieldInfo field:
                generator.Emit(field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld, field);
                break;
        }

        return true;
    }
}