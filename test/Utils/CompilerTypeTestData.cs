using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Tests.Utils;

public class CompilerTypeTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { CompilerType.Default };
        yield return new object[] { CompilerType.Microsoft };
        yield return new object[] { CompilerType.Stashbox };
        yield return new object[] { CompilerType.FastExpressionCompiler };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}