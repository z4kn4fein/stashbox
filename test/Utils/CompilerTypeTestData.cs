using System.Collections;
using System.Collections.Generic;

namespace Stashbox.Tests.Utils;

public class CompilerTypeTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return [CompilerType.Default];
        yield return [CompilerType.Microsoft];
        yield return [CompilerType.Stashbox];
        yield return [CompilerType.FastExpressionCompiler];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}