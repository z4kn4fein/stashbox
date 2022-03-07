using System.Reflection;

namespace Stashbox.Expressions.Compile
{
    internal class Closure
    {
        public static readonly FieldInfo ConstantsField = typeof(Closure).GetField(nameof(Constants))!;

        public readonly object[] Constants;

        public Closure(object[] constants)
        {
            this.Constants = constants;
        }
    }
}
