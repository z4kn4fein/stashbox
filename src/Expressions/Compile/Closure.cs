using System.Reflection;
using Stashbox.Utils;

namespace Stashbox.Expressions.Compile;

internal class Closure
{
    public static readonly FieldInfo ConstantsField = TypeCache<Closure>.Type.GetField(nameof(Constants))!;

    public readonly object[] Constants;

    public Closure(object[] constants)
    {
        this.Constants = constants;
    }
}