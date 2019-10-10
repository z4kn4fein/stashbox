using Stashbox.Configuration;
using System.Collections.Generic;
using Xunit;

namespace Stashbox.Tests.IssueTests
{

    public class DictionariesGetResolvedToArraysOfKeyTypeByDefault
    {
        [Fact]
        public void Ensure_Dictionary_Resolves()
        {
            var container = new StashboxContainer(c => c.WithUnknownTypeResolution(c2 =>
                c2.WithConstructorSelectionRule(Rules.ConstructorSelection.PreferLeastParameters)));
            var dict = container.Resolve<Dictionary<string, object>>();
        }
    }
}
