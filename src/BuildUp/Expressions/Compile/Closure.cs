#if IL_EMIT
using Stashbox.Utils;
using System.Reflection;

namespace Stashbox.BuildUp.Expressions.Compile
{
    internal class Closure
    {
        public static readonly FieldInfo ConstantsField = typeof(Closure).GetTypeInfo().GetDeclaredField(nameof(Constants));

        public readonly object[] Constants;

        public Closure(object[] constants)
        {
            this.Constants = constants;
        }
    }
}
#endif
