#if IL_EMIT
using Stashbox.Utils;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal static class DynamicModule
    {
#if NETSTANDARD
        public static readonly Lazy<ModuleBuilder> ModuleBuilder = new Lazy<ModuleBuilder>(() =>
            DynamicAssemblyBuilder.Value.DefineDynamicModule("Stashbox.Dynamic"));
#else
        public static readonly Lazy<ModuleBuilder> ModuleBuilder = new Lazy<ModuleBuilder>(() =>
            DynamicAssemblyBuilder.Value.DefineDynamicModule("Stashbox.Dynamic"));
#endif

#if NETSTANDARD
        public static readonly Lazy<AssemblyBuilder> DynamicAssemblyBuilder = new Lazy<AssemblyBuilder>(() =>
            AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("Stashbox.Dynamic"),
                AssemblyBuilderAccess.Run));
#else
        public static readonly Lazy<AssemblyBuilder> DynamicAssemblyBuilder = new Lazy<AssemblyBuilder>(() =>
            AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Stashbox.Dynamic"),
                AssemblyBuilderAccess.Run));
#endif
        private static int typeCounter;

        private static ImmutableTree<Type> TargetTypes = ImmutableTree<Type>.Empty;

        private static ImmutableTree<Type> CapturedArgumentTypes = ImmutableTree<Type>.Empty;

        public static Type GetOrAddTargetType(Type[] types)
        {
            var length = types.Length;

            var foundType = TargetTypes.GetOrDefault(length);
            if (foundType != null)
                return foundType.MakeGenericType(types);

            var fields = new FieldInfo[length];
            var typeBuilder = ModuleBuilder.Value.DefineType("Stashbox.Dynamic.DT" + Interlocked.Increment(ref typeCounter), TypeAttributes.Public);

            if (length > 0)
            {
                var typeParamNames = new string[length];
                for (var i = 0; i < length; i++)
                    typeParamNames[i] = "T" + i;

                var typeParams = typeBuilder.DefineGenericParameters(typeParamNames);
#if NETSTANDARD
                var genericTypes = new Type[length];
#endif

                for (var i = 0; i < length; i++)
                {
#if NETSTANDARD
                    var genericType = typeParams[i].AsType();
                    genericTypes[i] = genericType;
                    fields[i] = typeBuilder.DefineField("F" + i, genericType, FieldAttributes.Public);
#else
                    fields[i] = typeBuilder.DefineField("F" + i, typeParams[i], FieldAttributes.Public);
#endif
                }

#if NETSTANDARD
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, genericTypes);
#else
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, typeParams);
#endif
                var generator = constructor.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, Constants.ObjectConstructor);

                for (var i = 0; i < length; i++)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.LoadParameter(i + 1);
                    generator.Emit(OpCodes.Stfld, fields[i]);
                }

                generator.Emit(OpCodes.Ret);
            }

#if NETSTANDARD
            var type = typeBuilder.CreateTypeInfo().AsType();
#else
            var type = typeBuilder.CreateType();
#endif
            Swap.SwapValue(ref TargetTypes, (t1, t2, t3, t4, t) =>
                t.AddOrUpdate(t1, t2), length, type, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            return type.MakeGenericType(types);
        }

        public static Type GetOrAddCapturedArgumentsType(Expression[] expressions)
        {
            var length = expressions.Length;
            var types = new Type[length];

            var foundType = CapturedArgumentTypes.GetOrDefault(length);
            if (foundType != null)
            {
                if (length <= 0) return foundType;

                for (var i = 0; i < length; i++)
                    types[i] = expressions[i].Type;

                return foundType.MakeGenericType(types);
            }

            var fields = new FieldInfo[length];

            var typeBuilder = ModuleBuilder.Value.DefineType("Stashbox.Dynamic.DT" + Interlocked.Increment(ref typeCounter), TypeAttributes.Public);

            var typeParamNames = new string[length];
            for (var i = 0; i < length; i++)
                typeParamNames[i] = "T" + i;

            var typeParams = typeBuilder.DefineGenericParameters(typeParamNames);
#if NETSTANDARD
                var genericTypes = new Type[length];
#endif

            for (var i = 0; i < length; i++)
            {
                types[i] = expressions[i].Type;
#if NETSTANDARD
                    var genericType = typeParams[i].AsType();
                    genericTypes[i] = genericType;
                    fields[i] = typeBuilder.DefineField("F" + i, genericType, FieldAttributes.Public);
#else
                fields[i] = typeBuilder.DefineField("F" + i, typeParams[i], FieldAttributes.Public);
#endif
            }
#if NETSTANDARD
            var type = typeBuilder.CreateTypeInfo().AsType();
#else
            var type = typeBuilder.CreateType();
#endif
            Swap.SwapValue(ref CapturedArgumentTypes, (t1, t2, t3, t4, t) =>
                t.AddOrUpdate(t1, t2), length, type, Constants.DelegatePlaceholder, Constants.DelegatePlaceholder);
            return type.MakeGenericType(types);
        }
    }
}
#endif