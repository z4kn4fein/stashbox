#if NET45 || NET40 || NETSTANDARD1_3
using Stashbox.Utils;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class DynamicModule
    {
#if NETSTANDARD1_3
        public static readonly Lazy<ModuleBuilder> ModuleBuilder = new Lazy<ModuleBuilder>(() =>
            DynamicAssemblyBuilder.Value.DefineDynamicModule("Stashbox.Dynamic"));
#else
        public static readonly Lazy<ModuleBuilder> ModuleBuilder = new Lazy<ModuleBuilder>(() =>
            DynamicAssemblyBuilder.Value.DefineDynamicModule("Stashbox.Dynamic", "Stashbox.Dynamic.dll"));
#endif

#if NETSTANDARD1_3
        public static readonly Lazy<AssemblyBuilder> DynamicAssemblyBuilder = new Lazy<AssemblyBuilder>(() =>
            AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName("Stashbox.Dynamic"),
                AssemblyBuilderAccess.Run));
#else
        public static readonly Lazy<AssemblyBuilder> DynamicAssemblyBuilder = new Lazy<AssemblyBuilder>(() =>
            AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Stashbox.Dynamic"),
                AssemblyBuilderAccess.RunAndSave, "c:\\temp"));
#endif

        private static int typeCounter;

        private static readonly ConcurrentTree<Type> TargetTypes = new ConcurrentTree<Type>();

        public static Type GetDelegateTargetOrDefault(Expression[] expressions, object[] objects)
        {
            if (expressions.Length > 0)
                return GetOrAddTargetType(expressions, objects);

            return null;
        }

        private static Type GetOrAddTargetType(Expression[] expressions, object[] objects)
        {
            var length = expressions.Length;
            var types = new Type[length];

            var foundType = TargetTypes.GetOrDefault(length);
            if (foundType != null)
            {
                if (length <= 0) return foundType;

                for (var i = 0; i < length; i++)
                    types[i] = expressions[i].Type;

                return foundType.MakeGenericType(types);
            }

            var fields = new FieldInfo[length];
            var typeBuilder = ModuleBuilder.Value.DefineType("Stashbox.Dynamic.DT" + Interlocked.Increment(ref typeCounter), TypeAttributes.Public);

            if (length > 0)
            {
                var typeParamNames = new string[length];
                for (var i = 0; i < length; i++)
                    typeParamNames[i] = "T" + i;

                var typeParams = typeBuilder.DefineGenericParameters(typeParamNames);
#if NETSTANDARD1_3
                var genericTypes = new Type[length];
#endif

                for (var i = 0; i < length; i++)
                {
                    types[i] = expressions[i].Type;
#if NETSTANDARD1_3
                    var genericType = typeParams[i].AsType();
                    genericTypes[i] = genericType;
                    fields[i] = typeBuilder.DefineField("F" + i, genericType, FieldAttributes.Public);
#else
                    fields[i] = typeBuilder.DefineField("F" + i, typeParams[i], FieldAttributes.Public);
#endif
                }

#if NETSTANDARD1_3
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, genericTypes);
#else
                var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, typeParams);
#endif
                var generator = constructor.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Call, Stashbox.Constants.ObjectConstructor);

                for (var i = 0; i < length; i++)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.LoadParameter(i + 1);
                    generator.Emit(OpCodes.Stfld, fields[i]);
                }

                generator.Emit(OpCodes.Ret);
            }
#if NETSTANDARD1_3
            var type = typeBuilder.CreateTypeInfo().AsType();
#else
            var type = typeBuilder.CreateType();
#endif
            TargetTypes.AddOrUpdate(length, type);
            return type.MakeGenericType(types);
        }
    }
}
#endif