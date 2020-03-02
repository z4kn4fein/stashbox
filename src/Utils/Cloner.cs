using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Stashbox.Utils
{
#if IL_EMIT
    internal static class Cloner<TCloneable>
    {
        private static readonly Func<TCloneable, TCloneable> CloneDelegate = ConstructDelegate();

        public static TCloneable Clone(TCloneable cloneable) => CloneDelegate(cloneable);

        private static Func<TCloneable, TCloneable> ConstructDelegate()
        {
            var type = typeof(TCloneable);
            var cloneMethod = new DynamicMethod("Clone", type, new[] { type }, true);
            var constructor = type.GetTypeInfo().DeclaredConstructors.First(c => !c.GetParameters().Any());

            var generator = cloneMethod.GetILGenerator();

            var loc1 = generator.DeclareLocal(type);

            generator.Emit(OpCodes.Newobj, constructor);
            generator.Emit(OpCodes.Stloc, loc1);

            var fields = type.GetTypeInfo().DeclaredFields;
            foreach (var field in fields)
            {
                generator.Emit(OpCodes.Ldloc, loc1);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, field);
                generator.Emit(OpCodes.Stfld, field);
            }

            generator.Emit(OpCodes.Ldloc, loc1);
            generator.Emit(OpCodes.Ret);

            return (Func<TCloneable, TCloneable>)cloneMethod.CreateDelegate(typeof(Func<TCloneable, TCloneable>));
        }
    }
#endif
}
